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
        private readonly DbContextPiutang _context;
        private readonly DbContextBank _contextBank;

        public PaymentArDpServices(DbContextPiutang context, DbContextBank contextBank)
        {
            _context = context;
            _contextBank = contextBank;
        }

        #region Transaksi Piutang Pembayaran Class

        public ArTransH GetTrans(int id)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.ArTransHId == id).FirstOrDefault();
        }

        public bool GetPiutangSisa(string xDocNo)
        {
            var sisa = _context.ArPiutngs.Where(x => x.Dokumen == xDocNo).FirstOrDefault();
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
            List<ArTransH> arTrans = new List<ArTransH>();
            try
            {
                arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "13").ToList();
                foreach (var item in arTrans)
                {
                    item.NamaCust = (from e in _context.ArCusts where e.Customer == item.Customer select e.NamaCust).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return arTrans;
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await _context.ArTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await _context.ArTransHs.ToListAsync();

        }

        public List<ArTransH> Get3TransH()
        {
            List<ArTransH> arTrans = new List<ArTransH>();

            arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "13").ToList();
            foreach (var item in arTrans)
            {
                item.NamaCust = (from e in _context.ArCusts where e.Customer == item.Customer select e.NamaCust).FirstOrDefault();
            }

            return arTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<ArTransD> GetTransD()
        {
            return _context.ArTransDs.Where(x => x.Kode == "13").ToList();
        }

        public ArTransH AddTransH(ArTransHView trans)
        {
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

            var customer = (from e in _context.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();
            customer.Piutang -= transH.Jumlah;

            _context.ArCusts.Update(customer);
            _context.ArTransHs.Add(transH);
            _context.ArPiutngs.Add(transaksi);
            _context.SaveChanges();

            var cekBukti = (from e in _contextBank.CbTransHs where e.DocNo == transH.Bukti select e).FirstOrDefault();

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

                    var bank = (from e in _contextBank.CbBanks where e.KodeBank == trans.KdBank select e).FirstOrDefault();
                    bank.Saldo += trans.JumBayar;

                    _contextBank.CbBanks.Update(bank);
                    _contextBank.CbTransHs.Add(transBank);
                    _contextBank.SaveChanges();

                }
            }
            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;
            


        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _context.ArTransHs.Where(x => x.ArTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                    var customer = (from e in _context.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                    customer.Piutang -= ExistingTrans.Jumlah;


                    _context.ArCusts.Update(customer);
                    _context.ArTransHs.Remove(ExistingTrans);
                    _context.ArPiutngs.Remove(cekFirst);
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

        #endregion Transaksi Piutang Class
       
        public ArTransH GetTransDoc(string docno)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            string kodeno = "UMY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ArTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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
