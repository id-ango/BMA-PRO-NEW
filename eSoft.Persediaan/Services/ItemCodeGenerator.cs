using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Services
{
    public class ItemCodeGenerator : IItemCodeGenerator
    {
        private const string StockRootCode = "STK";
        private const int SequenceDigits = 4;

        private readonly DbContextPersediaan _db;

        public ItemCodeGenerator(DbContextPersediaan db)
        {
            _db = db;
        }

        public async Task<string> GenerateAsync(int itemGroupId, CancellationToken cancellationToken = default)
        {
            var chain = await GetAncestorChainAsync(itemGroupId, cancellationToken);
            var prefix = string.Join("-", chain.Select(g => g.Code.ToUpperInvariant())) + "-";

            var existingCodes = await _db.IcItems
                .AsNoTracking()
                .Where(i => i.ItemCode.StartsWith(prefix))
                .Select(i => i.ItemCode)
                .ToListAsync(cancellationToken);

            var maxSequence = existingCodes
                .Select(code => code[prefix.Length..])
                .Where(suffix => suffix.Length > 0 && suffix.All(char.IsDigit))
                .Select(suffix => int.Parse(suffix))
                .DefaultIfEmpty(0)
                .Max();

            return $"{prefix}{(maxSequence + 1).ToString($"D{SequenceDigits}")}";
        }

        private async Task<List<IcItemGroup>> GetAncestorChainAsync(int itemGroupId, CancellationToken cancellationToken)
        {
            var chain = new List<IcItemGroup>();
            int? currentId = itemGroupId;

            while (currentId.HasValue)
            {
                var group = await _db.IcItemGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.ItemGroupId == currentId.Value, cancellationToken)
                    ?? throw new InvalidOperationException($"ItemGroup {currentId.Value} tidak ditemukan.");

                if (!(group.ParentId is null && group.Code == StockRootCode))
                {
                    chain.Insert(0, group);
                }

                currentId = group.ParentId;
            }

            return chain;
        }
    }
}
