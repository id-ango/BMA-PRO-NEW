using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

var src = @"D:\Folder Kerja\Folder PT\LaporanStock_20261030_150356.xlsx";
var dst = @"D:\Folder Kerja\Folder PT\LaporanStock_20261030_150356_WithGroup.xlsx";

XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

// --- read shared strings + sheet ---
string sheetXml;
List<string> shared;
using (var zip = ZipFile.OpenRead(src))
{
    var sst = zip.GetEntry("xl/sharedStrings.xml");
    using (var r = new StreamReader(sst.Open()))
    {
        var doc = XDocument.Parse(r.ReadToEnd());
        shared = doc.Root.Elements(ns + "si").Select(si =>
            string.Concat(si.Descendants(ns + "t").Select(t => t.Value))).ToList();
    }
    var sh = zip.GetEntry("xl/worksheets/sheet1.xml");
    using (var r = new StreamReader(sh.Open()))
        sheetXml = r.ReadToEnd();
}

var sheet = XDocument.Parse(sheetXml);

string CellText(XElement c)
{
    var t = (string)c.Attribute("t");
    var v = c.Element(ns + "v");
    if (v == null) return "";
    if (t == "s") return shared[int.Parse(v.Value)];
    return v.Value;
}

string Classify(string code, string name)
{
    string c = (code ?? "").ToUpperInvariant();
    string n = (name ?? "").ToUpperInvariant();
    string Base = "STOCK > SPAREPART";

    // machines / whole units
    if (n.Contains("COLOR SORTER") && (c.StartsWith("CS") || n.Contains("CHUTE"))) return "STOCK > MACHINE > COLOR SORTER";
    if (n.Contains("AIR COMPRESSOR") || c.StartsWith("AC")) return "STOCK > MACHINE > COMPRESSOR";
    if (c.StartsWith("TC") || n.Contains("TANGKI COMPRESSOR")) return "STOCK > MACHINE > COMPRESSOR";
    if (n.Contains("ELEVATOR BUCKET LOW SPEED") || c.StartsWith("LS")) return "STOCK > MACHINE > LOW SPEED ELEVATOR";
    if (n.Contains("GEAR BOX FOR COMBINE MILL")) return "STOCK > MACHINE > COLOR SORTER";

    // airlock
    if (c.StartsWith("A0") || n.Contains("AIRLOCK")) return Base + " > HAMMERMILL > AIRLOCK";
    // buckets
    if (c.StartsWith("B0") || n.Contains("BUCKET")) return Base + " > ELEVATOR > BUCKET";
    // laboratory
    if (c.StartsWith("LAP") || c.StartsWith("KTS") || n.Contains("MOISTUR") || n.Contains("TESTER") || n.Contains("RICETER") || n.Contains("GRADER") || n.Contains("LDS-1G") || n.Contains("LTJM") || n.Contains("SAMPLE TRAY")) return "STOCK > LABORATORY";
    // conveyor / belt rolls
    if (n.Contains("CONVEYOR") || n.Contains("CONV.") || n.Contains("CON.PVC") || n.Contains("ROLL FOR CONVEYOR")) return "STOCK > MACHINE > CONVEYOR";
    // emery roller -> whitener
    if (n.Contains("EMERY ROLLER")) return Base + " > RICE WHITENER";
    // color sorter boards/parts
    if (n.Contains("SORTER") || n.Contains("PAPAN") || n.Contains("CAMERA") || n.Contains("KAMERA") || n.Contains("INPUT HOPPER"))
    {
        if (n.Contains("BOARD") || n.Contains("MAIN BOARD") || n.Contains("PAPAN") || n.Contains("RELAY") || n.Contains("VIBRATOR")) return Base + " > COLOR SORTER > PCB / MAINBOARD";
        if (n.Contains("HOPPER")) return "FABRICATION > HOPPER";
        return Base + " > COLOR SORTER";
    }
    // TDG / inverter / repeater electronic
    if (n.Contains("INVERTER") || n.Contains("INVENTER") || n.Contains("REPEATER") || n.Contains("RELAY") || n.Contains("MAIN BOARD") || n.Contains("MOTHER BOARD") || n.Contains("CPU") || n.Contains("PLC")) return "GENERAL > POWER SUPPLY";
    // belts (pvc/white/van/vbelt/timming/generic)
    if (n.Contains("PVC BELT") || n.Contains("WHITE BELT") || n.Contains("VAN BELT") || n.Contains("VBELT") || n.Contains("TIMMING BELT") || n.Contains("TIMING BELT") || n.Contains("BELT")) return "GENERAL > BELT";
    // combine mill 6LN / ZH-A1 / TDG -> paddy husker
    if (n.Contains("6LN") || n.Contains("ZH-A1") || c.StartsWith("SCM") || c.StartsWith("TDG")) return Base + " > PADDY HUSKER";
    // controller display / panel compact
    if (n.Contains("CONTROLLER DISPLAY") || n.Contains("PANEL COMPACT")) return "GENERAL > POWER SUPPLY";
    // pangkon / bracket / dudukan
    if (n.Contains("PANGKON") || n.Contains("BRACKET") || n.Contains("DUDUKAN")) return "GENERAL > TOOLS";
    // gear box
    if (n.Contains("GEAR BOX") || n.Contains("GEARBOX")) return Base + " > PACKING MACHINE > GEARBOX";
    // aspirator
    if (n.Contains("ASPIRATOR") || n.Contains("JETPOLE") || c.StartsWith("ASP") || c.StartsWith("IB")) return Base + " > BLOWER > IMPELLER";
    // fresh rice machine
    if (n.Contains("FRESH RICE MACHINE") || c.StartsWith("ZLN")) return "STOCK > MACHINE > RICE POLISHER";
    // stone for destoner
    if (n.Contains("STONE FOR") || c.StartsWith("ETQN")) return Base + " > DESTONER";
    // base for husker
    if (n.Contains("BASE FOR TH") || n.Contains("IRON BASE")) return Base + " > PADDY HUSKER > HOUSING";
    // transmission
    if (n.Contains("TRANSMISSION")) return Base + " > PADDY SEPARATOR > TRANSMISSION";

    // magnet
    if (c.StartsWith("MAGNET")) return Base + " > MAGNET";

    // screens by machine
    if (n.Contains("SCREEN") && n.Contains("HAMMER")) return Base + " > HAMMERMILL > SCREEN";
    if (n.Contains("SCREEN") && n.Contains("DESTONER")) return Base + " > DESTONER > SCREEN";
    if (n.Contains("SCREEN") && (n.Contains("SEPARATOR") || n.Contains("MGCZ") || n.Contains("6LN"))) return Base + " > PADDY SEPARATOR > SCREEN";
    if (n.Contains("SCREEN") && (n.Contains("WHITENER") || n.Contains("CFN") || n.Contains("MNMLS") || n.Contains("MLS"))) return Base + " > RICE WHITENER > SCREEN";
    if (n.Contains("SCREEN") && n.Contains("MMJP")) return Base + " > RICE SIFTER > SCREEN";
    if (n.Contains("SCREEN") && n.Contains("KB")) return Base + " > RICE POLISHER > SCREEN";
    if (n.Contains("SCREEN") || n.Contains("SARANGAN")) return "GENERAL > SCREEN";

    // husker
    if (n.Contains("HUSKER") || c.StartsWith("HSS") || n.Contains("TH10") || n.Contains("MLGQ"))
    {
        if (n.Contains("MOTOR")) return Base + " > PADDY HUSKER > MOTOR";
        if (n.Contains("SHAFT")) return Base + " > PADDY HUSKER > SHAFT";
        if (n.Contains("PNEUMATIC") || n.Contains("CYLINDER")) return Base + " > PADDY HUSKER > PNEUMATIC";
        if (n.Contains("RUBBER ROLL")) return Base + " > PADDY HUSKER > RUBBER ROLL";
        if (n.Contains("BELT")) return Base + " > PADDY HUSKER > V-BELT";
        if (n.Contains("HOUSING") || n.Contains("HOUSE")) return Base + " > PADDY HUSKER > HOUSING";
        if (n.Contains("BEARING")) return Base + " > PADDY HUSKER > BEARING";
        return Base + " > PADDY HUSKER";
    }

    // compressor sparepart (before separator, since it contains "SEPARATOR")
    if (n.Contains("COMPRESSOR"))
    {
        if (n.Contains("OIL FILTER")) return Base + " > COMPRESSOR > OIL FILTER";
        if (n.Contains("AIR FILTER")) return Base + " > COMPRESSOR > AIR FILTER";
        if (n.Contains("OIL SEPARATOR") || n.Contains("SEPARATOR")) return Base + " > COMPRESSOR > OIL SEPARATOR";
        if (n.Contains("MOTOR")) return Base + " > COMPRESSOR > MOTOR";
        return Base + " > COMPRESSOR";
    }

    // separator
    if (n.Contains("SEPARATOR") || n.Contains("MGCZ") || c.StartsWith("SCZ"))
    {
        if (n.Contains("SHAFT")) return Base + " > PADDY SEPARATOR > SHAFT";
        if (n.Contains("TRANSMISSION")) return Base + " > PADDY SEPARATOR > TRANSMISSION";
        if (n.Contains("PULLY") || n.Contains("PULLEY")) return Base + " > PADDY SEPARATOR > PULLY";
        if (n.Contains("VFD")) return Base + " > PADDY SEPARATOR > VFD";
        return Base + " > PADDY SEPARATOR";
    }

    // whitener
    if (n.Contains("WHITENER") || c.StartsWith("RWS") || n.Contains("MNMS") || n.Contains("CFM") || n.Contains("CFN"))
    {
        if (n.Contains("KNIFE")) return Base + " > RICE WHITENER > KNIFE";
        if (n.Contains("SHAFT")) return Base + " > RICE WHITENER > SHAFT";
        if (n.Contains("BEARING")) return Base + " > RICE WHITENER > BEARING";
        if (n.Contains("MOTOR")) return Base + " > RICE WHITENER > MOTOR";
        return Base + " > RICE WHITENER";
    }

    // sifter
    if (n.Contains("SIFTER") || n.Contains("MMJP") || c.StartsWith("RSS"))
    {
        if (n.Contains("VFD")) return Base + " > RICE SIFTER > VFD";
        if (n.Contains("MOTOR")) return Base + " > RICE SIFTER > MOTOR";
        if (n.Contains("PANEL")) return Base + " > RICE SIFTER > PANEL";
        if (n.Contains("SPRING")) return Base + " > RICE SIFTER > SPRING";
        return Base + " > RICE SIFTER";
    }

    // polisher
    if (n.Contains("POLISHER") || n.Contains("MPGL") || n.Contains("KB40") || n.Contains("KB 40") || c.StartsWith("SWP") || n.Contains("WATER FLOW"))
    {
        if (n.Contains("NOZZLE") || n.Contains("NEPPLE") || n.Contains("NEPLE") || n.Contains("SPRAY")) return Base + " > RICE POLISHER > NOZZLE / SPRAY";
        if (n.Contains("MAGHELIC")) return Base + " > RICE POLISHER > MAGHELIC";
        if (n.Contains("MOTOR") || n.Contains("AMPERE")) return Base + " > RICE POLISHER > MOTOR";
        return Base + " > RICE POLISHER";
    }

    // packing
    if (n.Contains("PACKING") || n.Contains("DCS") || n.Contains("FLOW SCALE") || n.Contains("SEWING") || c.StartsWith("RPS") || c.StartsWith("SWG") || n.Contains("LOAD CELL") || n.Contains("LOADING CELL"))
    {
        if (n.Contains("GEARBOX") || n.Contains("GEAR BOX")) return Base + " > PACKING MACHINE > GEARBOX";
        if (n.Contains("CYLINDER")) return Base + " > PACKING MACHINE > CYLINDER";
        if (n.Contains("LOAD CELL") || n.Contains("LOADING CELL")) return Base + " > PACKING MACHINE > LOAD CELL";
        if (n.Contains("CONTROLLER") || n.Contains("DISPLAY") || n.Contains("PANEL")) return Base + " > PACKING MACHINE > CONTROLLER / DISPLAY";
        if (n.Contains("NEEDLE")) return Base + " > PACKING MACHINE > NEEDLE";
        if (n.Contains("PNEUMATIC") || n.Contains("GRIPPER")) return Base + " > PACKING MACHINE > PNEUMATIC";
        return Base + " > PACKING MACHINE";
    }

    // compressor sparepart
    if (n.Contains("COMPRESSOR") || n.Contains("COMPRESSOR"))
    {
        if (n.Contains("OIL FILTER")) return Base + " > COMPRESSOR > OIL FILTER";
        if (n.Contains("AIR FILTER")) return Base + " > COMPRESSOR > AIR FILTER";
        if (n.Contains("SEPARATOR")) return Base + " > COMPRESSOR > OIL SEPARATOR";
        if (n.Contains("MOTOR")) return Base + " > COMPRESSOR > MOTOR";
        return Base + " > COMPRESSOR";
    }

    // blower
    if (n.Contains("BLOWER"))
    {
        if (n.Contains("IMPELLER")) return Base + " > BLOWER > IMPELLER";
        if (n.Contains("MOTOR") || n.Contains("MESIN")) return Base + " > BLOWER > MESIN";
        return Base + " > BLOWER";
    }

    // elevator
    if (n.Contains("ELEVATOR"))
    {
        if (n.Contains("BODY")) return Base + " > ELEVATOR > BODY";
        if (n.Contains("BUCKET")) return Base + " > ELEVATOR > BUCKET";
        if (n.Contains("BELT")) return Base + " > ELEVATOR > BELT";
        return Base + " > ELEVATOR";
    }

    // hammermill
    if (n.Contains("HAMMER") || n.Contains("HAMMERMILL") || c.StartsWith("CPL"))
    {
        if (n.Contains("RING")) return Base + " > HAMMERMILL > RING";
        if (n.Contains("PIRINGAN")) return Base + " > HAMMERMILL > PIRINGAN";
        if (n.Contains("AS ") || n.Contains("SHAFT") || n.Contains("AS HAMMER")) return Base + " > HAMMERMILL > SHAFT / AS";
        if (n.Contains("PISAU")) return Base + " > HAMMERMILL > TOOLS";
        if (n.Contains("MOTOR")) return Base + " > HAMMERMILL > MOTOR";
        return Base + " > HAMMERMILL";
    }

    // destoner / cleaner
    if (n.Contains("DESTONER") || c.StartsWith("SDS")) return Base + " > DESTONER";
    if (n.Contains("CLEANER")) return Base + " > CLEANER";

    // color sorter parts
    if (n.Contains("PCB") || n.Contains("MAINBOARD")) return Base + " > COLOR SORTER > PCB / MAINBOARD";
    if (n.Contains("EJECTOR")) return Base + " > COLOR SORTER > EJECTOR";

    // general
    if (n.Contains("BEARING")) return "GENERAL > BEARING";
    if (n.Contains("V BELT") || n.Contains("V-BELT") || n.Contains("TIMMING BELT") || n.Contains("TEFLON BELT")) return "GENERAL > BELT";
    if (n.Contains("DINAMO") || n.Contains("MOTOR STATER") || n.Contains("MOTOR ")) return "GENERAL > MOTOR / DINAMO";
    if (n.Contains("BOLT") || n.Contains("BAUT")) return "GENERAL > BOLT";
    if (n.Contains("KABEL")) return "GENERAL > KABEL";
    if (n.Contains("STAVOLT")) return "GENERAL > STAVOLT";
    if (n.Contains("PNEUMATIC")) return "GENERAL > PNEUMATIC";
    if (n.Contains("POWER SUPPLY")) return "GENERAL > POWER SUPPLY";
    if (n.Contains("SELENOID") || n.Contains("SOLENOID")) return "GENERAL > SELENOID";
    if (n.Contains("AIR PRESSURE") || n.Contains("AIRPRESSURE")) return "GENERAL > PNEUMATIC";
    if (n.Contains("PLC") || n.Contains("SENSOR") || n.Contains("SPEED CONTROL") || n.Contains("OVERLOAD") || n.Contains("TRAVO") || n.Contains("TRANSFORMER") || n.Contains("METER")) return "GENERAL > POWER SUPPLY";
    if (n.Contains("PANGKON") || n.Contains("HOOK") || n.Contains("ALAT")) return "GENERAL > TOOLS";
    if (n.Contains("MILLING")) return Base + " > HAMMERMILL";
    if (n.Contains("SPRAY") || n.Contains("SPRAYER")) return Base + " > RICE POLISHER > NOZZLE / SPRAY";

    return "GENERAL > TOOLS";
}

