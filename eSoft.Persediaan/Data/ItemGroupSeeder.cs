using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Data
{
    public static class ItemGroupSeeder
    {
        private sealed record Node(string Code, string Name, params Node[] Children);

        private static readonly Node[] Tree =
        {
            new Node("STK", "STOCK",
                new Node("SPR", "SPAREPART",
                    new Node("HMR", "HAMMERMILL",
                        new Node("SCR", "SCREEN"),
                        new Node("AIR", "AIRLOCK"),
                        new Node("MTR", "MOTOR"),
                        new Node("SHF", "SHAFT / AS"),
                        new Node("RIN", "RING"),
                        new Node("PIR", "PIRINGAN"),
                        new Node("TLS", "TOOLS")),
                    new Node("CLN", "CLEANER",
                        new Node("SCR", "SCREEN"),
                        new Node("TLS", "TOOLS")),
                    new Node("DST", "DESTONER",
                        new Node("SCR", "SCREEN"),
                        new Node("TLS", "TOOLS")),
                    new Node("BLW", "BLOWER",
                        new Node("MSN", "MESIN"),
                        new Node("IMP", "IMPELLER")),
                    new Node("ELV", "ELEVATOR",
                        new Node("BDY", "BODY"),
                        new Node("BKT", "BUCKET"),
                        new Node("BLT", "BELT")),
                    new Node("HSK", "PADDY HUSKER",
                        new Node("SHF", "SHAFT"),
                        new Node("BRG", "BEARING"),
                        new Node("MTR", "MOTOR"),
                        new Node("PNE", "PNEUMATIC"),
                        new Node("RBR", "RUBBER ROLL"),
                        new Node("BLT", "V-BELT"),
                        new Node("HSG", "HOUSING"),
                        new Node("SNS", "SENSOR")),
                    new Node("SEP", "PADDY SEPARATOR",
                        new Node("SCR", "SCREEN"),
                        new Node("SHF", "SHAFT"),
                        new Node("PL", "PULLY"),
                        new Node("TRN", "TRANSMISSION"),
                        new Node("VFD", "VFD")),
                    new Node("WHN", "RICE WHITENER",
                        new Node("SCR", "SCREEN"),
                        new Node("KNF", "KNIFE"),
                        new Node("SHF", "SHAFT"),
                        new Node("BRG", "BEARING"),
                        new Node("MTR", "MOTOR")),
                    new Node("SFT", "RICE SIFTER",
                        new Node("SCR", "SCREEN"),
                        new Node("SPR", "SPRING"),
                        new Node("MTR", "MOTOR"),
                        new Node("VFD", "VFD"),
                        new Node("PNL", "PANEL")),
                    new Node("POL", "RICE POLISHER",
                        new Node("NOZ", "NOZZLE / SPRAY"),
                        new Node("SCR", "SCREEN"),
                        new Node("MGH", "MAGHELIC"),
                        new Node("MTR", "MOTOR")),
                    new Node("CMP", "COMPRESSOR",
                        new Node("OIL", "OIL FILTER"),
                        new Node("AIR", "AIR FILTER"),
                        new Node("SEP", "OIL SEPARATOR"),
                        new Node("MTR", "MOTOR")),
                    new Node("PCK", "PACKING MACHINE",
                        new Node("GBX", "GEARBOX"),
                        new Node("CYL", "CYLINDER"),
                        new Node("LDC", "LOAD CELL"),
                        new Node("CTL", "CONTROLLER / DISPLAY"),
                        new Node("NDL", "NEEDLE"),
                        new Node("PNE", "PNEUMATIC")),
                    new Node("MGN", "MAGNET"),
                    new Node("CLR", "COLOR SORTER",
                        new Node("PCB", "PCB / MAINBOARD"),
                        new Node("EJC", "EJECTOR"),
                        new Node("PNL", "PANEL"))),
                new Node("MES", "MACHINE",
                    new Node("HMR", "HAMMERMILL"),
                    new Node("CLN", "CLEANER"),
                    new Node("DST", "DESTONER"),
                    new Node("CNV", "CONVEYOR"),
                    new Node("CSR", "COLOR SORTER"),
                    new Node("CMP", "COMPRESSOR"),
                    new Node("ELV", "LOW SPEED ELEVATOR"),
                    new Node("WHN", "RICE WHITENER"),
                    new Node("HSK", "PADDY HUSKER"),
                    new Node("SEP", "PADDY SEPARATOR"),
                    new Node("SFT", "RICE SIFTER"),
                    new Node("POL", "RICE POLISHER"),
                    new Node("PCK", "PACKING MACHINE")),
                new Node("LAB", "LABORATORY",
                    new Node("MOI", "MOISTURE METER"),
                    new Node("DSC", "DIGITAL SCALE"),
                    new Node("SVE", "TEST SIEVE")),
                new Node("CON", "CONSUMABLE",
                    new Node("GRS", "GREASE"),
                    new Node("OIL", "OIL"),
                    new Node("CHM", "CHEMICAL"))),
            new Node("SRV", "SERVICE",
                new Node("INS", "INSTALLATION"),
                new Node("REP", "REPAIR"),
                new Node("MNT", "MAINTENANCE"),
                new Node("TRN", "TRAINING")),
            new Node("GEN", "GENERAL",
                new Node("BLT", "BOLT"),
                new Node("NUT", "NUT"),
                new Node("CBT", "CABLE TIE"),
                new Node("TLS", "TOOLS"),
                new Node("BRG", "BEARING"),
                new Node("BLR", "BELT"),
                new Node("MTR", "MOTOR / DINAMO"),
                new Node("SCR", "SCREEN"),
                new Node("KBL", "KABEL"),
                new Node("STV", "STAVOLT"),
                new Node("PNE", "PNEUMATIC"),
                new Node("PLW", "POWER SUPPLY"),
                new Node("SLD", "SELENOID")),
            new Node("FAB", "FABRICATION",
                new Node("SCR", "SCREEN"),
                new Node("HPR", "HOPPER"),
                new Node("FRM", "FRAME"),
                new Node("TNK", "TANK"))
        };

        public static async Task SeedAsync(DbContextPersediaan db, CancellationToken cancellationToken = default)
        {
            foreach (var node in Tree)
            {
                await SeedNodeAsync(db, node, null, cancellationToken);
            }
        }

        private static async Task SeedNodeAsync(DbContextPersediaan db, Node node, int? parentId, CancellationToken cancellationToken)
        {
            var group = await db.IcItemGroups
                .FirstOrDefaultAsync(g => g.ParentId == parentId && g.Code == node.Code, cancellationToken);

            if (group is null)
            {
                group = new IcItemGroup { Code = node.Code, Name = node.Name, ParentId = parentId };
                db.IcItemGroups.Add(group);
                await db.SaveChangesAsync(cancellationToken);
            }

            foreach (var child in node.Children)
            {
                await SeedNodeAsync(db, child, group.ItemGroupId, cancellationToken);
            }
        }
    }
}
