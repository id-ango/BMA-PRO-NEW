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
                        new Node("HAM", "HAMMER"),
                        new Node("BRG", "BEARING"),
                        new Node("BLT", "BELT"),
                        new Node("MTR", "MOTOR")),
                    new Node("CLN", "CLEANER",
                        new Node("SCR", "SCREEN"),
                        new Node("BRG", "BEARING")),
                    new Node("DST", "DESTONER",
                        new Node("SCR", "SCREEN")),
                    new Node("ELV", "ELEVATOR",
                        new Node("BKT", "BUCKET"),
                        new Node("BLT", "BELT"),
                        new Node("BRG", "BEARING"))),
                new Node("MES", "MACHINE",
                    new Node("HMR", "HAMMERMILL"),
                    new Node("CLN", "CLEANER"),
                    new Node("DST", "DESTONER"),
                    new Node("CNV", "CONVEYOR"),
                    new Node("CSR", "COLOR SORTER")),
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
                new Node("TLS", "TOOLS")),
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
            var group = await db.ItemGroups
                .FirstOrDefaultAsync(g => g.ParentId == parentId && g.Code == node.Code, cancellationToken);

            if (group is null)
            {
                group = new ItemGroup { Code = node.Code, Name = node.Name, ParentId = parentId };
                db.ItemGroups.Add(group);
                await db.SaveChangesAsync(cancellationToken);
            }

            foreach (var child in node.Children)
            {
                await SeedNodeAsync(db, child, group.ItemGroupId, cancellationToken);
            }
        }
    }
}
