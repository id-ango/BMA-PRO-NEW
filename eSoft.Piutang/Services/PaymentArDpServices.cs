using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Piutang.View;
using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Piutang.Services
{
    public class PaymentArDpServices : IPaymentArDpServices
    {
        private readonly IDbContextFactory<DbContextPiutang> _context;
        private readonly IDbContextFactory<DbContextBank> _contextBank;

        public PaymentArDpServices(IDbContextFactory<DbContextPiutang> context, IDbContextFactory<DbContextBank> contextBank)
        {
            _context = context;
            _contextBank = contextBank;
        }

        #region Transaksi Piutang Pembayaran Class

        public ArTransH GetTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.ArTransHs.Include(p => p.ArTransDs).Where(x => x.ArTransHId == id).FirstOrDefault();
        }

        public bool GetPiutangSisa(string xDocNo)
        {
            using var db = _context.CreateDbContext();
            var sisa = db.ArPiutngs.Where(x => x.Dokumen == xDocNo).FirstOrDefault();
            if (sisa.Jumlah == sisa.Sisa)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public List<ArTransH> GetTransH()
        {
            using var db = _context.CreateDbContext();
            List<ArTransH> arTrans = new List<ArTransH>();
            try
            {
                arTrans = db.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "13").ToList();
                foreach (var item in arTrans)
                {
                    item.NamaCust = (from e in db.ArCusts where e.Customer == item.Customer select e.NamaCust).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return arTrans;
        }

        public List<ArTransH> Get3TransH()
        {
            using var db = _context.CreateDbContext();
            List<ArTransH> arTrans = new List<ArTransH>();

            arTrans = db.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "13").ToList();
            foreach (var item in arTrans)
            {
                item.NamaCust = (from e in db.ArCusts where e.Customer == item.Customer select e.NamaCust).FirstOrDefault();
            }

            return arTrans;
        }

        public List<ArTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.ArTransDs.Where(x => x.Kode == "13").ToList();
        }

        public ArTransH AddTransH(ArTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbBank = _contextBank.CreateDbContext();

            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            string KdSrc = "AR";

            ArTransH transH = new ArTransH
            {
                Bukti = GetNumber(),
                Customer = trans.Customer.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.JumBayar,
                Discount = 0,
                Unapplied = trans.UpdateUnapplied,
                Piutang = 0,
                KdBank = trans.KdBank,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = 0,
                Netto = 0,
                Pajak = false,
                Kode = "13",
                ArCustId = trans.ArCustId

                //  ArTransDs = new List<ArTransD>()
            };

            #region detailTrans

            //List<ArPiutng> transaksi = new List<ArPiutng>();
            //transaksi = _context.ArPiutngs.Where(x => x.Customer == trans.Customer && x.Sisa != 0).ToList();

            //foreach (var item in trans.ArTransDs)
            //{
            //    transH.ArTransDs.Add(new ArTransD()
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
            //transH.ArTransDs.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);
            //transaksi.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);

            #endregion



            ArPiutng transaksi = new ArPiutng
            {
                Kode = "CA",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                KodeTran = "13",
                Jumlah = -1 * transH.Jumlah,
                SldSisa = -1 * transH.Jumlah,
                Discount = 0,

                Sisa = -1 * transH.Unapplied,

                Dpp = 0,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
                //       Bayar = -1 * transH.Jumlah,
                // UnApplied = -1 * transH.Unapplied,
            };

            var customer = (from e in db.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();
            customer.Piutang -= transH.Jumlah;

            db.ArCusts.Update(customer);
            db.ArTransHs.Add(transH);
            db.ArPiutngs.Add(transaksi);
            db.SaveChanges();

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
                        Saldo = trans.JumBayar,

                        CbTransDs = new List<CbTransD>()
                    };

                    transBank.CbTransDs.Add(new CbTransD()
                    {
                        SrcCode = KdSrc,
                        Keterangan = "Pembayaran Uang Muka " + trans.Customer.ToUpper(),
                        Terima = (trans.JumBayar > 0 ?  (trans.JumBayar) : 0),
                        Bayar = (trans.JumBayar < 0 ?  -1 * (trans.JumBayar) : 0),
                        Jumlah = trans.JumBayar,

                    });

                    var bank = (from e in dbBank.CbBanks where e.KodeBank == trans.KdBank select e).FirstOrDefault();
                    bank.Saldo += trans.JumBayar;

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
                var ExistingTrans = db.ArTransHs.Where(x => x.ArTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    var cekFirst = db.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                    var customer = (from e in db.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                    customer.Piutang -= ExistingTrans.Jumlah;


                    db.ArCusts.Update(customer);
                    db.ArTransHs.Remove(ExistingTrans);
                    db.ArPiutngs.Remove(cekFirst);
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

        #endregion Transaksi Piutang Class
       
        public ArTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.ArTransHs.Include(p => p.ArTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            using var db = _context.CreateDbContext();
            string kodeno = "UMY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = db.ArTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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
