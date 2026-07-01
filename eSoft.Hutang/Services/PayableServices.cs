using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Hutang.View;

using Microsoft.EntityFrameworkCore;

namespace eSoft.Hutang.Services
{
    public class PayableServices : IPayableServices
    {
        private readonly IDbContextFactory<DbContextHutang> _context;

        public PayableServices(IDbContextFactory<DbContextHutang> context)
        {
            _context = context;
        }

        public bool CekKdSupplier(string supplier)
        {
            using var context = _context.CreateDbContext();
            string test = supplier.ToUpper();
            var cekFirst = context.ApSuppls.Where(x => x.Supplier == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApSuppl> GetSupplier()
        {
            using var context = _context.CreateDbContext();
            return context.ApSuppls.OrderBy(x => x.NamaSup).ToList();
        }

        public ApSuppl GetSupplierId(int id)
        {
            using var context = _context.CreateDbContext();
            return context.ApSuppls.FirstOrDefault(x => x.ApSupplId == id);
        }

        public ApSuppl GetSupplierKode(string kode)
        {
            using var context = _context.CreateDbContext();
            return context.ApSuppls.FirstOrDefault(x => x.Supplier == kode);
        }

        public bool AddSupplier(SupplierView suppliers)
        {
            using var context = _context.CreateDbContext();
            string test = suppliers.Supplier.ToUpper();
            var cekFirst = context.ApSuppls.Where(x => x.Supplier == test).ToList();
            if (cekFirst.Count == 0)
            {
                ApSuppl Supplier = new ApSuppl()
                {
                    Supplier = suppliers.Supplier.ToUpper(),
                    NamaSup = suppliers.NamaSup,
                    Termin = suppliers.Termin,
                    Alamat = suppliers.Alamat,
                    Kota = suppliers.Kota,
                    Telpon = suppliers.Telpon,
                    Kontak = suppliers.Kontak,
                    NamaLengkap = suppliers.NamaLengkap,
                    Kurs = suppliers.Kurs,
                    Pajak = suppliers.Pajak,
                    AcctSet = suppliers.AcctSet

                };
                context.ApSuppls.Add(Supplier);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditSupplier(SupplierView suppliers)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingSupplier = context.ApSuppls.FirstOrDefault(x => x.ApSupplId == suppliers.ApSupplId);
                if (ExistingSupplier != null)
                {
                    ExistingSupplier.NamaSup = suppliers.NamaSup;
                    ExistingSupplier.Termin = suppliers.Termin;
                    ExistingSupplier.Alamat = suppliers.Alamat;
                    ExistingSupplier.Kota = suppliers.Kota;
                    ExistingSupplier.Telpon = suppliers.Telpon;
                    ExistingSupplier.Kontak = suppliers.Kontak;
                    ExistingSupplier.NamaLengkap = suppliers.NamaLengkap;
                    ExistingSupplier.Kurs = suppliers.Kurs;
                    ExistingSupplier.AcctSet = suppliers.AcctSet;
                    ExistingSupplier.Pajak = suppliers.Pajak;

                    context.ApSuppls.Update(ExistingSupplier);
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

        public async Task<bool> DelSupplier(int suppliers)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingSupplier = context.ApSuppls.FirstOrDefault(x => x.ApSupplId == suppliers);
                if (ExistingSupplier != null)
                {
                    context.ApSuppls.Remove(ExistingSupplier);
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

        #region ApAcct Class

        public bool CekAcctSet(string supplier)
        {
            using var context = _context.CreateDbContext();
            string test = supplier.ToUpper();
            var cekFirst = context.ApAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApAcct> GetApAkunSet()
        {
            using var context = _context.CreateDbContext();
            return context.ApAccts.ToList();
        }

        public ApAcct GetApAkunSetId(int id)
        {
            using var context = _context.CreateDbContext();
            return context.ApAccts.FirstOrDefault(x => x.ApAcctId == id);
        }

        public bool AddAkunSet(ApAcctView codeview)
        {
            using var context = _context.CreateDbContext();
            string test = codeview.AcctSet.ToUpper();
            var cekFirst = context.ApAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                ApAcct AcctCode = new ApAcct()
                {
                    AcctSet = codeview.AcctSet.ToUpper(),
                    Description = codeview.Description,
                    Acct1 = codeview.Acct1,
                    Acct2 = codeview.Acct2,
                    Acct3 = codeview.Acct3,
                    Acct4 = codeview.Acct4,
                    Acct5 = codeview.Acct5,
                    Acct6 = codeview.Acct6

                };
                context.ApAccts.Add(AcctCode);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditAkunSet(ApAcctView codeview)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingAkunSet = context.ApAccts.FirstOrDefault(x => x.ApAcctId == codeview.ApAcctId);
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.Description = codeview.Description;
                    ExistingAkunSet.Acct1 = codeview.Acct1;
                    ExistingAkunSet.Acct2 = codeview.Acct2;
                    ExistingAkunSet.Acct3 = codeview.Acct3;
                    ExistingAkunSet.Acct4 = codeview.Acct4;
                    ExistingAkunSet.Acct5 = codeview.Acct5;
                    ExistingAkunSet.Acct6 = codeview.Acct6;

                    context.ApAccts.Update(ExistingAkunSet);
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

        public async Task<bool> DelAkunSet(int codeview)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingAkunSet = context.ApAccts.FirstOrDefault(x => x.ApAcctId == codeview);
                if (ExistingAkunSet != null)
                {
                    context.ApAccts.Remove(ExistingAkunSet);
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
        #endregion ApAcct Class

        #region ApDist Class

        public bool CekDistCode(string distcode)
        {
            using var context = _context.CreateDbContext();
            string test = distcode.ToUpper();
            var cekFirst = context.ApDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApDist> GetDist()
        {
            using var context = _context.CreateDbContext();
            return context.ApDists.ToList();
        }

        public ApDist GetDistId(int id)
        {
            using var context = _context.CreateDbContext();
            return context.ApDists.FirstOrDefault(x => x.ApDistId == id);
        }

        public bool AddDist(ApDistView codeview)
        {
            using var context = _context.CreateDbContext();
            string test = codeview.DistCode.ToUpper();
            var cekFirst = context.ApDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                ApDist AcctCode = new ApDist()
                {
                    DistCode = codeview.DistCode.ToUpper(),
                    Description = codeview.Description,
                    Dist1 = codeview.Dist1

                };
                context.ApDists.Add(AcctCode);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditDist(ApDistView codeview)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingDist = context.ApDists.FirstOrDefault(x => x.ApDistId == codeview.ApDistId);
                if (ExistingDist != null)
                {
                    ExistingDist.Description = codeview.Description;
                    ExistingDist.Dist1 = codeview.Dist1;

                    context.ApDists.Update(ExistingDist);
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

        public async Task<bool> DelDist(int codeview)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingDist = context.ApDists.FirstOrDefault(x => x.ApDistId == codeview);
                if (ExistingDist != null)
                {
                    context.ApDists.Remove(ExistingDist);
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
        #endregion ApDist Class

        #region Transaksi Hutang Class

        public ApTransH GetTrans(int id)
        {
            using var context = _context.CreateDbContext();
            return context.ApTransHs.Include(p => p.ApTransDs).FirstOrDefault(x => x.ApTransHId == id);
        }

        public ApHutang GetHutang(string bukti)
        {
            using var context = _context.CreateDbContext();
            return context.ApHutangs.FirstOrDefault(x => x.Dokumen == bukti);

        }
        public List<ApTransH> GetTransH()
        {
            using var context = _context.CreateDbContext();
            List<ApTransH> ApTrans = new List<ApTransH>();
            try
            {
                ApTrans = context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "21").ToList();
                foreach (var item in ApTrans)
                {
                    item.NamaSup = context.ApSuppls.FirstOrDefault(e => e.Supplier == item.Supplier)?.NamaSup;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ApTrans;
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.ToListAsync();

        }

        public List<ApTransH> Get3TransH()
        {
            using var context = _context.CreateDbContext();
            List<ApTransH> ApTrans = new List<ApTransH>();

            ApTrans = context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "21").ToList();
            foreach (var item in ApTrans)
            {
                item.NamaSup = context.ApSuppls.FirstOrDefault(e => e.Supplier == item.Supplier)?.NamaSup;
            }

            return ApTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public bool CekBeli(string noLpb)
        {
            using var context = _context.CreateDbContext();
            var cekFirst = context.ApHutangs.FirstOrDefault(x => x.Dokumen == noLpb && x.Bayar == 0);

            if (cekFirst != null)
                return true;

            return false;
        }

        public List<ApTransD> GetTransD()
        {
            using var context = _context.CreateDbContext();
            return context.ApTransDs.ToList();
        }

        public ApTransH AddTransH(ApTransHView trans)
        {
            using var context = _context.CreateDbContext();
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            ApTransH transH = new ApTransH
            {
                Bukti = GetNumber(),
                Supplier = trans.Supplier.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                Currency = trans.Currency,
                Kurs = trans.Kurs,
                Nilai = trans.JumNilai,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = trans.Jumlah,
                Netto = 0,
                Discount = 0,
                Hutang = 0,
                Pajak = false,
                Unapplied = 0,
                Kode = "21",
                ApSupplId = trans.ApSupplId,

                ApTransDs = new List<ApTransD>()
            };
            foreach (var item in trans.ApTransDs)
            {
                transH.ApTransDs.Add(new ApTransD()
                {
                    DistCode = item.DistCode,
                    Keterangan = item.Keterangan,
                    Jumlah = item.Jumlah,
                    Kode = "21",
                    KodeTran = "21",
                    Bukti = transH.Bukti,
                    Sisa = item.Jumlah,
                    Discount = 0,
                    Bayar = 0,
                    Tanggal = trans.Tanggal
                });
            }
            ApHutang transaksi = new ApHutang
            {
                Kode = "IN",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Supplier = transH.Supplier,
                Keterangan = transH.Keterangan,
                KodeTran = "21",
                Jumlah = transH.Jumlah,
                Bayar = 0,
                Discount = 0,
                UnApplied = 0,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                Dpp = transH.Jumlah,
                Kurs = transH.Kurs,
                Currency = trans.Currency,
                Nilai = transH.Nilai,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };

            var Supplier = context.ApSuppls.FirstOrDefault(e => e.Supplier == trans.Supplier);
            Supplier.Hutang += trans.Jumlah;

            context.ApSuppls.Update(Supplier);
            context.ApTransHs.Add(transH);
            context.ApHutangs.Add(transaksi);
            context.SaveChanges();
            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }

        public ApTransH EditTransH(ApTransHView trans)
        {
            using var context = _context.CreateDbContext();
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            var cekFirst = context.ApHutangs.FirstOrDefault(x => x.Dokumen == trans.Bukti);


            ApTransH transH = new ApTransH
            {

                Supplier = trans.Supplier.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                Currency = trans.Currency,
                Kurs = trans.Kurs,
                Nilai = trans.JumNilai,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = trans.Jumlah,
                Netto = 0,
                Discount = 0,
                Hutang = 0,
                Pajak = false,
                Unapplied = 0,
                Kode = "21",
                ApSupplId = trans.ApSupplId,

                ApTransDs = new List<ApTransD>()
            };
            foreach (var item in trans.ApTransDs)
            {
                transH.ApTransDs.Add(new ApTransD()
                {
                    DistCode = item.DistCode,
                    Keterangan = item.Keterangan,
                    Jumlah = item.Jumlah,
                    Kode = "21",
                    KodeTran = "21",
                    Lpb = transH.Bukti,
                    Sisa = item.Jumlah,
                    Discount = 0,
                    Bayar = 0,
                    Tanggal = trans.Tanggal
                });
            }

            ApHutang transaksi = new ApHutang
            {
                Kode = "IN",
                Tanggal = transH.Tanggal,
                Supplier = transH.Supplier,
                Keterangan = transH.Keterangan,
                KodeTran = "21",
                Jumlah = transH.Jumlah,
                Bayar = 0,
                Discount = 0,
                UnApplied = 0,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                Dpp = transH.Jumlah,
                Kurs = transH.Kurs,
                Currency = trans.Currency,
                Nilai = transH.Nilai,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };


            var ExistingTrans = context.ApTransHs.FirstOrDefault(x => x.ApTransHId == trans.ApTransHId);
            if (ExistingTrans != null)
            {
                transH.Bukti = ExistingTrans.Bukti;
                transaksi.Dokumen = ExistingTrans.Bukti;

                context.ApTransHs.Remove(ExistingTrans);

                var Supplier = context.ApSuppls.FirstOrDefault(e => e.Supplier == trans.Supplier);

                Supplier.Hutang -= ExistingTrans.Jumlah;
                Supplier.Hutang += trans.Jumlah;

                context.ApSuppls.Update(Supplier);
                context.ApHutangs.Remove(cekFirst);

                context.ApTransHs.Add(transH);
                context.ApHutangs.Add(transaksi);
                context.SaveChanges();

                var TempTrans = GetTransDoc(transH.Bukti);

                return TempTrans;
            }
            else
            {
                return ExistingTrans;
            }



        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var ExistingTrans = context.ApTransHs.FirstOrDefault(x => x.ApTransHId == id);
                if (ExistingTrans != null)
                {
                    var cekFirst = context.ApHutangs.FirstOrDefault(x => x.Dokumen == ExistingTrans.Bukti);
                    var Supplier = context.ApSuppls.FirstOrDefault(e => e.Supplier == ExistingTrans.Supplier);

                    Supplier.Hutang -= ExistingTrans.Jumlah;


                    context.ApSuppls.Update(Supplier);
                    context.ApTransHs.Remove(ExistingTrans);
                    context.ApHutangs.Remove(cekFirst);
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
        public bool CekAlreadyPayment(string dokumen)
        {
            using var context = _context.CreateDbContext();
            var cekFirst = context.ApHutangs.FirstOrDefault(x => x.Dokumen == dokumen);

            if (cekFirst.SldSisa != cekFirst.Sisa)
            {
                return true;
            }
            return false;
        }

        #endregion Transaksi Hutang Class

        #region laporan Hutang

        public List<ApHutang> Detail1(string xKdHeader)
        {
            using var context = _context.CreateDbContext();
            List<ApHutang> trans = new List<ApHutang>();

            //    trans = _context.ApHutangs.Where(x => x.Supplier == xKdHeader && (x.Sisa != 0)).ToList();
            trans = context.ApHutangs.Where(x => x.Supplier == xKdHeader).ToList();
            return trans;
        }

        public List<ApHutangView> GetBayarDimuka()
        {
            using var context = _context.CreateDbContext();
            List<ApHutangView> transView = new List<ApHutangView>();
            var trans = context.ApHutangs.Where(x => x.Kode == "CA" && x.KodeTran == "23" && x.Sisa != 0).ToList();
            var supplier = GetSupplier();


            if (trans != null && supplier != null)
            {
                transView = (from header in trans
                             join detail in supplier on header.Supplier equals detail.Supplier
                             select new ApHutangView()
                             {
                                 Sisa = header.Sisa,
                                 Dokumen = header.Dokumen,
                                 Tanggal = header.Tanggal,
                                 Supplier = header.Supplier,
                                 NamaSuppl = detail.NamaSup,
                                 KdBank = GetTransDoc(header.Dokumen).KdBank
                             }).ToList();
            }

            return transView;
        }
        #endregion

        public ApTransH GetTransDoc(string docno)
        {
            using var context = _context.CreateDbContext();
            return context.ApTransHs.Include(p => p.ApTransDs).FirstOrDefault(x => x.Bukti == docno);
        }

        public string GetNumber()
        {
            using var context = _context.CreateDbContext();
            string kodeno = "API";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = context.ApTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.Bukti);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }

        public List<ApAgingView> GetAgingSchedule()
        {
            using var context = _context.CreateDbContext();
            List<ApHutang> trans = new List<ApHutang>();
            List<ApAgingView> transaksi = new List<ApAgingView>();

            List<ApSuppl> supplier = context.ApSuppls.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;
            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            // trans = _context.ApHutangs.Where(x => x.Kode != "CA" && (x.Sisa != 0)).OrderBy(x => x.Supplier).ToList();
            trans = context.ApHutangs.Where(x => (x.Sisa != 0)).OrderBy(x => x.Supplier).ThenByDescending(x => x.Dokumen).ToList();
            foreach (var ap in trans)
            {
                duedate = ap.DueDate ?? ap.Tanggal;
                date1 = duedate.AddMonths(1);
                date2 = duedate.AddMonths(2);
                date3 = duedate.AddMonths(3);


                transaksi.Add(new ApAgingView()
                {
                    Kode = ap.Kode,
                    ApAgingId = ap.ApHutangId,
                    Supplier = ap.Supplier,
                    Tanggal = ap.Tanggal,
                    Dokumen = ap.Dokumen,
                    Kurs = ap.Kurs,
                    Duedate = duedate,
                    Cicilan = (ap.Sisa != ap.SldSisa ? true : false),
                    NamaSup = (from e in supplier where e.Supplier == ap.Supplier select e.NamaSup).FirstOrDefault(),
                    Keterangan = ap.Keterangan,
                    
                    Sisa = ap.Sisa,
                    Jumlah = (currentDate < duedate ? ap.Sisa : 0),
                    Jumlah1 = (currentDate >= duedate && currentDate <= date1 ? ap.Sisa : 0),
                    Jumlah2 = (currentDate > date1 && currentDate <= date2 ? ap.Sisa : 0),
                    Jumlah3 = (currentDate > date2 && currentDate <= date3 ? ap.Sisa : 0),
                    Jumlah4 = (currentDate > date3 ? ap.Sisa : 0),
                });
            }
            return transaksi;

        }

        #region proseshutang

        public async Task  ProsesHutang()
        {
            using var context = _context.CreateDbContext();

            List<ApSuppl> Suppliers = context.ApSuppls.ToList();
            List<ApHutang> Hutangs = context.ApHutangs.ToList();

            List<ApTransH> TransHutang = new List<ApTransH>();


            Suppliers.ForEach(i => { i.Hutang = 0; });


            Suppliers.ForEach(i => { i.Hutang = i.SldAwal; });
            foreach (var hutang in Hutangs)
            {
                if (hutang.Kode == "IR" || hutang.Kode == "IN")
                {
                    Suppliers.Find(x => x.Supplier == hutang.Supplier).Hutang += hutang.Jumlah;
                }
            }

            Hutangs.ForEach(i => { i.Bayar = i.SldBayar; i.Discount = i.SldDisc; i.Sisa = i.SldSisa; });

            foreach (var hutang in Hutangs)
            {

                Suppliers.Find(x => x.Supplier == hutang.Supplier).Hutang -= (hutang.KodeTran == "23" ? -1 * hutang.SldSisa : hutang.SldBayar);

            }

           

            TransHutang = context.ApTransHs.OrderBy(x => x.Tanggal).Include(x => x.ApTransDs).Where(x => x.Kode != "21").ToList();


            foreach (var trans in TransHutang)
            {

                decimal mPayee = 0;
                decimal mDiskon = 0;

                var transdetail = trans.ApTransDs;

                if (transdetail != null)
                {
                    foreach (var transdetails in transdetail)
                    {
                        // if (transdetails.KodeTran != "14")
                        // {
                        Hutangs.Find(x => x.Dokumen == transdetails.Lpb).Bayar += (transdetails.Bayar + transdetails.Discount);
                        Hutangs.Find(x => x.Dokumen == transdetails.Lpb).Discount += transdetails.Discount;
                        // }


                        Hutangs.Find(x => x.Dokumen == transdetails.Lpb).Sisa -= (transdetails.Bayar + transdetails.Discount);
                        mPayee += transdetails.Bayar;
                        mDiskon += transdetails.Discount;

                    }
                }

                trans.Unapplied = trans.Jumlah - mPayee;
                trans.Discount = mDiskon;

                Hutangs.Find(x => x.Dokumen == trans.Bukti).Jumlah = (trans.Kode == "13" ? -1 * trans.Jumlah : -1 * trans.Hutang);

                Hutangs.Find(x => x.Dokumen == trans.Bukti).UnApplied = -1 * trans.Unapplied;

                if (trans.Kode != "23")
                {
                    Hutangs.Find(x => x.Dokumen == trans.Bukti).Bayar = -1 * trans.Jumlah;
                    Hutangs.Find(x => x.Dokumen == trans.Bukti).Sisa = -1 * trans.Unapplied;
                    Hutangs.Find(x => x.Dokumen == trans.Bukti).Discount = -1 * trans.Discount;
                    Suppliers.Find(x => x.Supplier == trans.Supplier).Hutang -= (trans.Kode == "23" ? -1 * (trans.Jumlah) : (trans.Hutang + trans.Unapplied));
                }




            }



            context.UpdateRange(Suppliers);
            context.UpdateRange(Hutangs);



           await context.SaveChangesAsync();


            // return Transaksi;

        }

        #endregion

        public List<ApHutang> GetAllApHutangBySupplier(string supplier, DateTime sampaiTanggal)
        {
            using var context = _context.CreateDbContext();
            return context.ApHutangs
                .Where(x => x.Supplier == supplier && x.Tanggal.Date <= sampaiTanggal.Date)
                .OrderBy(x => x.Tanggal)
                .ThenBy(x => x.Kode)
                .ToList();
        }

    }
}