var rows = sheet.Root.Element(ns + "sheetData").Elements(ns + "row").ToList();
var newStrings = new Dictionary<string, int>(StringComparer.Ordinal);
foreach (var s in shared) { if (!newStrings.ContainsKey(s)) newStrings[s] = newStrings.Count; }
int nextIdx = shared.Count;

int GetStringIndex(string text)
{
    if (!newStrings.TryGetValue(text, out var idx))
    {
        idx = nextIdx++;
        newStrings[text] = idx;
    }
    return idx;
}

int classified = 0, headers = 0;
foreach (var row in rows)
{
    var cells = row.Elements(ns + "c").ToList();
    var b = cells.FirstOrDefault(c => ((string)c.Attribute("r")).StartsWith("B"));
    var cName = cells.FirstOrDefault(c => ((string)c.Attribute("r")).StartsWith("C"));
    string code = CellText(b), name = CellText(cName);

    bool isHeader = !string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name);
    string group;
    if (isHeader) { group = ""; headers++; }
    else if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) group = "";
    else { group = Classify(code, name); classified++; }

    // remove existing I cell
    var existing = cells.FirstOrDefault(c => ((string)c.Attribute("r")).StartsWith("I"));
    existing?.Remove();

    int r = int.Parse((string)row.Attribute("r"));
    var cell = new XElement(ns + "c",
        new XAttribute("r", $"I{r}"),
        new XAttribute("t", "s"),
        new XElement(ns + "v", r == 3 ? GetStringIndex("Group") : GetStringIndex(group)));
    row.Add(cell);
}

