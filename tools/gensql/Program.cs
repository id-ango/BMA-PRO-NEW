using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// Reads the WithGroup excel and emits a single .sql script that:
//  1. creates staging table #Map(ItemCode, GroupPath)
//  2. ensures IcItemGroup hierarchy exists for each distinct path
//  3. updates IcItem.ItemGroupId + IcItem.SearchTags
// Run in SSMS / Azure Data Studio against the app database.

var excel = args.Length > 0 ? args[0] : @"D:\Folder Kerja\Folder PT\LaporanStock_20261030_150356_WithGroup.xlsx";
var outSql = args.Length > 1 ? args[1] : @"D:\Folder Kerja\Folder PT\ImportItemGroups.sql";

XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
List<string> shared; XDocument sheet;
using (var zip = ZipFile.OpenRead(excel))
{
    using (var r = new StreamReader(zip.GetEntry("xl/sharedStrings.xml").Open()))
        shared = XDocument.Parse(r.ReadToEnd()).Root.Elements(ns + "si")
            .Select(si => string.Concat(si.Descendants(ns + "t").Select(t => t.Value))).ToList();
    using (var r = new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml").Open()))
        sheet = XDocument.Parse(r.ReadToEnd());
}

string CellText(XElement c)
{
    var v = c.Element(ns + "v"); if (v == null) return "";
    return (string)c.Attribute("t") == "s" ? shared[int.Parse(v.Value)] : v.Value;
}

string Q(string s) => "N'" + s.Replace("'", "''") + "'";
string CodeFor(string name)
{
    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["STOCK"]="STK",["SPAREPART"]="SPR",["MACHINE"]="MES",["LABORATORY"]="LAB",["CONSUMABLE"]="CON",
        ["SERVICE"]="SRV",["GENERAL"]="GEN",["FABRICATION"]="FAB",["HAMMERMILL"]="HMR",["CLEANER"]="CLN",
        ["DESTONER"]="DST",["BLOWER"]="BLW",["ELEVATOR"]="ELV",["PADDY HUSKER"]="HSK",["PADDY SEPARATOR"]="SEP",
        ["RICE WHITENER"]="WHN",["RICE SIFTER"]="SFT",["RICE POLISHER"]="POL",["COMPRESSOR"]="CMP",
        ["PACKING MACHINE"]="PCK",["MAGNET"]="MGN",["COLOR SORTER"]="CLR",["LOW SPEED ELEVATOR"]="ELV",
        ["CONVEYOR"]="CNV",["SCREEN"]="SCR",["AIRLOCK"]="AIR",["MOTOR"]="MTR",["MOTOR / DINAMO"]="MTR",
        ["SHAFT / AS"]="SHF",["SHAFT"]="SHF",["RING"]="RIN",["PIRINGAN"]="PIR",["TOOLS"]="TLS",
        ["MESIN"]="MSN",["IMPELLER"]="IMP",["BODY"]="BDY",["BUCKET"]="BKT",["BELT"]="BLT",["BEARING"]="BRG",
        ["PNEUMATIC"]="PNE",["RUBBER ROLL"]="RBR",["V-BELT"]="BLT",["HOUSING"]="HSG",["SENSOR"]="SNS",
        ["PULLY"]="PL",["TRANSMISSION"]="TRN",["VFD"]="VFD",["KNIFE"]="KNF",["SPRING"]="SPR",["PANEL"]="PNL",
        ["NOZZLE / SPRAY"]="NOZ",["MAGHELIC"]="MGH",["OIL FILTER"]="OIL",["AIR FILTER"]="AIR",
        ["OIL SEPARATOR"]="SEP",["GEARBOX"]="GBX",["CYLINDER"]="CYL",["LOAD CELL"]="LDC",
        ["CONTROLLER / DISPLAY"]="CTL",["NEEDLE"]="NDL",["PCB / MAINBOARD"]="PCB",["EJECTOR"]="EJC",
        ["MOISTURE METER"]="MOI",["DIGITAL SCALE"]="DSC",["TEST SIEVE"]="SVE",["GREASE"]="GRS",["OIL"]="OIL",
        ["CHEMICAL"]="CHM",["BOLT"]="BLT",["NUT"]="NUT",["CABLE TIE"]="CBT",["KABEL"]="KBL",
        ["STAVOLT"]="STV",["POWER SUPPLY"]="PLW",["SELENOID"]="SLD",["HOPPER"]="HPR",["FRAME"]="FRM",["TANK"]="TNK"
    };
    return map.TryGetValue(name, out var c) ? c : new string(name.Where(char.IsLetterOrDigit).Take(3).ToArray()).ToUpperInvariant();
}

