using System;
using System.Linq;
using System.Transactions;
using eSoft.Penjualan.Data;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesDocumentNumberService : ISalesDocumentNumberService
    {
        private readonly IDbContextFactory<DbContextJual> _context;
        private static readonly object NumberGenerationLock = new();

        public SalesDocumentNumberService(IDbContextFactory<DbContextJual> context)
        {
            _context = context;
        }

        public string GetNumber() => GenerateNumber("SLS");

        public string GetNumberTax() => GenerateNumber("PJL");

        public string GetNumberRetur() => GenerateNumber("R/J");

        public string GetNumberTaxRetur() => GenerateNumber("RTJ");

        private string GenerateNumber(string kodeNo)
        {
            lock (NumberGenerationLock)
            {
                return GenerateNumberCore(kodeNo);
            }
        }

        private string GenerateNumberCore(string kodeNo)
        {
            using var context = _context.CreateDbContext();

            string kodeUrut = kodeNo + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xBukti = kodeUrut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            string maxValue = context.OeTransHs
                .Where(x => x.NoLpb.Substring(0, 10).Equals(xBukti))
                .OrderByDescending(x => x.NoLpb)
                .Select(x => x.NoLpb)
                .FirstOrDefault();

            string noUrut = string.IsNullOrEmpty(maxValue) ? "00000" : maxValue.Substring(10, 5);
            return xBukti + (int.Parse(noUrut) + 1).ToString("00000");
        }
    }
}
