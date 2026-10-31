using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

// ===== CONFIG (override via args) =====
// usage: importgroup [--excel "path.xlsx"] [--conn "connStr"] [--dry]
string ArgVal(string key, string def)
{
    var i = Array.FindIndex(args, a => a.Equals(key, StringComparison.OrdinalIgnoreCase));
    return (i >= 0 && i + 1 < args.Length) ? args[i + 1] : def;
}
var excel = ArgVal("--excel", @"D:\Folder Kerja\Folder PT\LaporanStock_20261030_150356_WithGroup.xlsx");
var connStr = ArgVal("--conn", @"Server=localhost\SQLEXPRESS;Database=aspnet-GlobalSoft01;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=True;");
bool dryRun = args.Any(a => a.Equals("--dry", StringComparison.OrdinalIgnoreCase));

XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

// ===== READ EXCEL (shared strings + sheet) =====
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

// rows: code -> group path
var items = new List<(string Code, string GroupPath)>();
foreach (var row in sheet.Root.Element(ns + "sheetData").Elements(ns + "row"))
{
    var cells = row.Elements(ns + "c")
        .ToDictionary(c => new string(((string)c.Attribute("r")).TakeWhile(char.IsLetter).ToArray()), c => CellText(c));
    cells.TryGetValue("B", out var code);
    cells.TryGetValue("I", out var group);
    if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(group) && group != "Group")
        items.Add((code.Trim(), group.Trim()));
}
Console.WriteLine($"Excel items with group: {items.Count}");

// ===== LOAD DB =====
using var conn = new SqlConnection(connStr);
conn.Open();

var groups = new List<(int Id, string Code, string Name, int? ParentId)>();
using (var cmd = new SqlCommand("SELECT ItemGroupId, Code, Name, ParentId FROM IcItemGroup", conn))
using (var rd = cmd.ExecuteReader())
    while (rd.Read())
        groups.Add((rd.GetInt32(0), rd.GetString(1), rd.GetString(2), rd.IsDBNull(3) ? (int?)null : rd.GetInt32(3)));

var dbItems = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
using (var cmd = new SqlCommand("SELECT ItemCode, IcItemId FROM IcItem", conn))
using (var rd = cmd.ExecuteReader())
    while (rd.Read())
        dbItems[rd.GetString(0)] = rd.GetInt32(1);

Console.WriteLine($"DB groups: {groups.Count}, DB items: {dbItems.Count}");

// helper: map group name -> code
string CodeFor(string name) => name switch
{
    "STOCK" => "STK", "SPAREPART" => "SPR", "MACHINE" => "MES", "LABORATORY" => "LAB",
    "CONSUMABLE" => "CON", "SERVICE" => "SRV", "GENERAL" => "GEN", "FABRICATION" => "FAB",
    "HAMMERMILL" => "HMR", "CLEANER" => "CLN", "DESTONER" => "DST", "BLOWER" => "BLW",
    "ELEVATOR" => "ELV", "PADDY HUSKER" => "HSK", "PADDY SEPARATOR" => "SEP",
    "RICE WHITENER" => "WHN", "RICE SIFTER" => "SFT", "RICE POLISHER" => "POL",
    "COMPRESSOR" => "CMP", "PACKING MACHINE" => "PCK", "MAGNET" => "MGN", "COLOR SORTER" => "CLR",
    "LOW SPEED ELEVATOR" => "ELV", "CONVEYOR" => "CNV",
    "SCREEN" => "SCR", "AIRLOCK" => "AIR", "MOTOR" => "MTR", "MOTOR / DINAMO" => "MTR",
    "SHAFT / AS" => "SHF", "SHAFT" => "SHF", "RING" => "RIN", "PIRINGAN" => "PIR", "TOOLS" => "TLS",
    "MESIN" => "MSN", "IMPELLER" => "IMP", "BODY" => "BDY", "BUCKET" => "BKT", "BELT" => "BLT",
    "BEARING" => "BRG", "PNEUMATIC" => "PNE", "RUBBER ROLL" => "RBR", "V-BELT" => "BLT",
    "HOUSING" => "HSG", "SENSOR" => "SNS", "PULLY" => "PL", "TRANSMISSION" => "TRN", "VFD" => "VFD",
    "KNIFE" => "KNF", "SPRING" => "SPR", "PANEL" => "PNL", "NOZZLE / SPRAY" => "NOZ",
    "MAGHELIC" => "MGH", "OIL FILTER" => "OIL", "AIR FILTER" => "AIR", "OIL SEPARATOR" => "SEP",
    "GEARBOX" => "GBX", "CYLINDER" => "CYL", "LOAD CELL" => "LDC", "CONTROLLER / DISPLAY" => "CTL",
    "NEEDLE" => "NDL", "PCB / MAINBOARD" => "PCB", "EJECTOR" => "EJC",
    "MOISTURE METER" => "MOI", "DIGITAL SCALE" => "DSC", "TEST SIEVE" => "SVE",
    "GREASE" => "GRS", "OIL" => "OIL", "CHEMICAL" => "CHM",
    "BOLT" => "BLT", "NUT" => "NUT", "CABLE TIE" => "CBT", "KABEL" => "KBL", "STAVOLT" => "STV",
    "POWER SUPPLY" => "PLW", "SELENOID" => "SLD", "HOPPER" => "HPR", "FRAME" => "FRM", "TANK" => "TNK",
    _ => new string(name.Where(char.IsLetterOrDigit).Take(3).ToArray()).ToUpperInvariant()
};

