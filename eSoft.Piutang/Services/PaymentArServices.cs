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
    public class PaymentArServices: IPaymentArServices
    {
        private readonly DbContextPiutang _context;
        private readonly DbContextBank _contextBank;

        public PaymentArServices(DbContextPiutang context, DbContextBank contextBank)
        {
            _context = context;
            _contextBank = contextBank;
        }

        #region Transaksi Piutang Pembayaran Class

        public ArTransH GetTrans(int id)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.ArTransHId == id).FirstOrDefault();
        }

        public List<ArPiutng> GetPiutangSisa(string customer)
        {
            return _context.ArPiutngs.Where(x => x.Customer == customer && x.Sisa != 0).ToList();

        }
        public List<ArTransH> GetTransH()
        {
            List<ArTransH> arTrans = new List<ArTransH>();
            try
            {
                arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "14").ToList();
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

        public List<ArTransH> Get3TransH(DateTime tgl1, DateTime tgl2)
        {
            List<ArTransH> arTrans = new List<ArTransH>();

            arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date) && x.Kode == "14").ToList();
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
            return _context.ArTransDs.Where(x => x.Kode == "14").ToList();
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
                Discount = trans.JumDiskon,
                Unapplied = trans.UpdateUnapplied,
                Piutang = trans.JumPiutang,
                KdBank = trans.KdBank,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = 0,
                Netto = 0,
                Pajak = false,
                Kode = "14",
                ArCustId = trans.ArCustId,

                ArTransDs = new List<ArTransD>()
            };

            List<ArPiutng> transaksi = new List<ArPiutng>();
            transaksi = _context.ArPiutngs.Where(x => x.Customer == trans.Customer).ToList();

            foreach (var item in trans.ArTransDs)
            {
                transH.ArTransDs.Add(new ArTransD()
                {
                    Jumlah = item.Jumlah,
                    Kode = "14",
                    KodeTran = item.KodeTran,
                    Bukti = transH.Bukti,
                    Lpb = item.Lpb,
                    DueDate = item.DueDate,
                    Tanggal = trans.Tanggal,
                    Discount = item.Discount,
                    Bayar = item.Bayar,
                    Sisa = item.UpdateSisa

                });

                transaksi.Where(x => x.Dokumen == item.Lpb).ToList()
                    .ForEach(s =>
                    {
                        s.Bayar += item.Bayar + item.Discount;
                        s.Discount += item.Discount;
                        s.Sisa -= item.Bayar + item.Discount;
                    });

            }
            transH.ArTransDs.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);
            transaksi.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);

            var customer = (from e in _context.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();
            customer.Piutang -= trans.JumPiutang;

            ArPiutng Newtransaksi = new ArPiutng
            {
                Kode = "CA",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                KodeTran = "14",
                Jumlah = -1 * trans.JumPiutang,
                SldSisa = -1 * trans.JumPiutang,
                Bayar = -1 * trans.JumBayar,
                Discount = 0,
                UnApplied = -1 * trans.UpdateUnapplied,
                Sisa = -1 * trans.UpdateUnapplied,

                Dpp = 0,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };

            _context.ArCusts.Update(customer);
            _context.ArTransHs.Add(transH);
            _context.ArPiutngs.UpdateRange(transaksi);
            _context.ArPiutngs.Add(Newtransaksi);




            var cekBukti = (from e in _contextBank.CbTransHs where e.DocNo == transH.Bukti select e).FirstOrDefault();

            if (cekBukti == null)
            {
                if (!string.IsNullOrEmpty(transH.KdBank) )
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
                    foreach (var item in trans.ArTransDs)
                    {
                        if(item.Bayar != 0)
                        {
                            transBank.CbTransDs.Add(new CbTransD()
                            {

                                SrcCode = KdSrc,
                                Keterangan = "Pembayaran Piutang" + trans.Customer.ToUpper() + "," + customer.NamaCust,
                                Terima = item.Bayar,
                                Jumlah = item.Bayar,

                            });
                        }
                        
                    }
                    var bank = (from e in _contextBank.CbBanks where e.KodeBank == trans.KdBank select e).FirstOrDefault();
                    bank.Saldo += trans.JumBayar;

                    _contextBank.CbBanks.Update(bank);
                    _contextBank.CbTransHs.Add(transBank);



                }
            }
            _context.SaveChanges();
            _contextBank.SaveChanges();

            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }



        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _context.ArTransHs.Where(x => x.ArTransHId == id).FirstOrDefault();
                var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();

                List<ArPiutng> transaksi = new List<ArPiutng>();
                transaksi = _context.ArPiutngs.Where(x => x.Customer == ExistingTrans.Customer).ToList();

                if (ExistingTrans != null)
                {

                    foreach (var item in ExistingTrans.ArTransDs)
                    {
                        transaksi.Where(x => x.Dokumen == item.Lpb).ToList()
                    .ForEach(s =>
                    {
                        s.Bayar -= item.Bayar + item.Discount;
                        s.Discount -= item.Discount;
                        s.Sisa += item.Bayar + item.Discount;
                    });
                    }
                }
                var customer = (from e in _context.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                customer.Piutang += ExistingTrans.Piutang;


                _context.ArCusts.Update(customer);
                _context.ArTransHs.Remove(ExistingTrans);
                _context.ArPiutngs.Remove(cekFirst);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool CekAlreadyPosting(string dokumen)
        {
            var cekFirst = _context.ArTransHs.Where(x => x.Bukti == dokumen).FirstOrDefault();

            if (!string.IsNullOrEmpty(cekFirst.AcctSet))
            {
                return true;
            }
            return false;
        }


        public ArTransH GetTransDoc(string docno)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        #endregion Transaksi Piutang Class

        public string GetNumber()
        {
            string kodeno = "BMY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '3' + thnbln.Substring(2, 2) + '-';
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

