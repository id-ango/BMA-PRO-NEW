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
    public class KurirMasterService : IKurirMasterService
    {
        private readonly IDbContextFactory<DbContextJual> _context;

        public KurirMasterService(IDbContextFactory<DbContextJual> context)
        {
            _context = context;
        }

        public List<OeKurir> GetKurir()
        {
            using var context = _context.CreateDbContext();
            return context.OeKurirs
                .AsNoTracking()
                .ToList();
        }

        public OeKurir GetKurirId(int id)
        {
            using var context = _context.CreateDbContext();
            return context.OeKurirs
                .AsNoTracking()
                .FirstOrDefault(x => x.OeKurirId == id);
        }

        public string GetKurirKode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            using var context = _context.CreateDbContext();
            return context.OeKurirs
                .AsNoTracking()
                .Where(x => x.Kurir == id)
                .Select(x => x.NamaKurir)
                .FirstOrDefault() ?? string.Empty;
        }

        public bool CekKdKurir(string kurir)
        {
            string test = kurir.ToUpper();
            using var context = _context.CreateDbContext();
            return context.OeKurirs.Any(x => x.Kurir == test);
        }

        public bool AddKurir(OeKurirView kurir)
        {
            using var context = _context.CreateDbContext();
            string test = kurir.Kurir.ToUpper();
            var exists = context.OeKurirs.Any(x => x.Kurir == test);
            if (!exists)
            {
                OeKurir entity = new()
                {
                    Kurir = kurir.Kurir.ToUpper(),
                    NamaKurir = kurir.NamaKurir,
                    Termin = kurir.Termin,
                    Alamat = kurir.Alamat,
                    Kota = kurir.Kota,
                    Telpon = kurir.Telpon,
                    NamaLengkap = kurir.NamaLengkap,
                    AcctSet = kurir.AcctSet,
                    AlmtKrm = kurir.AlmtKrm,
                    KotaKrm = kurir.KotaKrm,
                    NPWP_Kurir = kurir.NPWP_Kurir,
                    Kontak = kurir.Kontak
                };
                context.OeKurirs.Add(entity);
                context.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<bool> EditKurir(OeKurirView kurir)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var existingKurir = context.OeKurirs.FirstOrDefault(x => x.OeKurirId == kurir.OeKurirId);
                if (existingKurir != null)
                {
                    existingKurir.NamaKurir = kurir.NamaKurir;
                    existingKurir.Alamat = kurir.Alamat;
                    existingKurir.Kota = kurir.Kota;
                    existingKurir.Telpon = kurir.Telpon;
                    existingKurir.Termin = kurir.Termin;
                    existingKurir.NamaLengkap = kurir.NamaLengkap;
                    existingKurir.AcctSet = kurir.AcctSet;
                    existingKurir.AcctPjk = kurir.AcctPjk;
                    existingKurir.AlmtKrm = kurir.AlmtKrm;
                    existingKurir.KotaKrm = kurir.KotaKrm;
                    existingKurir.ProvKirim = kurir.ProvKirim;
                    existingKurir.Kontak = kurir.Kontak;
                    existingKurir.NPWP_Kurir = kurir.NPWP_Kurir;

                    context.OeKurirs.Update(existingKurir);
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

        public async Task<bool> DelKurir(int id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var existingKurir = context.OeKurirs.FirstOrDefault(x => x.OeKurirId == id);
                if (existingKurir != null)
                {
                    context.OeKurirs.Remove(existingKurir);
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