// rebuild sharedStrings
var sstDoc = new XDocument(new XElement(ns + "sst",
    new XAttribute("count", newStrings.Count),
    new XAttribute("uniqueCount", newStrings.Count),
    newStrings.OrderBy(kv => kv.Value).Select(kv =>
        new XElement(ns + "si", new XElement(ns + "t",
            kv.Key.IndexOfAny(new[]{' ','\t','\n'}) >= 0 || kv.Key != kv.Key.Trim()
                ? new object[]{ new XAttribute(XNamespace.Xml + "space", "preserve"), kv.Key }
                : kv.Key)))));

if (File.Exists(dst)) File.Delete(dst);
File.Copy(src, dst);
using (var zip = ZipFile.Open(dst, ZipArchiveMode.Update))
{
    zip.GetEntry("xl/worksheets/sheet1.xml").Delete();
    var se = zip.CreateEntry("xl/worksheets/sheet1.xml");
    using (var w = new StreamWriter(se.Open(), new UTF8Encoding(false)))
        w.Write(sheet.ToString(SaveOptions.DisableFormatting));
    zip.GetEntry("xl/sharedStrings.xml").Delete();
    var se2 = zip.CreateEntry("xl/sharedStrings.xml");
    using (var w = new StreamWriter(se2.Open(), new UTF8Encoding(false)))
        w.Write(sstDoc.ToString(SaveOptions.DisableFormatting));
}

Console.WriteLine($"OK. Items classified: {classified}, division headers: {headers}");
Console.WriteLine(dst);