// ensure a group path exists; returns leaf group id and builds search tags (names)
(int leafId, string tags) EnsurePath(string path)
{
    var parts = path.Split('>', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    int? parentId = null;
    var names = new List<string>();
    foreach (var part in parts)
    {
        names.Add(part);
        var g = groups.FirstOrDefault(x => x.ParentId == parentId &&
                    (x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(CodeFor(part), StringComparison.OrdinalIgnoreCase)));
        if (g == default)
        {
            var code = CodeFor(part);
            // avoid duplicate code under same parent
            var existing = groups.FirstOrDefault(x => x.ParentId == parentId && x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (existing != default) { g = existing; }
            else
            {
                if (dryRun) { Console.WriteLine($"  [dry] create group {path} :: {part}"); g = (-1 * (groups.Count + 1), code, part, parentId); groups.Add(g); parentId = -1; continue; }
                using var ins = new SqlCommand(
                    "INSERT INTO IcItemGroup (Code, Name, ParentId) OUTPUT INSERTED.ItemGroupId VALUES (@c,@n,@p)", conn);
                ins.Parameters.AddWithValue("@c", code);
                ins.Parameters.AddWithValue("@n", part);
                ins.Parameters.AddWithValue("@p", (object)parentId ?? DBNull.Value);
                var id = (int)ins.ExecuteScalar();
                g = (id, code, part, parentId);
                groups.Add(g);
                Console.WriteLine($"  + created group: {string.Join(" > ", names)}");
            }
        }
        parentId = g.Id;
    }
    return (parentId!.Value, string.Join(' ', names.Distinct()));
}

int updated = 0, notFound = 0, created = 0, skipped = 0;
foreach (var (code, path) in items)
{
    if (!dbItems.TryGetValue(code, out var itemId))
    {
        notFound++;
        continue;
    }
    var (leafId, tags) = EnsurePath(path);
    if (leafId < 0) { skipped++; continue; } // dry run created
    if (dryRun) continue;

    using var upd = new SqlCommand(
        "UPDATE IcItem SET ItemGroupId=@g, SearchTags=@t WHERE IcItemId=@id", conn);
    upd.Parameters.AddWithValue("@g", leafId);
    upd.Parameters.AddWithValue("@t", (object)tags ?? DBNull.Value);
    upd.Parameters.AddWithValue("@id", itemId);
    upd.ExecuteNonQuery();
    updated++;
}

Console.WriteLine();
Console.WriteLine($"Updated items : {updated}");
Console.WriteLine($"Not in DB     : {notFound}");
Console.WriteLine($"Dry-run paths : {skipped}");
Console.WriteLine(dryRun ? "MODE: DRY RUN (no changes)" : "MODE: APPLIED");
