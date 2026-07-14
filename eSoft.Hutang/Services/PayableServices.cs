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
        private readonly DbContextHutang _context;

        public PayableServices(DbContextHutang context)
        {
            _context = context;
        }

        public bool CekKdSupplier(string supplier)
        {
            string test = supplier.ToUpper();
            var cekFirst = _context.ApSuppls.Where(x => x.Supplier == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApSuppl> GetSupplier()
        {
            return _context.ApSuppls.OrderBy(x => x.NamaSup).ToList();
        }

        public ApSuppl GetSupplierId(int id)
        {
            return _context.ApSuppls.Where(x => x.ApSupplId == id).FirstOrDefault();
        }

        public ApSuppl GetSupplierKode(string kode)
        {
            return _context.ApSuppls.Where(x => x.Supplier == kode).FirstOrDefault();
        }

        public bool AddSupplier(SupplierView suppliers)
        {
            string test = suppliers.Supplier.ToUpper();
            var cekFirst = _context.ApSuppls.Where(x => x.Supplier == test).ToList();
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
                _context.ApSuppls.Add(Supplier);
                _context.SaveChanges();
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
                var ExistingSupplier = _context.ApSuppls.Where(x => x.ApSupplId == suppliers.ApSupplId).FirstOrDefault();
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

                    _context.ApSuppls.Update(ExistingSupplier);
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

        public async Task<bool> DelSupplier(int suppliers)
        {
            try
            {
                var ExistingSupplier = _context.ApSuppls.Where(x => x.ApSupplId == suppliers).FirstOrDefault();
                if (ExistingSupplier != null)
                {
                    _context.ApSuppls.Remove(ExistingSupplier);
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

        #region ApAcct Class

        public bool CekAcctSet(string supplier)
        {
            string test = supplier.ToUpper();
            var cekFirst = _context.ApAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApAcct> GetApAkunSet()
        {
            return _context.ApAccts.ToList();
        }

        public ApAcct GetApAkunSetId(int id)
        {
            return _context.ApAccts.Where(x => x.ApAcctId == id).FirstOrDefault();
        }

        public bool AddAkunSet(ApAcctView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
            var cekFirst = _context.ApAccts.Where(x => x.AcctSet == test).ToList();
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
                _context.ApAccts.Add(AcctCode);
                _context.SaveChanges();
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
                var ExistingAkunSet = _context.ApAccts.Where(x => x.ApAcctId == codeview.ApAcctId).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.Description = codeview.Description;
                    ExistingAkunSet.Acct1 = codeview.Acct1;
                    ExistingAkunSet.Acct2 = codeview.Acct2;
                    ExistingAkunSet.Acct3 = codeview.Acct3;
                    ExistingAkunSet.Acct4 = codeview.Acct4;
                    ExistingAkunSet.Acct5 = codeview.Acct5;
                    ExistingAkunSet.Acct6 = codeview.Acct6;

                    _context.ApAccts.Update(ExistingAkunSet);
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

        public async Task<bool> DelAkunSet(int codeview)
        {
            try
            {
                var ExistingAkunSet = _context.ApAccts.Where(x => x.ApAcctId == codeview).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    _context.ApAccts.Remove(ExistingAkunSet);
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
        #endregion ApAcct Class

        #region ApDist Class

        public bool CekDistCode(string distcode)
        {
            string test = distcode.ToUpper();
            var cekFirst = _context.ApDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<ApDist> GetDist()
        {
            return _context.ApDists.ToList();
        }

        public ApDist GetDistId(int id)
        {
            return _context.ApDists.Where(x => x.ApDistId == id).FirstOrDefault();
        }

        public bool AddDist(ApDistView codeview)
        {
            string test = codeview.DistCode.ToUpper();
            var cekFirst = _context.ApDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                ApDist AcctCode = new ApDist()
                {
                    DistCode = codeview.DistCode.ToUpper(),
                    Description = codeview.Description,
                    Dist1 = codeview.Dist1

                };
                _context.ApDists.Add(AcctCode);
                _context.SaveChanges();
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
                var ExistingDist = _context.ApDists.Where(x => x.ApDistId == codeview.ApDistId).FirstOrDefault();
                if (ExistingDist != null)
                {
                    ExistingDist.Description = codeview.Description;
                    ExistingDist.Dist1 = codeview.Dist1;

                    _context.ApDists.Update(ExistingDist);
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

        public async Task<bool> DelDist(int codeview)
        {
            try
            {
                var ExistingDist = _context.ApDists.Where(x => x.ApDistId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    _context.ApDists.Remove(ExistingDist);
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
        #endregion ApDist Class

        #region Transaksi Hutang Class

        public ApTransH GetTrans(int id)
        {
            return _context.ApTransHs.Include(p => p.ApTransDs).Where(x => x.ApTransHId == id).FirstOrDefault();
        }

        public ApHutang GetHutang(string bukti)
        {
            return _context.ApHutangs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }
        public List<ApTransH> GetTransH()
        {
            List<ApTransH> ApTrans = new List<ApTransH>();
            try
            {
                ApTrans = _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "21").ToList();
                foreach (var item in ApTrans)
                {
                    item.NamaSup = (from e in _context.ApSuppls where e.Supplier == item.Supplier select e.NamaSup).FirstOrDefault();
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
            List<ApTransH> ApTrans = new List<ApTransH>();

            ApTrans = _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "21").ToList();
            foreach (var item in ApTrans)
            {
                item.NamaSup = (from e in _context.ApSuppls where e.Supplier == item.Supplier select e.NamaSup).FirstOrDefault();
            }

            return ApTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public bool CekBeli(string noLpb)
        {
            var cekFirst = _context.ApHutangs.Where(x => x.Dokumen == noLpb && x.Bayar == 0).FirstOrDefault();

            if (cekFirst != null)
                return true;

            return false;
        }

        public List<ApTransD> GetTransD()
        {
            return _context.ApTransDs.ToList();
        }

        public ApTransH AddTransH(ApTransHView trans)
        {
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

            var Supplier = (from e in _context.ApSuppls where e.Supplier == trans.Supplier select e).FirstOrDefault();
            Supplier.Hutang += trans.Jumlah;

            _context.ApSuppls.Update(Supplier);
            _context.ApTransHs.Add(transH);
            _context.ApHutangs.Add(transaksi);
            _context.SaveChanges();
            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }

        public ApTransH EditTransH(ApTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            var cekFirst = _context.ApHutangs.Where(x => x.Dokumen == trans.Bukti).FirstOrDefault();


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


            var ExistingTrans = _context.ApTransHs.Where(x => x.ApTransHId == trans.ApTransHId).FirstOrDefault();
            if (ExistingTrans != null)
            {
                transH.Bukti = ExistingTrans.Bukti;
                transaksi.Dokumen = ExistingTrans.Bukti;

                _context.ApTransHs.Remove(ExistingTrans);

                var Supplier = (from e in _context.ApSuppls where e.Supplier == trans.Supplier select e).FirstOrDefault();

                Supplier.Hutang -= ExistingTrans.Jumlah;
                Supplier.Hutang += trans.Jumlah;

                _context.ApSuppls.Update(Supplier);
                _context.ApHutangs.Remove(cekFirst);

                _context.ApTransHs.Add(transH);
                _context.ApHutangs.Add(transaksi);
                _context.SaveChanges();

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
                var ExistingTrans = _context.ApTransHs.Where(x => x.ApTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    var cekFirst = _context.ApHutangs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                    var Supplier = (from e in _context.ApSuppls where e.Supplier == ExistingTrans.Supplier select e).FirstOrDefault();

                    Supplier.Hutang -= ExistingTrans.Jumlah;


                    _context.ApSuppls.Update(Supplier);
                    _context.ApTransHs.Remove(ExistingTrans);
                    _context.ApHutangs.Remove(cekFirst);
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
        public bool CekAlreadyPayment(string dokumen)
        {
            var cekFirst = _context.ApHutangs.Where(x => x.Dokumen == dokumen).FirstOrDefault();

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

            List<ApHutang> trans = new List<ApHutang>();

            //    trans = _context.ApHutangs.Where(x => x.Supplier == xKdHeader && (x.Sisa != 0)).ToList();
            trans = _context.ApHutangs.Where(x => x.Supplier == xKdHeader).ToList();
            return trans;
        }

        public List<ApHutangView> GetBayarDimuka()
        {
            List<ApHutangView> transView = new List<ApHutangView>();
            var trans = _context.ApHutangs.Where(x => x.Kode == "CA" && x.KodeTran == "23" && x.Sisa != 0).ToList();
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
            return _context.ApTransHs.Include(p => p.ApTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        public List<ApAgingView> GetAgingSchedule()
        {
            List<ApHutang> trans = new List<ApHutang>();
            List<ApAgingView> transaksi = new List<ApAgingView>();

            List<ApSuppl> supplier = _context.ApSuppls.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;
            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            // trans = _context.ApHutangs.Where(x => x.Kode != "CA" && (x.Sisa != 0)).OrderBy(x => x.Supplier).ToList();
            trans = _context.ApHutangs.Where(x => (x.Sisa != 0)).OrderBy(x => x.Supplier).ThenByDescending(x => x.Dokumen).ToList();
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

            List<ApSuppl> Suppliers = _context.ApSuppls.ToList();
            List<ApHutang> Hutangs = _context.ApHutangs.ToList();

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

           

            TransHutang = _context.ApTransHs.OrderBy(x => x.Tanggal).Include(x => x.ApTransDs).Where(x => x.Kode != "21").ToList();


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



            _context.UpdateRange(Suppliers);
            _context.UpdateRange(Hutangs);



           await  _context.SaveChangesAsync();


            // return Transaksi;

        }

        #endregion

        public List<ApHutang> GetAllApHutangBySupplier(string supplier, DateTime sampaiTanggal)
        {
            return _context.ApHutangs
                .Where(x => x.Supplier == supplier && x.Tanggal.Date <= sampaiTanggal.Date)
                .OrderBy(x => x.Tanggal)
                .ThenBy(x => x.Kode)
                .ToList();
        }

        public async Task<bool> UpdateApHutangWithPaymentAsync(string dokumen, decimal bayar, decimal discount)
        {
            try
            {
                var hutang = await _context.ApHutangs.FirstOrDefaultAsync(h => h.Dokumen == dokumen);
                if (hutang == null) return false;

                hutang.Bayar += bayar;
                hutang.Discount += discount;
                hutang.Sisa = Math.Max(0, hutang.Jumlah - hutang.Bayar - hutang.Discount);

                _context.ApHutangs.Update(hutang);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApTransH> CreateApPaymentTransactionAsync(
            DateTime tanggal,
            string kdBank,
            string supplier,
            string keterangan,
            List<(string dokumen, decimal bayar, decimal discount)> allocations)
        {
            try
            {
                // Generate Bukti number for payment: BKY-yy3MM-XXXXX
                string buktiNo = GetNumberPayment();

                // Get supplier info
                var supplierInfo = await _context.ApSuppls.FirstOrDefaultAsync(s => s.Supplier == supplier);

                // Create ApTransH header with Kode = "API" for payment transactions
                var transH = new ApTransH
                {
                    Bukti = buktiNo,
                    Kode = "24",  // Payment code for AP (from BMA-PT)
                    Tanggal = tanggal,
                    KdBank = kdBank,
                    Supplier = supplier,
                    Keterangan = keterangan,
                    ApSupplId = supplierInfo?.ApSupplId ?? 0,
                    NamaSup = supplierInfo?.NamaSup ?? supplier,
                    Currency = "IDR",
                    Kurs = 1,
                    ApTransDs = new List<ApTransD>()
                };

                decimal totalBayar = 0;
                decimal totalDiscount = 0;

                // Create ApTransD details for each allocation
                foreach (var alloc in allocations)
                {
                    // Get the outstanding doc info
                    var hutangItem = await _context.ApHutangs.FirstOrDefaultAsync(h => h.Dokumen == alloc.dokumen);
                    if (hutangItem == null) continue;

                    var transD = new ApTransD
                    {
                        Bukti = buktiNo,
                        Tanggal = tanggal,
                        DueDate = hutangItem.Tanggal,
                        Kode = "24",
                        KodeTran = "24",  // Payment transaction code (from BMA-PT)
                        Lpb = alloc.dokumen,
                        Jumlah = hutangItem.Sisa,
                        Bayar = alloc.bayar,
                        Discount = alloc.discount,
                        Sisa = Math.Max(0, hutangItem.Sisa - alloc.bayar - alloc.discount),
                        Keterangan = keterangan
                    };
                    transH.ApTransDs.Add(transD);

                    totalBayar += alloc.bayar;
                    totalDiscount += alloc.discount;

                    // Update ApHutang balance
                    hutangItem.Bayar += alloc.bayar;
                    hutangItem.Discount += alloc.discount;
                    hutangItem.Sisa = Math.Max(0, hutangItem.Jumlah - hutangItem.Bayar - hutangItem.Discount);
                    _context.ApHutangs.Update(hutangItem);
                }

                // Set header totals
                transH.Jumlah = totalBayar;
                transH.Discount = totalDiscount;
                transH.Hutang = totalBayar + totalDiscount;

                // Save transaction header and details
                _context.ApTransHs.Add(transH);
                await _context.SaveChangesAsync();

                return transH;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating AP payment transaction: {ex.Message}", ex);
            }
        }

        private string GetNumber()
        {
            // Generate invoice bukti number: API-yy2MM-XXXXX (for AddTransH)
            string kodeno = "API";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ApTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null && maxlist.Count > 0)
            {
                maxvalue = maxlist.Max(x => x.Bukti);
            }

            string nourut = "00000";
            if (string.IsNullOrEmpty(maxvalue))
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            return cAngNo;
        }

        private string GetNumberPayment()
        {
            // Generate payment bukti number: BKY-yy3MM-XXXXX (for bank payments)
            string kodeno = "BKY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '3' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ApTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null && maxlist.Count > 0)
            {
                maxvalue = maxlist.Max(x => x.Bukti);
            }

            string nourut = "00000";
            if (string.IsNullOrEmpty(maxvalue))
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            return cAngNo;
        }

        private int GetSupplierIdByCode(string supplierCode)
        {
            var supplier = _context.ApSuppls.FirstOrDefault(s => s.Supplier == supplierCode);
            return supplier?.ApSupplId ?? 0;
        }

        private string GetSupplierNameByCode(string supplierCode)
        {
            var supplier = _context.ApSuppls.FirstOrDefault(s => s.Supplier == supplierCode);
            return supplier?.NamaSup ?? supplierCode;
        }

    }
}
