using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public class KurirMasterService : IKurirMasterService
    {
        private readonly DbContextJual _context;

        public KurirMasterService(DbContextJual context)
        {
            _context = context;
        }

        public List<OeKurir> GetKurir()
        {
            return _context.OeKurirs.ToList();
        }

        public OeKurir GetKurirId(int id)
        {
            return _context.OeKurirs.Where(x => x.OeKurirId == id).FirstOrDefault();
        }

        public string GetKurirKode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return _context.OeKurirs.Where(x => x.Kurir == id).FirstOrDefault().NamaKurir;
        }

        public bool CekKdKurir(string kurir)
        {
            string test = kurir.ToUpper();
            var cekFirst = _context.OeKurirs.Where(x => x.Kurir == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool AddKurir(OeKurirView kurir)
        {
            string test = kurir.Kurir.ToUpper();
            var cekFirst = _context.OeKurirs.Where(x => x.Kurir == test).ToList();
            if (cekFirst.Count == 0)
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
                _context.OeKurirs.Add(entity);
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<bool> EditKurir(OeKurirView kurir)
        {
            try
            {
                var existingKurir = _context.OeKurirs.Where(x => x.OeKurirId == kurir.OeKurirId).FirstOrDefault();
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

                    _context.OeKurirs.Update(existingKurir);
                    await _context.SaveChangesAsync();
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
                var existingKurir = _context.OeKurirs.Where(x => x.OeKurirId == id).FirstOrDefault();
                if (existingKurir != null)
                {
                    _context.OeKurirs.Remove(existingKurir);
                    await _context.SaveChangesAsync();
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