// collect rows
var rows = new List<(string Code, string Path)>();
foreach (var row in sheet.Root.Element(ns + "sheetData").Elements(ns + "row"))
{
    var cells = row.Elements(ns + "c")
        .ToDictionary(c => new string(((string)c.Attribute("r")).TakeWhile(char.IsLetter).ToArray()), c => CellText(c));
    cells.TryGetValue("B", out var code);
    cells.TryGetValue("I", out var group);
    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(group) && group != "Group")
        rows.Add((code.Trim(), group.Trim()));
}

var paths = rows.Select(r => r.Path).Distinct().OrderBy(p => p).ToList();

var sb = new StringBuilder();
sb.AppendLine("-- Import Item Groups from Excel (generated)");
sb.AppendLine("-- Run against the inventory database. Safe to re-run (idempotent).");
sb.AppendLine("SET NOCOUNT ON;");
sb.AppendLine("BEGIN TRAN;");
sb.AppendLine();
sb.AppendLine("IF OBJECT_ID('tempdb..#Map') IS NOT NULL DROP TABLE #Map;");
sb.AppendLine("CREATE TABLE #Map (ItemCode NVARCHAR(100) NOT NULL, GroupPath NVARCHAR(400) NOT NULL);");
sb.AppendLine();

// insert mapping in batches
const int batch = 200;
for (int i = 0; i < rows.Count; i += batch)
{
    sb.AppendLine("INSERT INTO #Map (ItemCode, GroupPath) VALUES");
    var vals = rows.Skip(i).Take(batch).Select(r => $"({Q(r.Code)}, {Q(r.Path)})");
    sb.AppendLine(string.Join(",\n", vals) + ";");
    sb.AppendLine();
}

// build groups using a recursive CTE per path is complex in T-SQL; instead emit per-path ensure blocks
sb.AppendLine("DECLARE @g TABLE (Path NVARCHAR(400) PRIMARY KEY, LeafId INT, Tags NVARCHAR(500));");
sb.AppendLine("DECLARE @p INT, @id INT;");
sb.AppendLine();

foreach (var path in paths)
{
    var parts = path.Split('>', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    sb.AppendLine($"-- {path}");
    sb.AppendLine($"SET @p = NULL;");
    foreach (var part in parts)
    {
        var code = CodeFor(part);
        // find existing by (parent,name) OR (parent,code)
        sb.AppendLine($"SELECT @id = ItemGroupId FROM IcItemGroup WHERE (ParentId IS NULL AND @p IS NULL OR ParentId = @p) AND (Name = {Q(part)} OR Code = {Q(code)});");
        sb.AppendLine($"IF @id IS NULL BEGIN");
        sb.AppendLine($"    IF NOT EXISTS (SELECT 1 FROM IcItemGroup WHERE (ParentId IS NULL AND @p IS NULL OR ParentId = @p) AND Code = {Q(code)})");
        sb.AppendLine($"        INSERT INTO IcItemGroup (Code, Name, ParentId) VALUES ({Q(code)}, {Q(part)}, @p);");
        sb.AppendLine($"    SELECT @id = ItemGroupId FROM IcItemGroup WHERE (ParentId IS NULL AND @p IS NULL OR ParentId = @p) AND (Name = {Q(part)} OR Code = {Q(code)});");
        sb.AppendLine($"END");
        sb.AppendLine($"SET @p = @id;");
    }
    var tags = string.Join(' ', parts.Distinct());
    sb.AppendLine($"INSERT INTO @g (Path, LeafId, Tags) VALUES ({Q(path)}, @p, {Q(tags)});");
    sb.AppendLine();
}

// update items
sb.AppendLine("-- Apply to items");
sb.AppendLine("UPDATE i");
sb.AppendLine("SET i.ItemGroupId = g.LeafId, i.SearchTags = g.Tags");
sb.AppendLine("FROM IcItems i");
sb.AppendLine("JOIN #Map m ON m.ItemCode = i.ItemCode");
sb.AppendLine("JOIN @g g ON g.Path = m.GroupPath;");
sb.AppendLine();
sb.AppendLine("DECLARE @c INT = @@ROWCOUNT;");
sb.AppendLine("DECLARE @missing INT;");
sb.AppendLine("SELECT @missing = COUNT(*) FROM #Map m WHERE NOT EXISTS (SELECT 1 FROM IcItems i WHERE i.ItemCode = m.ItemCode);");
sb.AppendLine("PRINT 'Updated items: ' + CAST(@c AS NVARCHAR(20));");
sb.AppendLine("PRINT 'Items in mapping not found in IcItem: ' + CAST(@missing AS NVARCHAR(20));");
sb.AppendLine();
sb.AppendLine("-- Review then COMMIT. To abort: ROLLBACK;");
sb.AppendLine("COMMIT TRAN;");
sb.AppendLine("DROP TABLE #Map;");

File.WriteAllText(outSql, sb.ToString(), new UTF8Encoding(true));
Console.WriteLine($"Rows mapped: {rows.Count}");
Console.WriteLine($"Distinct group paths: {paths.Count}");
Console.WriteLine($"SQL written: {outSql}");
