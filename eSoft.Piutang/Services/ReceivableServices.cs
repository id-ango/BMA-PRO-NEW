using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Piutang.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Piutang.Services
{
    public class ReceivableServices : IReceivableServices
    {
        private readonly IDbContextFactory<DbContextPiutang> _context;

        public ReceivableServices(IDbContextFactory<DbContextPiutang> context)
        {
            _context = context;
        }

        public bool CekKdCustomer(string customer)
        {
            using (var ctx = _context.CreateDbContext())
            {
                string test = customer.ToUpper();
                var cekFirst = ctx.ArCusts.Where(x => x.Customer == test).ToList();
                if (cekFirst.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }


        public List<ArCust> GetCustomer()
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArCusts.OrderBy(x => x.NamaCust).ToList();
            }
        }

        public ArCust GetCustomerId(int id)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArCusts.Where(x => x.ArCustId == id).FirstOrDefault();
            }
        }

        public ArCust GetCustomerCode(string xKode)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArCusts.Where(x => x.Customer == xKode).FirstOrDefault();
            }
        }

        public bool AddCustomer(CustomerView customers)
        {
            using (var ctx = _context.CreateDbContext())
            {
                string test = customers.Customer.ToUpper();
                var cekFirst = ctx.ArCusts.Where(x => x.Customer == test).ToList();
                if (cekFirst.Count == 0)
                {
                    ArCust Customer = new ArCust()
                    {
                        Customer = customers.Customer.ToUpper(),
                        NamaCust = customers.NamaCust,
                        Termin = customers.Termin,
                        Alamat = customers.Alamat,
                        Kota = customers.Kota,
                        Telpon = customers.Telpon,
                        NamaLengkap = customers.NamaLengkap,
                        AcctSet = customers.AcctSet,
                        //AcctPjk = customers.AcctPjk,
                        AlmtKrm = customers.AlmtKrm,
                        KotaKrm = customers.KotaKrm,
                        //ProvKirim = customers.ProvKirim,
                        NPWP_Cust = customers.NPWP_Cust,
                        Kontak = customers.Kontak,
                        Golongan = customers.Golongan,
                        Expedisi = customers.Expedisi



                    };
                    ctx.ArCusts.Add(Customer);
                    ctx.SaveChanges();
                    return true;
                }
                else
                {

                    return false;
                }
            }
        }

        public async Task<bool> EditCustomer(CustomerView customers)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingCustomer = ctx.ArCusts.Where(x => x.ArCustId == customers.ArCustId).FirstOrDefault();
                    if (ExistingCustomer != null)
                    {
                        ExistingCustomer.NamaCust = customers.NamaCust;
                        ExistingCustomer.Alamat = customers.Alamat;
                        ExistingCustomer.Kota = customers.Kota;
                        ExistingCustomer.Telpon = customers.Telpon;
                        ExistingCustomer.Termin = customers.Termin;
                        ExistingCustomer.NamaLengkap = customers.NamaLengkap;
                        ExistingCustomer.AcctSet = customers.AcctSet;
                        ExistingCustomer.AcctPjk = customers.AcctPjk;
                        ExistingCustomer.AlmtKrm = customers.AlmtKrm;
                        ExistingCustomer.KotaKrm = customers.KotaKrm;
                        ExistingCustomer.ProvKirim = customers.ProvKirim;
                        ExistingCustomer.Kontak = customers.Kontak;
                        ExistingCustomer.NPWP_Cust = customers.NPWP_Cust;
                        ExistingCustomer.Golongan  = customers.Golongan;
                        ExistingCustomer.Expedisi = customers.Expedisi;

                        ctx.ArCusts.Update(ExistingCustomer);
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelCustomer(int customers)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingCustomer = ctx.ArCusts.Where(x => x.ArCustId == customers).FirstOrDefault();
                    if (ExistingCustomer != null)
                    {
                        ctx.ArCusts.Remove(ExistingCustomer);
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #region ArAcct Class

        public bool CekAcctSet(string customer)
        {
            string test = customer.ToUpper();
            using (var ctx = _context.CreateDbContext())
            {
                var cekFirst = ctx.ArAccts.Where(x => x.AcctSet == test).ToList();
                if (cekFirst.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }


        public List<ArAcct> GetArAkunSet()
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArAccts.ToList();
            }
        }

        public ArAcct GetArAkunSetId(int id)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArAccts.Where(x => x.ArAcctId == id).FirstOrDefault();
            }
        }

        public bool AddAkunSet(ArAcctView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
            using (var ctx = _context.CreateDbContext())
            {
                var cekFirst = ctx.ArAccts.Where(x => x.AcctSet == test).ToList();
                if (cekFirst.Count == 0)
                {
                    ArAcct AcctCode = new ArAcct()
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
                    ctx.ArAccts.Add(AcctCode);
                    ctx.SaveChanges();
                    return true;
                }
                else
                {

                    return false;
                }
            }


        }

        public async Task<bool> EditAkunSet(ArAcctView codeview)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingAkunSet = ctx.ArAccts.Where(x => x.ArAcctId == codeview.ArAcctId).FirstOrDefault();
                    if (ExistingAkunSet != null)
                    {
                        ExistingAkunSet.Description = codeview.Description;
                        ExistingAkunSet.Acct1 = codeview.Acct1;
                        ExistingAkunSet.Acct2 = codeview.Acct2;
                        ExistingAkunSet.Acct3 = codeview.Acct3;
                        ExistingAkunSet.Acct4 = codeview.Acct4;
                        ExistingAkunSet.Acct5 = codeview.Acct5;
                        ExistingAkunSet.Acct6 = codeview.Acct6;
                        await ctx.SaveChangesAsync();
                        return true;
                    }
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
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingAkunSet = ctx.ArAccts.Where(x => x.ArAcctId == codeview).FirstOrDefault();
                    if (ExistingAkunSet != null)
                    {
                        ctx.ArAccts.Remove(ExistingAkunSet);
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception) { throw; }

            return false;

        }
        #endregion ArAcct Class

        #region ArDist Class

        public bool CekDistCode(string distcode)
        {
            string test = distcode.ToUpper();
            using var db = _context.CreateDbContext();
            var cekFirst = db.ArDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }


        public List<ArDist> GetDist()
        {
            using var db = _context.CreateDbContext();
            return db.ArDists.ToList();
        }

        public ArDist GetDistId(int id)
        {
            using var db = _context.CreateDbContext();
            return db.ArDists.Where(x => x.ArDistId == id).FirstOrDefault();
        }

        public bool AddDist(ArDistView codeview)
        {
            string test = codeview.DistCode.ToUpper();
            using (var ctx = _context.CreateDbContext())
            {
                var cekFirst = ctx.ArDists.Where(x => x.DistCode == test).ToList();
                if (cekFirst.Count == 0)
                {
                    ArDist AcctCode = new ArDist()
                    {
                        DistCode = codeview.DistCode.ToUpper(),
                        Description = codeview.Description,
                        Dist1 = codeview.Dist1

                    };
                    ctx.ArDists.Add(AcctCode);
                    ctx.SaveChanges();
                    return true;
                }
                else
                {

                    return false;
                }
            }


        }

        public async Task<bool> EditDist(ArDistView codeview)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingDist = ctx.ArDists.Where(x => x.ArDistId == codeview.ArDistId).FirstOrDefault();
                    if (ExistingDist != null)
                    {
                        ExistingDist.Description = codeview.Description;
                        ExistingDist.Dist1 = codeview.Dist1;
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception) { throw; }

            return false;

        }

        public async Task<bool> DelDist(int codeview)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingDist = ctx.ArDists.Where(x => x.ArDistId == codeview).FirstOrDefault();
                    if (ExistingDist != null)
                    {
                        ctx.ArDists.Remove(ExistingDist);
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception) { throw; }

            return false;

        }
        #endregion ArDist Class



        #region Transaksi Piutang Class

        public ArTransH GetTrans(int id)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArTransHs.Include(p => p.ArTransDs).Where(x => x.ArTransHId == id).FirstOrDefault();
            }
        }

        public ArPiutng GetPiutang(string bukti)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArPiutngs.Where(x => x.Dokumen == bukti).FirstOrDefault();
            }
        }


        public List<ArTransH> GetTransH()
        {
            List<ArTransH> arTrans = new List<ArTransH>();
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    arTrans = ctx.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "11").ToList();
                    foreach (var item in arTrans)
                    {
                        item.NamaCust = (from e in ctx.ArCusts where e.ArCustId == item.ArCustId select e.NamaCust).FirstOrDefault();
                    }
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

            //  arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "11").ToList();
            using (var ctx = _context.CreateDbContext())
            {
                arTrans = ctx.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "11")
                    .Select(x => new ArTransH
                    {
                        ArTransHId = x.ArTransHId,
                        Bukti = x.Bukti,
                        Tanggal = x.Tanggal,
                        Keterangan = x.Keterangan,
                        Customer = x.Customer,
                        NamaCust = (from e in ctx.ArCusts where e.Customer == x.Customer select e.NamaCust).SingleOrDefault(),
                        Jumlah = x.Jumlah,
                    })
                    .ToList();
            }
            //foreach (var item in arTrans)
            //{
            //    item.NamaCust = (from e in _context.ArCusts where e.ArCustId == item.ArCustId select e.NamaCust).FirstOrDefault();
            //}

            return arTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<ArTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.ArTransDs.ToList();
        }
        public bool BatalPiutang(ArPiutng piutang)
        {
            using (var ctx = _context.CreateDbContext())
            {
                ctx.ArPiutngs.Remove(piutang);
                ctx.SaveChanges();
            }

            return true;

        }
        public ArTransH AddTransH(ArTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            ArTransH transH = new ArTransH
            {
                Bukti = GetNumber(),
                Customer = trans.Customer.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = trans.Jumlah,
                Netto = 0,
                Discount = 0,
                Piutang = 0,
                Pajak = false,
                Unapplied = 0,
                Kode = "11",
                ArCustId = trans.ArCustId,

                ArTransDs = new List<ArTransD>()
            };
            foreach (var item in trans.ArTransDs)
            {
                transH.ArTransDs.Add(new ArTransD()
                {
                    DistCode = item.DistCode,
                    Keterangan = item.Keterangan,
                    Jumlah = item.Jumlah,
                    Kode = "11",
                    KodeTran = "11",
                    Bukti = transH.Bukti,
                    Sisa = item.Jumlah,
                    Discount = 0,
                    Bayar = 0,
                    Tanggal = trans.Tanggal
                });
            }
            ArPiutng transaksi = new ArPiutng
            {
                Kode = "IN",
                Dokumen = transH.Bukti,
                Tanggal = transH.Tanggal,
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                KodeTran = "11",
                Jumlah = transH.Jumlah,
                Bayar = 0,
                Discount = 0,
                UnApplied = 0,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                Dpp = transH.Jumlah,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };

            using (var ctx = _context.CreateDbContext())
            {
                var customer = (from e in ctx.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();
                customer.Piutang += trans.Jumlah;

                ctx.ArCusts.Update(customer);
                ctx.ArTransHs.Add(transH);
                ctx.ArPiutngs.Add(transaksi);
                ctx.SaveChanges();
            }

            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }

        public ArTransH EditTransH(ArTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            using var db = _context.CreateDbContext();
            var cekFirst = db.ArPiutngs.Where(x => x.Dokumen == trans.Bukti).FirstOrDefault();


            ArTransH transH = new ArTransH
            {

                Customer = trans.Customer.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                PPn = 0,
                PPh = 0,
                JumPPh = 0,
                JumPPn = 0,
                Bruto = trans.Jumlah,
                Netto = 0,
                Discount = 0,
                Piutang = 0,
                Pajak = false,
                Unapplied = 0,
                Kode = "11",
                ArCustId = trans.ArCustId,

                ArTransDs = new List<ArTransD>()
            };
            foreach (var item in trans.ArTransDs)
            {
                transH.ArTransDs.Add(new ArTransD()
                {
                    DistCode = item.DistCode,
                    Keterangan = item.Keterangan,
                    Jumlah = item.Jumlah,
                    Kode = "11",
                    KodeTran = "11",
                    Bukti = transH.Bukti,
                    Sisa = item.Jumlah,
                    Discount = 0,
                    Bayar = 0,
                    Tanggal = trans.Tanggal
                });
            }

            ArPiutng transaksi = new ArPiutng
            {
                Kode = "IN",
                Tanggal = transH.Tanggal,
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                KodeTran = "11",
                Jumlah = transH.Jumlah,
                Bayar = 0,
                Discount = 0,
                UnApplied = 0,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                Dpp = transH.Jumlah,
                PPn = 0,
                PPh = 0,
                SldBayar = 0,
                SldDisc = 0,
                SldUnpl = 0
            };


            using (var ctx = _context.CreateDbContext())
            {
                var ExistingTrans = ctx.ArTransHs.Where(x => x.ArTransHId == trans.ArTransHId).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    transH.Bukti = ExistingTrans.Bukti;
                    transaksi.Dokumen = ExistingTrans.Bukti;

                    ctx.ArTransHs.Remove(ExistingTrans);

                    var customer = (from e in ctx.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();

                    customer.Piutang -= ExistingTrans.Jumlah;
                    customer.Piutang += trans.Jumlah;

                    ctx.ArCusts.Update(customer);
                    ctx.ArPiutngs.Remove(cekFirst);

                    ctx.ArTransHs.Add(transH);
                    ctx.ArPiutngs.Add(transaksi);
                    ctx.SaveChanges();

                    var TempTrans = GetTransDoc(transH.Bukti);

                    return TempTrans;

                }
                else
                {
                    return ExistingTrans;
                }
            }


            // return false;


        }
        public bool CekJual(string noLpb)
        {
            using var db = _context.CreateDbContext();
            var cekFirst = db.ArPiutngs.Where(x => x.Dokumen == noLpb && x.Bayar == 0).FirstOrDefault();

            if (cekFirst != null)
                return true;

            return false;
        }
        public bool CekAlreadyPayment(string dokumen)
        {
            using var db = _context.CreateDbContext();
            var cekFirst = db.ArPiutngs.Where(x => x.Dokumen == dokumen).FirstOrDefault();

            if (cekFirst.SldSisa != cekFirst.Sisa)
            {
                return true;
            }
            return false;
        }

        public bool HasSettlement(string documentNo)
        {
            using var db = _context.CreateDbContext();
            return db.ArPiutngs.Any(x => x.Dokumen == documentNo && x.Bayar > 0);
        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                using (var ctx = _context.CreateDbContext())
                {
                    var ExistingTrans = ctx.ArTransHs.Where(x => x.ArTransHId == id).FirstOrDefault();
                    if (ExistingTrans != null)
                    {
                        var cekFirst = ctx.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                        var customer = (from e in ctx.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                        customer.Piutang -= ExistingTrans.Jumlah;


                        ctx.ArCusts.Update(customer);
                        ctx.ArTransHs.Remove(ExistingTrans);
                        ctx.ArPiutngs.Remove(cekFirst);
                        await ctx.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public ArTransH GetTransDoc(string docno)
        {
            using (var ctx = _context.CreateDbContext())
            {
                return ctx.ArTransHs.Include(p => p.ArTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
            }
        }

        #endregion Transaksi Piutang Class

        #region laporan piutang

        public List<ArPiutng> Detail1(string xKdHeader)
        {

            List<ArPiutng> trans = new List<ArPiutng>();

            // trans = _context.ArPiutngs.Where(x => x.Customer == xKdHeader && (x.Sisa != 0)).ToList();
            using (var ctx = _context.CreateDbContext())
            {
                trans = ctx.ArPiutngs.Where(x => x.Customer == xKdHeader).ToList();
            }
            return trans;
        }

        public List<ArPiutngView> GetUangMuka()
        {
            List<ArPiutngView> transView = new List<ArPiutngView>();
            using var db = _context.CreateDbContext();
            var trans = db.ArPiutngs.Where(x => x.Kode == "CA" && x.KodeTran == "13" && x.Sisa != 0).ToList();
            var customer = GetCustomer();


            if (trans != null && customer != null)
            {
                foreach (var header in trans)
                {
                    transView.Add(new ArPiutngView()
                    {
                        Sisa = header.Sisa,
                        Dokumen = header.Dokumen,
                        Tanggal = header.Tanggal,
                        DueDate = header.DueDate,
                        Customer = header.Customer,
                        Keterangan = header.Keterangan,
                        NamaCust = customer.Find(x => x.Customer == header.Customer).NamaCust
                    });
                }
                //transView = (from header in trans
                //             join detail in customer on header.Customer equals detail.Customer
                //             select new ArPiutngView()
                //             {
                //                 Sisa = header.Sisa,
                //                 Dokumen = header.Dokumen,
                //                 Tanggal = header.Tanggal,
                //                 DueDate = header.DueDate,
                //                 Customer = header.Customer,
                //                 NamaCust = detail.NamaCust,
                //                 ArPiutngId = 0
                //                 //  KdBank = GetTransDoc(header.Dokumen).KdBank
                //             }).ToList();
            }

            return transView;
        }
        #endregion

        public string GetNumber()
        {
            string kodeno = "ARI";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            using (var db = _context.CreateDbContext())
            {
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

        public List<ArAgingView> GetAgingSchedule()
        {
            List<ArPiutng> trans = new List<ArPiutng>();
            List<ArAgingView> transaksi = new List<ArAgingView>();
            using var db = _context.CreateDbContext();
            List<ArCust> supplier = db.ArCusts.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;
            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            using (var ctx = _context.CreateDbContext())
            {
                trans = ctx.ArPiutngs
                    .AsNoTracking()
                    .Where(x => x.Sisa != 0)
                    .OrderBy(x => x.Customer)
                    .ThenByDescending(x => x.Dokumen)
                    .ToList();

                // Ambil semua pembayaran (ArTransD) sekaligus, join ke ArTransH untuk tanggal
                var semuaPembayaran = ctx.ArTransDs
                    .AsNoTracking()
                    .Include(x => x.ArTransH)
                    .Where(x => x.Bayar > 0 || x.Discount > 0)
                    .ToList();

                foreach (var ap in trans)
                {
                    duedate = ap.DueDate ?? ap.Tanggal;
                    date1 = duedate.AddMonths(1);
                    date2 = duedate.AddMonths(2);
                    date3 = duedate.AddMonths(3);

                    // Cari histori pembayaran untuk dokumen ini
                    var pembayaranDok = semuaPembayaran
                        .Where(x => x.Lpb == ap.Dokumen)
                        .OrderBy(x => x.ArTransH?.Tanggal)
                        .ToList();

                    int jumlahCicilan = pembayaranDok.Count;
                    decimal sudahBayar = pembayaranDok.Sum(x => x.Bayar + x.Discount);
                    DateTime? tglTerakhirBayar = pembayaranDok.LastOrDefault()?.ArTransH?.Tanggal;
                    int hariSejakBayar = tglTerakhirBayar.HasValue
                        ? (int)(currentDate - tglTerakhirBayar.Value.Date).TotalDays
                        : (int)(currentDate - ap.Tanggal.Date).TotalDays;

                    transaksi.Add(new ArAgingView()
                    {
                        Kode = ap.Kode,
                        ArAgingId = ap.ArPiutngId,
                        Customer = ap.Customer,
                        Tanggal = ap.Tanggal,
                        Dokumen = ap.Dokumen,
                        Duedate = duedate,
                        Cicilan = (ap.Sisa != ap.SldSisa ? true : false),
                        NamaCust = (from e in supplier where e.Customer == ap.Customer select e.NamaCust).FirstOrDefault(),
                        Salesman = ap.Salesman,
                        Keterangan = ap.Keterangan,
                        Remarks = (string.IsNullOrEmpty(ap.Remarks) ? ap.Keterangan : ap.Remarks),
                        JumlahAwal = ap.SldSisa,
                        SudahBayar = sudahBayar,
                        Sisa = ap.Sisa,
                        JumlahCicilan = jumlahCicilan,
                        TglTerakhirBayar = tglTerakhirBayar,
                        HariSejakTerakhirBayar = hariSejakBayar,
                        Jumlah = (currentDate < duedate ? ap.Sisa : 0),
                        Jumlah1 = (currentDate >= duedate && currentDate <= date1 ? ap.Sisa : 0),
                        Jumlah2 = (currentDate > date1 && currentDate <= date2 ? ap.Sisa : 0),
                        Jumlah3 = (currentDate > date2 && currentDate <= date3 ? ap.Sisa : 0),
                        Jumlah4 = (currentDate > date3 ? ap.Sisa : 0),
                    });
                }
            }
            return transaksi;

        }

        #region prosesPiutang
       
        public async Task ProsesPiutang()
        {

            using var db = _context.CreateDbContext();
            List<ArCust> Customers = db.ArCusts.ToList();
            List<ArPiutng> Piutangs = db.ArPiutngs.ToList();

            List<ArTransH> TransPiutang = new List<ArTransH>();


            Customers.ForEach(i => { i.Piutang = 0; });


            Customers.ForEach(i => { i.Piutang = i.SldAwal; });
            foreach (var piutang in Piutangs)
            {
                if (piutang.Kode == "OE" || piutang.Kode == "IN")
                {
                    var customer = Customers.Find(x => x.Customer == piutang.Customer);

                    if(customer is not null)
                    {
                        
                        customer.Piutang += piutang.Jumlah;
                        customer.LstOrder = piutang.Tanggal;
                    }
                   
                }
            }

            Piutangs.ForEach(i => { i.Bayar = i.SldBayar; i.Discount = i.SldDisc; i.Sisa = i.SldSisa; });

            foreach (var piutang in Piutangs)
            {

                Customers.Find(x => x.Customer == piutang.Customer).Piutang -= (piutang.KodeTran == "13" ? -1 * piutang.SldSisa : piutang.SldBayar);

            }


            

            TransPiutang = db.ArTransHs.OrderBy(x => x.Tanggal).Include(x => x.ArTransDs).Where(x => x.Kode != "11").ToList();


            foreach (var trans in TransPiutang)
            {

                decimal mPayee = 0;
                decimal mDiskon = 0;

                var transdetail = trans.ArTransDs;

                if (transdetail != null)
                {
                    foreach (var transdetails in transdetail)
                    {
                        var piutang = Piutangs.Find(x => x.Dokumen == transdetails.Lpb);

                        // if (transdetails.KodeTran != "14")
                        // {

                        piutang.Bayar += (transdetails.Bayar + transdetails.Discount);
                        piutang.Discount += transdetails.Discount;

                        // }


                        piutang.Sisa -= (transdetails.Bayar + transdetails.Discount);

                        mPayee += transdetails.Bayar;
                        mDiskon += transdetails.Discount;

                    }
                }

                trans.Unapplied = trans.Jumlah - mPayee;
                trans.Discount = mDiskon;

                var piutangBukti = Piutangs.Find(x => x.Dokumen == trans.Bukti);

                piutangBukti.Jumlah = (trans.Kode == "13" ? -1 * trans.Jumlah : -1 * trans.Piutang);
                piutangBukti.UnApplied = -1 * trans.Unapplied;


                if (trans.Kode != "13")
                {
                    piutangBukti.Bayar = -1 * trans.Jumlah;
                    piutangBukti.Sisa = -1 * trans.Unapplied;
                    piutangBukti.Discount = -1 * trans.Discount;
                    Customers.Find(x => x.Customer == trans.Customer).Piutang -= (trans.Kode == "13" ? -1 * (trans.Jumlah) : (trans.Piutang + trans.Unapplied));
                }




            }



            db.UpdateRange(Customers);
            db.UpdateRange(Piutangs);



           await db.SaveChangesAsync();


            // return Transaksi;

        }
        #endregion

        #region remarks aging

        public List<ArAgingView> GetRemarksSchedule()
        {
            List<ArPiutng> trans = new List<ArPiutng>();
            List<ArAgingView> transaksi = new List<ArAgingView>();
            using var db = _context.CreateDbContext();
            List<ArCust> supplier = db.ArCusts.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;
            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            //trans = _context.ArPiutngs.Where(x => x.Kode != "CA" && (x.Sisa != 0)).OrderBy(x => x.Customer).ToList();
            trans = db.ArPiutngs.Where(x => (x.Sisa != 0)).OrderBy(x => x.Customer).ToList();

            foreach (var ap in trans)
            {
                duedate = ap.DueDate ?? ap.Tanggal;
                date1 = duedate.AddMonths(1);
                date2 = duedate.AddMonths(2);
                date3 = duedate.AddMonths(3);


                transaksi.Add(new ArAgingView()
                {
                    Kode = ap.Kode,
                    ArAgingId = ap.ArPiutngId,
                    Customer = ap.Customer,
                    Tanggal = ap.Tanggal,
                    Dokumen = ap.Dokumen,
                    Duedate = duedate,
                    NamaCust = (from e in supplier where e.Customer == ap.Customer select e.NamaCust).FirstOrDefault(),
                    Keterangan = ap.Keterangan,
                    Remarks = ap.Remarks,
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
        public void SimpanRemarks(string dokumen, string remarks)
        {
            using (var ctx = _context.CreateDbContext())
            {
                var piutang = ctx.ArPiutngs.Where(x => x.Dokumen == dokumen).FirstOrDefault();
                piutang.Remarks = remarks;

                ctx.ArPiutngs.Update(piutang);
                ctx.SaveChanges();
            }

        }


        #endregion

        #region agingpiutangdetail
        public List<ArAgingDetailView> GetAgingDetailView()
        {
            using var db = _context.CreateDbContext();
            var piutangs = db.ArPiutngs
                .AsNoTracking()
                .Where(p => p.Sisa != 0) // hanya yang belum lunas
                .ToList();

            var customers = db.ArCusts.AsNoTracking().ToList();
            var cicilanAll = db.ArTransDs.AsNoTracking().ToList();

            var list = new List<ArAgingDetailView>();

            foreach (var p in piutangs)
            {
              //  var dueDate = p.Tanggal;

                var view = new ArAgingDetailView
                {
                    Kode = p.Kode,
                    Customer = p.Customer,
                    NamaCust = customers.FirstOrDefault(c => c.Customer == p.Customer)?.NamaCust,
                    Dokumen = p.Dokumen,
                    Tanggal = p.Tanggal,
                 //   DueDate = dueDate,
                    Salesman = p.Salesman,
                    Keterangan = p.Keterangan,
                    Remarks = string.IsNullOrEmpty(p.Remarks) ? p.Keterangan : p.Remarks,
                    Sisa = p.Sisa,
                    Total = p.SldSisa,
                    Cicilan = p.Sisa != p.SldSisa,
                    CicilanList = cicilanAll
                        .Where(c => c.Lpb == p.Dokumen)
                        .OrderBy(c => c.Tanggal)
                        .Select(c => new CicilanDetail
                        {
                            Tanggal = c.Tanggal,
                            Jumlah = c.Jumlah,
                            Bayar = c.Bayar
                        }).ToList()
                };

                list.Add(view);
            }

            return list;
        }



        #endregion

        public List<ArForecastPiutangView> GetForecastPiutang(DateTime tanggalMulai, int jumlahBulan = 12)
        {
            var tanggalAkhir = tanggalMulai.AddMonths(jumlahBulan);

            using var db = _context.CreateDbContext();

            // Ambil piutang yang belum lunas
            var piutangBelumLunas = db.ArPiutngs
                .AsNoTracking()
                .Where(x => x.Sisa > 0)
                .ToList()
                .Where(x =>
                {
                    var dueDate = x.DueDate ?? x.Tanggal;
                    return dueDate >= tanggalMulai && dueDate < tanggalAkhir;
                })
                .OrderBy(x => x.DueDate ?? x.Tanggal)
                .ToList();

            // Load customer untuk lookup nama
            var customerIds = piutangBelumLunas.Select(x => x.Customer).Distinct().ToList();
            var customers = db.ArCusts
                .AsNoTracking()
                .Where(x => customerIds.Contains(x.Customer))
                .ToDictionary(x => x.Customer, x => x.NamaCust);

            var forecast = new List<ArForecastPiutangView>();

            for (int i = 0; i < jumlahBulan; i++)
            {
                var bulan = tanggalMulai.AddMonths(i);
                var bulanInt = bulan.Month;
                var tahunInt = bulan.Year;

                var piutangBulan = piutangBelumLunas
                    .Where(x =>
                    {
                        var dueDate = x.DueDate ?? x.Tanggal;
                        return dueDate.Year == tahunInt && dueDate.Month == bulanInt;
                    })
                    .ToList();

                forecast.Add(new ArForecastPiutangView
                {
                    Bulan = bulanInt,
                    Tahun = tahunInt,
                    NamaBulan = new DateTime(tahunInt, bulanInt, 1).ToString("MMMM yyyy",
                        new System.Globalization.CultureInfo("id-ID")),
                    TotalTagihan = piutangBulan.Sum(x => x.Sisa),
                    JumlahDokumen = piutangBulan.Count,
                    Details = piutangBulan.Select(x => new ArPiutangForecastDetail
                    {
                        Dokumen = x.Dokumen,
                        Tanggal = x.Tanggal,
                        DueDate = x.DueDate ?? x.Tanggal,
                        Customer = x.Customer,
                        NamaCust = customers.TryGetValue(x.Customer, out var nama) ? nama : x.Customer,
                        Jumlah = x.Jumlah,
                        Bayar = x.Bayar,
                        Sisa = x.Sisa,
                        Keterangan = x.Keterangan,
                        Salesman = x.Salesman
                    }).ToList()
                });
            }

            return forecast;
        }

        public List<ArTransD> GetPembayaranByDokumen(string dokumen)
        {
            using var db = _context.CreateDbContext();
            return db.ArTransDs
                .AsNoTracking()
                .Where(x => x.Lpb == dokumen)
                .OrderBy(x => x.Tanggal)
                .ToList();
        }

        public List<ArCustomerAnalysisView> GetCustomerAnalysis()
        {
            var today = DateTime.Today;

            // 1 query untuk semua piutang — outstanding dihitung dari memori
            using var db = _context.CreateDbContext();
            var semuaPiutang = db.ArPiutngs
                .AsNoTracking()
                .Where(x => x.Kode != "CA")
                .ToList();

            // Load ArTransD tanpa Include — hanya ambil kolom yang dibutuhkan
            var semuaBayarRaw = db.ArTransDs
                .AsNoTracking()
                .Where(x => x.Bayar > 0 || x.Discount > 0)
                .Select(x => new { x.ArTransHId, x.Lpb, x.Bayar, x.Discount })
                .ToList();

            // Load ArTransH sebagai dictionary (id → tanggal)
            var transHIds = semuaBayarRaw.Select(x => x.ArTransHId).Distinct().ToList();
            var transHDict = db.ArTransHs
                .AsNoTracking()
                .Where(x => transHIds.Contains(x.ArTransHId))
                .Select(x => new { x.ArTransHId, x.Tanggal })
                .ToDictionary(x => x.ArTransHId, x => x.Tanggal);

            // Gabungkan bayar + tanggal di memori
            var semuaBayar = semuaBayarRaw
                .Select(x => new
                {
                    x.Lpb,
                    x.Bayar,
                    x.Discount,
                    Tanggal = transHDict.TryGetValue(x.ArTransHId, out var tgl) ? (DateTime?)tgl : null
                })
                .Where(x => x.Tanggal != null)
                .OrderBy(x => x.Tanggal)
                .ToList();

            // Outstanding dihitung dari semuaPiutang (tidak perlu query ulang)
            var outstanding = semuaPiutang.Where(x => x.Sisa != 0).ToList();

            var customers = db.ArCusts.AsNoTracking()
                .Select(x => new { x.Customer, x.NamaCust })
                .ToList();

            // Kelompokkan piutang per customer
            var perCustomer = semuaPiutang
                .GroupBy(x => x.Customer)
                .ToList();

            var result = new List<ArCustomerAnalysisView>();

            foreach (var grp in perCustomer)
            {
                var custCode = grp.Key;
                var custInfo = customers.FirstOrDefault(x => x.Customer == custCode);
                var fakturCust = grp.ToList();

                // Pembayaran untuk customer ini (lewat Lpb yang ada di daftar faktur customer ini)
                var dokumenCust = new HashSet<string>(fakturCust.Select(x => x.Dokumen));
                var bayarCust = semuaBayar
                    .Where(x => dokumenCust.Contains(x.Lpb))
                    .ToList();

                // Outstanding
                var outstandingCust = outstanding.Where(x => x.Customer == custCode).ToList();
                decimal totalOutstanding = outstandingCust.Sum(x => x.Sisa);
                int fakturOpen = outstandingCust.Count;

                int totalFaktur = fakturCust.Count;
                decimal totalNilai = fakturCust.Sum(x => x.SldSisa); // nilai awal semua faktur

                // Faktur lunas: sisa = 0
                var fakturLunas = fakturCust.Where(x => x.Sisa == 0 && x.SldSisa > 0).ToList();
                int jumlahLunas = fakturLunas.Count;

                // ── HISTORI: dari faktur yang sudah lunas ─────────────────────
                var daysLateList  = new List<int>();
                var daysBayarList = new List<int>();

                foreach (var faktur in fakturLunas)
                {
                    var dueDate    = faktur.DueDate ?? faktur.Tanggal;
                    var bayarFaktur = bayarCust
                        .Where(x => x.Lpb == faktur.Dokumen)
                        .OrderBy(x => x.Tanggal)
                        .LastOrDefault();

                    if (bayarFaktur?.Tanggal != null)
                    {
                        var tglBayar      = bayarFaktur.Tanggal.Value.Date;
                        var hariDariFaktur = (int)(tglBayar - faktur.Tanggal.Date).TotalDays;
                        var hariTerlambat  = (int)(tglBayar - dueDate.Date).TotalDays;

                        daysBayarList.Add(Math.Max(0, hariDariFaktur));
                        daysLateList.Add(Math.Max(0, hariTerlambat));
                    }
                }

                double avgHariBayar = daysBayarList.Any() ? daysBayarList.Average() : 0;
                double avgDaysLate  = daysLateList.Any()  ? daysLateList.Average()  : 0;
                int    maxDaysLate  = daysLateList.Any()  ? daysLateList.Max()       : 0;

                int onTimeCount = daysLateList.Count(x => x == 0);
                double onTimeRate = jumlahLunas > 0 ? (double)onTimeCount / jumlahLunas * 100.0 : 0;

                // ── OUTSTANDING: penentu utama label risiko ───────────────────
                // Untuk setiap faktur open, hitung berapa hari sudah "diam":
                //   - jika pernah ada cicilan → today - tanggal_cicilan_terakhir
                //   - jika belum pernah bayar → today - due_date (kalau sudah jatuh tempo)
                int    maxHariMacet        = 0;
                int    countTelat60        = 0;
                int    jumlahFakturCicilan = 0;
                decimal outstandingTelat60 = 0;
                DateTime? tglTerakhirBayarOutstanding = null;

                foreach (var faktur in outstandingCust)
                {
                    var dueDate = faktur.DueDate ?? faktur.Tanggal;

                    // Pembayaran terakhir untuk faktur ini
                    var bayarTerakhir = bayarCust
                        .Where(x => x.Lpb == faktur.Dokumen)
                        .OrderByDescending(x => x.Tanggal)
                        .FirstOrDefault();

                    int hariDiam;
                    if (bayarTerakhir?.Tanggal != null)
                    {
                        // Ada cicilan — hitung dari kapan terakhir bayar
                        hariDiam = (int)(today - bayarTerakhir.Tanggal.Value.Date).TotalDays;
                        jumlahFakturCicilan++;

                        if (tglTerakhirBayarOutstanding == null || bayarTerakhir.Tanggal.Value > tglTerakhirBayarOutstanding.Value)
                            tglTerakhirBayarOutstanding = bayarTerakhir.Tanggal.Value;
                    }
                    else
                    {
                        // Belum pernah bayar sama sekali — hitung dari due date
                        hariDiam = (int)(today - dueDate.Date).TotalDays;
                        if (hariDiam < 0) hariDiam = 0; // belum jatuh tempo
                    }

                    if (hariDiam > maxHariMacet) maxHariMacet = hariDiam;

                    if (hariDiam > 60)
                    {
                        countTelat60++;
                        outstandingTelat60 += faktur.Sisa;
                    }
                }

                // ── DSO ───────────────────────────────────────────────────────
                var cutoff12bln = today.AddMonths(-12);
                decimal penjualan12bln = fakturCust
                    .Where(x => x.Tanggal >= cutoff12bln && x.SldSisa > 0)
                    .Sum(x => x.SldSisa);
                double dso = penjualan12bln > 0
                    ? (double)totalOutstanding / (double)penjualan12bln * 365.0
                    : 0;

                // ── PENENTUAN LABEL RISIKO ────────────────────────────────────
                // Aturan UTAMA: berdasarkan kondisi outstanding saat ini.
                // Jika tidak ada outstanding → berdasarkan histori bayar.
                string riskLabel, rekomendasi;
                int    riskScore;

                bool adaOutstanding = outstandingCust.Any();

                if (adaOutstanding)
                {
                    // Ada piutang yang belum lunas — nilai dari maxHariMacet
                    if (maxHariMacet > 60)
                    {
                        // Sudah >60 hari diam (tidak bayar / cicilan berhenti) → Blacklist
                        riskLabel   = "Blacklist";
                        rekomendasi = $"⛔ Ada piutang yang {maxHariMacet} hari tidak ada pembayaran. Hentikan kredit. Minta pelunasan + jaminan sebelum transaksi baru.";
                        riskScore   = 5;
                    }
                    else if (maxHariMacet > 30)
                    {
                        // 31-60 hari diam → Macet
                        riskLabel   = "Macet";
                        rekomendasi = $"🔴 Ada piutang yang {maxHariMacet} hari tidak ada pembayaran. Tahan pengiriman. Minta komitmen bayar segera.";
                        riskScore   = 20;
                    }
                    else if (maxHariMacet > 0)
                    {
                        // ≤30 hari, masih dalam batas atau baru jatuh tempo → Cukup / Jelek tergantung histori
                        if (avgDaysLate > 30 || maxDaysLate > 60)
                        {
                            riskLabel   = "Jelek";
                            rekomendasi = "🟠 Masih aktif bayar tapi histori sering terlambat. Perketat termin, minta DP lebih besar.";
                            riskScore   = 35;
                        }
                        else
                        {
                            riskLabel   = "Cukup";
                            rekomendasi = "⚠️ Ada outstanding, masih aktif bayar. Monitor setiap bulan. Jangan tambah limit dulu.";
                            riskScore   = 55;
                        }
                    }
                    else
                    {
                        // Faktur open tapi belum jatuh tempo → Baik/Cukup dari histori
                        if (avgDaysLate > 30 || maxDaysLate > 60)
                        {
                            riskLabel   = "Hati-hati";
                            rekomendasi = "⚠️ Belum jatuh tempo, tapi histori bayar pernah terlambat >30-60 hari. Pantau ketat.";
                            riskScore   = 50;
                        }
                        else
                        {
                            riskLabel   = "Baik";
                            rekomendasi = "✅ Outstanding belum jatuh tempo & histori bayar baik. Dapat dipercaya.";
                            riskScore   = 80;
                        }
                    }
                }
                else
                {
                    // Semua sudah lunas — nilai murni dari histori
                    if (maxDaysLate > 60 || avgDaysLate > 45)
                    {
                        riskLabel   = "Hati-hati";
                        rekomendasi = "⚠️ Semua lunas, tapi pernah terlambat >60 hari. Setujui kredit dengan syarat ketat.";
                        riskScore   = 50;
                    }
                    else if (avgDaysLate > 15)
                    {
                        riskLabel   = "Cukup";
                        rekomendasi = "⚠️ Semua lunas, rata-rata agak telat. Monitor jika ada transaksi baru.";
                        riskScore   = 65;
                    }
                    else
                    {
                        riskLabel   = "Baik";
                        rekomendasi = "✅ Semua lunas & histori bayar baik. Dapat dipercaya.";
                        riskScore   = 90;
                    }
                }

                // Tidak ada histori sama sekali → netral
                if (totalFaktur == 0) { riskScore = 50; riskLabel = "Baru"; rekomendasi = "🔵 Belum ada histori transaksi."; }

                result.Add(new ArCustomerAnalysisView
                {
                    Customer                   = custCode,
                    NamaCust                   = custInfo?.NamaCust ?? custCode,
                    Salesman                   = fakturCust.LastOrDefault()?.Salesman ?? "",
                    TotalOutstanding           = totalOutstanding,
                    JumlahFakturOpen           = fakturOpen,
                    JumlahFakturCicilan        = jumlahFakturCicilan,
                    CountTelat60               = countTelat60,
                    OutstandingTelat60         = outstandingTelat60,
                    MaxHariMacetOutstanding    = maxHariMacet,
                    TglTerakhirBayarOutstanding= tglTerakhirBayarOutstanding,
                    TotalFaktur                = totalFaktur,
                    FakturLunas                = jumlahLunas,
                    TotalNilaiTransaksi        = totalNilai,
                    AvgHariBayar               = Math.Round(avgHariBayar, 1),
                    AvgDaysLate                = Math.Round(avgDaysLate, 1),
                    MaxDaysLate                = maxDaysLate,
                    OnTimeRate                 = Math.Round(onTimeRate, 1),
                    DSO                        = Math.Round(dso, 1),
                    RiskScore                  = riskScore,
                    RiskLabel                  = riskLabel,
                    Rekomendasi                = rekomendasi,
                });
            }

            return result.OrderBy(x => x.RiskScore).ToList(); // terburuk di atas
        }

        public List<ArMonthlyCollectionView> GetMonthlyCollection(int bulanKebelakang = 12)
        {
            var cutoff = DateTime.Today.AddMonths(-bulanKebelakang).Date;

            using var db = _context.CreateDbContext();

            // Load header yang relevan
            var headers = db.ArTransHs
                .AsNoTracking()
                .Where(x => x.Tanggal >= cutoff)
                .Select(x => new { x.ArTransHId, x.Tanggal })
                .ToDictionary(x => x.ArTransHId, x => x.Tanggal);

            var hIds = headers.Keys.ToList();

            // Load detail bayar
            var details = db.ArTransDs
                .AsNoTracking()
                .Where(x => hIds.Contains(x.ArTransHId) && (x.Bayar > 0 || x.Discount > 0))
                .Select(x => new { x.ArTransHId, x.Lpb, x.Bayar, x.Discount })
                .ToList();

            // Grup per bulan
            var grouped = details
                .Select(x => new
                {
                    x.Lpb,
                    x.Bayar,
                    x.Discount,
                    Tgl = headers.TryGetValue(x.ArTransHId, out var t) ? t : (DateTime?)null
                })
                .Where(x => x.Tgl != null)
                .GroupBy(x => new { x.Tgl.Value.Year, x.Tgl.Value.Month })
                .Select(g => new ArMonthlyCollectionView
                {
                    Tahun          = g.Key.Year,
                    Bulan          = g.Key.Month,
                    TotalBayar     = g.Sum(x => x.Bayar),
                    TotalDiscount  = g.Sum(x => x.Discount),
                    JumlahFaktur   = g.Select(x => x.Lpb).Distinct().Count(),
                    JumlahCustomer = 0 // akan di-enrich jika diperlukan
                })
                .OrderBy(x => x.Tahun).ThenBy(x => x.Bulan)
                .ToList();

            return grouped;
        }
    }
}
