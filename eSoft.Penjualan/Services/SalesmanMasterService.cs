using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesmanMasterService : ISalesmanMasterService
    {
        private readonly IDbContextFactory<DbContextJual> _context;

        public SalesmanMasterService(IDbContextFactory<DbContextJual> context)
        {
            _context = context;
        }

        public List<OeSalesman> GetSalesman()
        {
            using var context = _context.CreateDbContext();
            return context.OeSalesmans
                .AsNoTracking()
                .ToList();
        }

        public OeSalesman GetSalesmanId(int id)
        {
            using var context = _context.CreateDbContext();
            return context.OeSalesmans
                .AsNoTracking()
                .FirstOrDefault(x => x.OeSalesmanId == id);
        }

        public string GetSalesmanKode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            using var context = _context.CreateDbContext();
            return context.OeSalesmans
                .AsNoTracking()
                .Where(x => x.Salesman == id)
                .Select(x => x.NamaSales)
                .FirstOrDefault() ?? string.Empty;
        }

        public async Task<bool> DelSalesman(int id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var existingSalesman = context.OeSalesmans.FirstOrDefault(x => x.OeSalesmanId == id);
                if (existingSalesman != null)
                {
                    context.OeSalesmans.Remove(existingSalesman);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public bool CekKdSalesman(string salesman)
        {
            string test = salesman.ToUpper();
            using var context = _context.CreateDbContext();
            return context.OeSalesmans.Any(x => x.Salesman == test);
        }

        public bool AddSalesman(OeSalesmanView salesman)
        {
            using var context = _context.CreateDbContext();
            string test = salesman.Salesman.ToUpper();
            var exists = context.OeSalesmans.Any(x => x.Salesman == test);
            if (!exists)
            {
                OeSalesman entity = new()
                {
                    Salesman = salesman.Salesman.ToUpper(),
                    NamaSales = salesman.NamaSales,
                    Termin = salesman.Termin,
                    Alamat = salesman.Alamat,
                    Kota = salesman.Kota,
                    Telpon = salesman.Telpon,
                    NamaLengkap = salesman.NamaLengkap,
                    AcctSet = salesman.AcctSet,
                    AlmtKrm = salesman.AlmtKrm,
                    KotaKrm = salesman.KotaKrm,
                    NPWP_Sales = salesman.NPWP_Sales,
                    Kontak = salesman.Kontak
                };
                context.OeSalesmans.Add(entity);
                context.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<bool> EditSalesman(OeSalesmanView salesman)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var existingSalesman = context.OeSalesmans.FirstOrDefault(x => x.OeSalesmanId == salesman.OeSalesmanId);
                if (existingSalesman != null)
                {
                    existingSalesman.NamaSales = salesman.NamaSales;
                    existingSalesman.Alamat = salesman.Alamat;
                    existingSalesman.Kota = salesman.Kota;
                    existingSalesman.Telpon = salesman.Telpon;
                    existingSalesman.Termin = salesman.Termin;
                    existingSalesman.NamaLengkap = salesman.NamaLengkap;
                    existingSalesman.AcctSet = salesman.AcctSet;
                    existingSalesman.AcctPjk = salesman.AcctPjk;
                    existingSalesman.AlmtKrm = salesman.AlmtKrm;
                    existingSalesman.KotaKrm = salesman.KotaKrm;
                    existingSalesman.ProvKirim = salesman.ProvKirim;
                    existingSalesman.Kontak = salesman.Kontak;
                    existingSalesman.NPWP_Sales = salesman.NPWP_Sales;

                    context.OeSalesmans.Update(existingSalesman);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
    }
}
