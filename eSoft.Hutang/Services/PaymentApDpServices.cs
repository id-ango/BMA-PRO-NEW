using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Hutang.View;
using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Hutang.Services
{
    public class PaymentApDpServices : IPaymentApDpServices
    {
        private readonly IDbContextFactory<DbContextHutang> _context;
        private readonly IDbContextFactory<DbContextBank> _contextBank;

        public PaymentApDpServices(IDbContextFactory<DbContextHutang> context, IDbContextFactory<DbContextBank> contextBank)
        {
            _context = context;
            _contextBank = contextBank;
        }

        #region Transaksi Hutang Pembayaran Class

        public ApTransH GetTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.ApTransHs.Include(p => p.ApTransDs).Where(x => x.ApTransHId == id).FirstOrDefault();
        }

        public bool GetHutangSisa(string xDocNo)
        {
            using var db = _context.CreateDbContext();
            var sisa = db.ApHutangs.Where(x => x.Dokumen == xDocNo).FirstOrDefault();
            if (sisa.Jumlah == sisa.Sisa)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        public List<ApTransH> GetTransH()
        {
            using var db = _context.CreateDbContext();
            List<ApTransH> ApTrans = new List<ApTransH>();
            try
            {
                ApTrans = db.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "23").ToList();
                foreach (var item in ApTrans)
                {
                    item.NamaSup = (from e in db.ApSuppls where e.Supplier == item.Supplier select e.NamaSup).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ApTrans;
        }

        public List<ApTransH> Get3TransH()
        {
            using var db = _context.CreateDbContext();
            List<ApTransH> ApTrans = new List<ApTransH>();

            ApTrans = db.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "23").ToList();
            foreach (var item in ApTrans)
            {
                item.NamaSup = (from e in db.ApSuppls where e.Supplier == item.Supplier select e.NamaSup).FirstOrDefault();
            }

            return ApTrans;
        }

        public List<ApTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.ApTransDs.Where(x => x.Kode == "23").ToList();
        }

        public ApTransH AddTransH(ApTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbBank = _contextBank.CreateDbContext();

            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            string KdSrc = "AP";

            ApTransH transH = new ApTransH
            {
                Bukti = GetNumber(),
                Supplier = trans.Supplier.ToUpper(),
                Tanggal = trans.Tanggal,
                Currency = trans.Currency,
                Keterangan = trans.Keterangan,
                Kurs = trans.Kurs,
                Nilai = trans.Nilai,
                Jumlah = trans.JumBayar,
                Discount = 0,
                Unapplied = trans.UpdateUnapplied,
                Hutang = 0,
                KdBank = trans.KdBank,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = 0,
                Netto = 0,
                Pajak = false,
                Kode = "23",
                ApSupplId = trans.ApSupplId

                //  ApTransDs = new List<ApTransD>()
            };

            #region detailTrans

            //List<ApHutang> transaksi = new List<ApHutang>();
            //transaksi = _context.ApHutangs.Where(x => x.supplier == trans.supplier && x.Sisa != 0).ToList();

            //foreach (var item in trans.ApTransDs)
            //{
            //    transH.ApTransDs.Add(new ApTransD()
            //    {
            //        Jumlah = item.Jumlah,
            //        Kode = "14",
            //        KodeTran = item.KodeTran,
            //        Lpb = transH.Bukti,
            //        Tanggal = trans.Tanggal,
            //        Discount = item.Discount,
            //        Bayar = item.Bayar,
            //        Sisa = item.UpdateSisa

            //    });

            //    transaksi.Where(x => x.Dokumen == item.Lpb).ToList()
            //        .ForEach(s =>
            //        {
            //            s.Bayar = item.Bayar + item.Discount;
            //            s.Discount = item.Discount;
            //            s.Sisa = item.UpdateSisa;
            //        });

            //}
            //transH.ApTransDs.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);
            //transaksi.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);

            #endregion

            ApHutang transaksi = new ApHutang
            {
                Kode = "CA",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Supplier = transH.Supplier,               
                Keterangan = transH.Keterangan,
                KodeTran = "23",
                Jumlah = -1 * transH.Jumlah,
                SldSisa = -1 * transH.Jumlah,
              //  Bayar = -1 * transH.Jumlah,
                Discount = 0,
             //   UnApplied = -1 * transH.Unapplied,
                Sisa = -1 * transH.Unapplied,
                Kurs = transH.Kurs,
                Currency = trans.Currency,
                Nilai = transH.Nilai,
                Dpp = 0,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };

            var supplier = (from e in db.ApSuppls where e.Supplier == trans.Supplier select e).FirstOrDefault();
            supplier.Hutang -= transH.Jumlah;

            db.ApSuppls.Update(supplier);
            db.ApTransHs.Add(transH);
            db.ApHutangs.Add(transaksi);
            db.SaveChanges();

            var bank = (from e in dbBank.CbBanks where e.KodeBank == trans.KdBank select e).FirstOrDefault();

            var cekBukti = (from e in dbBank.CbTransHs where e.DocNo == transH.Bukti select e).FirstOrDefault();

            if (cekBukti == null)
            {
                if (!string.IsNullOrEmpty(transH.KdBank))
                {
                    CbTransH transBank = new CbTransH
                    {
                        DocNo = transH.Bukti,
                        KodeBank = trans.KdBank,
                        Tanggal = trans.Tanggal,
                        Keterangan = trans.Keterangan,
                        Kurs = bank.Kurs,
                        Saldo = -1 * (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar),
                        KSaldo = -1 * (trans.Kurs != 0 ? trans.JumBayar : 0),

                        CbTransDs = new List<CbTransD>()
                    };

                    transBank.CbTransDs.Add(new CbTransD()
                    {
                        SrcCode = KdSrc,
                        Keterangan = "Pembayaran Uang Muka " + trans.Supplier.ToUpper(),

                        KTerima = (trans.JumBayar < 0 ? (trans.Kurs != 0 ? -1 * trans.JumBayar : 0) : 0),
                        Terima = (trans.JumBayar < 0 ? (trans.Kurs != 0 ? -1 * (trans.Nilai) : -1 * trans.JumBayar) : 0),


                        KBayar = (trans.JumBayar > 0 ? (trans.Kurs != 0 ? trans.JumBayar : 0) :0),
                        Bayar = (trans.JumBayar > 0 ? (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar) : 0),

                        KJumlah = -1 * (trans.Kurs != 0 ? trans.JumBayar : 0),
                         Jumlah = -1 * (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar),
                         KValue = trans.Kurs,                      
                        Kurs = bank.Kurs
                    });


                    bank.KSaldo -= (trans.Kurs != 0 ? trans.JumBayar : 0);
                    bank.Saldo -= (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar);

                    dbBank.CbBanks.Update(bank);
                    dbBank.CbTransHs.Add(transBank);
                    dbBank.SaveChanges();

                }
            }

            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }



        public async Task<bool> DelTransH(int id)
        {
            try
            {
                using var db = _context.CreateDbContext();
                var ExistingTrans = db.ApTransHs.Where(x => x.ApTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    var cekFirst = db.ApHutangs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                    var supplier = (from e in db.ApSuppls where e.Supplier == ExistingTrans.Supplier select e).FirstOrDefault();

                    supplier.Hutang += ExistingTrans.Jumlah;


                    db.ApSuppls.Update(supplier);
                    db.ApTransHs.Remove(ExistingTrans);
                    db.ApHutangs.Remove(cekFirst);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #endregion Transaksi Hutang Class

        public ApTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.ApTransHs.Include(p => p.ApTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            using var db = _context.CreateDbContext();
            string kodeno = "DPY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '5' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = db.ApTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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
    }
}
