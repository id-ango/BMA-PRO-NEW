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
    public class PaymentApServices : IPaymentApServices
    {
        private readonly DbContextHutang _context;
        private readonly DbContextBank _contextBank;

        public PaymentApServices(DbContextHutang context, DbContextBank contextBank)
        {
            _context = context;
            _contextBank = contextBank;
        }

        #region Transaksi Hutang Pembayaran Class

        public ApTransH GetTrans(int id)
        {
            return _context.ApTransHs.Include(p => p.ApTransDs).Where(x => x.ApTransHId == id).FirstOrDefault();
        }

        public List<ApHutang> GetHutangSisa(string Supplier)
        {
            return _context.ApHutangs.Where(x => x.Supplier == Supplier && x.Sisa != 0).ToList();

        }
        public List<ApTransH> GetTransH()
        {
            List<ApTransH> ApTrans = new List<ApTransH>();
            try
            {
                ApTrans = _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "24").ToList();
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

            ApTrans = _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "24").ToList();
            foreach (var item in ApTrans)
            {
                item.NamaSup = (from e in _context.ApSuppls where e.Supplier == item.Supplier select e.NamaSup).FirstOrDefault();
            }

            return ApTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<ApTransD> GetTransD()
        {
            return _context.ApTransDs.Where(x => x.Kode == "24").ToList();
        }

        public ApTransH AddTransH(ApTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            string KdSrc = "AP";

            ApTransH transH = new ApTransH
            {
                Bukti = GetNumber(),
                Supplier = trans.Supplier.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Currency = trans.Currency,
                Kurs = trans.Kurs,
                Nilai = trans.Nilai,
                Jumlah = trans.JumBayar,
                Discount = trans.JumDiskon,
                Unapplied = trans.UpdateUnapplied,
                Hutang = trans.JumHutang,
                KdBank = trans.KdBank,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = 0,
                Netto = 0,
                Pajak = false,
                Kode = "24",
                ApSupplId = trans.ApSupplId,

                ApTransDs = new List<ApTransD>()
            };

            List<ApHutang> transaksi = new List<ApHutang>();
            transaksi = _context.ApHutangs.Where(x => x.Supplier == trans.Supplier).ToList();

            foreach (var item in trans.ApTransDs)
            {
                transH.ApTransDs.Add(new ApTransD()
                {
                    Jumlah = item.Jumlah,
                    Kode = "24",
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
                        if (s.Bayar + s.Discount != 0)
                        {
                            //  _context.IrTransHs.Where(x => x.NoLpb == item.Lpb).FirstOrDefault().Cek = "3";
                        }
                        else
                        {
                            //   _context.IrTransHs.Where(x => x.NoLpb == item.Lpb).FirstOrDefault().Cek = "1";
                        }
                    });

            }
            transH.ApTransDs.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);
            transaksi.RemoveAll(x => x.Bayar == 0 && x.Discount == 0);

            // Verify supplier exists before updating
            var Supplier = _context.ApSuppls.FirstOrDefault(e => e.Supplier == trans.Supplier);
            if (Supplier == null)
            {
                throw new InvalidOperationException($"Supplier '{trans.Supplier}' not found in ApSuppl master data. Please create supplier record first.");
            }

            Supplier.Hutang -= trans.JumHutang;

            ApHutang Newtransaksi = new ApHutang
            {
                Kode = "CA",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Supplier = transH.Supplier,
                Keterangan = transH.Keterangan,
                KodeTran = "24",
                Jumlah = -1 * trans.JumHutang,
                SldSisa = -1 * trans.JumHutang,
                Bayar = -1 * trans.JumBayar,
                Discount = 0,
                UnApplied = -1 * trans.UpdateUnapplied,
                Sisa = -1 * trans.UpdateUnapplied,
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

            _context.ApSuppls.Update(Supplier);
            _context.ApTransHs.Add(transH);
            _context.ApHutangs.UpdateRange(transaksi);
            _context.ApHutangs.Add(Newtransaksi);
            _context.SaveChanges();


            var bank = (from e in _contextBank.CbBanks where e.KodeBank == trans.KdBank select e).FirstOrDefault();
            var cekBukti = (from e in _contextBank.CbTransHs where e.DocNo == transH.Bukti select e).FirstOrDefault();

            if (cekBukti == null)
            {
                if (!string.IsNullOrEmpty(transH.KdBank))
                {
                    CbTransH transBank = new CbTransH
                    {
                        DocNo = transH.Bukti,
                        KodeBank = trans.KdBank,
                        Kurs = bank.Kurs,
                        Tanggal = trans.Tanggal,
                        Keterangan = trans.Keterangan,
                        // Saldo = -1 * trans.JumBayar,
                        Saldo = -1 * (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar),
                        KSaldo = -1 * (trans.Kurs != 0 ? trans.JumBayar : 0),
                        CbTransDs = new List<CbTransD>()
                    };
              

                    foreach (var item in trans.ApTransDs)
                    {
                        if (item.Bayar != 0)
                        {

                            transBank.CbTransDs.Add(new CbTransD()
                            {
                                SrcCode = KdSrc,
                                Keterangan = "Pembayaran Hutang" + trans.Supplier.ToUpper() + "," + Supplier.NamaSup,
                                //   Bayar = item.Bayar,
                                //   Jumlah = -1 * item.Bayar,
                         //      KJumlah = -1 * (trans.Kurs != 0 ? trans.JumBayar : 0),
                           //     Jumlah = -1 * (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar),

                                KTerima = (item.Bayar < 0 ? (trans.Kurs != 0 ? -1*item.Bayar : 0) : 0),                             
                                Terima = (item.Bayar < 0 ? (trans.Kurs != 0 ? -1*(item.Bayar * trans.Kurs) : -1*item.Bayar) : 0),
                                KBayar = (item.Bayar > 0 ? (trans.Kurs != 0 ? item.Bayar : 0) : 0),
                                Bayar = (item.Bayar > 0 ? (trans.Kurs != 0 ? item.Bayar * trans.Kurs : item.Bayar):0),                            
                                KJumlah = (item.Bayar>0 ? (-1* (trans.Kurs != 0 ? item.Bayar : 0)) : -1* (trans.Kurs != 0 ? item.Bayar : 0)),
                                Jumlah = (item.Bayar>0 ? (-1*(trans.Kurs != 0 ? item.Bayar * trans.Kurs : item.Bayar)) : -1*(trans.Kurs != 0 ? item.Bayar * trans.Kurs : item.Bayar)),
                                KValue = trans.Kurs,
                                Kurs = bank.Kurs
                            });

                        }
                    }
                
                  //  bank.Saldo -= trans.JumBayar;
                    bank.KSaldo -= (trans.Kurs != 0 ? trans.JumBayar : 0);
                    bank.Saldo -= (trans.Kurs != 0 ? trans.Nilai : trans.JumBayar);

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
                var ExistingTrans = _context.ApTransHs.Where(x => x.ApTransHId == id).FirstOrDefault();
                var cekFirst = _context.ApHutangs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();

                List<ApHutang> transaksi = new List<ApHutang>();
                transaksi = _context.ApHutangs.Where(x => x.Supplier == ExistingTrans.Supplier).ToList();
                if (ExistingTrans != null)
                {

                    foreach (var item in ExistingTrans.ApTransDs)
                    {
                        transaksi.Where(x => x.Dokumen == item.Lpb).ToList()
                    .ForEach(s =>
                    {
                        s.Bayar -= item.Bayar + item.Discount;
                        s.Discount -= item.Discount;
                        s.Sisa += item.Bayar + item.Discount;

                        if (s.Bayar + s.Discount != 0)
                        {
                            //  _context.IrTransHs.Where(x => x.NoLpb == item.Lpb).FirstOrDefault().Cek = "3";
                        }
                        else
                        {
                            //  _context.IrTransHs.Where(x => x.NoLpb == item.Lpb).FirstOrDefault().Cek = "1";
                        }
                    });
                    }
                }


                var Supplier = (from e in _context.ApSuppls where e.Supplier == ExistingTrans.Supplier select e).FirstOrDefault();

                Supplier.Hutang += ExistingTrans.Jumlah;


                _context.ApSuppls.Update(Supplier);
                _context.ApTransHs.Remove(ExistingTrans);
                _context.ApHutangs.Remove(cekFirst);
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
            var cekFirst = _context.ApTransHs.Where(x => x.Bukti == dokumen).FirstOrDefault();

            if (!string.IsNullOrEmpty(cekFirst.AcctSet))
            {
                return true;
            }
            return false;
        }


        #endregion Transaksi Hutang Class

        public ApTransH GetTransDoc(string docno)
        {
            return _context.ApTransHs.Include(p => p.ApTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            string kodeno = "BKY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '3' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ApTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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
