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
        private readonly DbContextPiutang _context;

        public ReceivableServices(DbContextPiutang context)
        {
            _context = context;
        }

        public bool CekKdCustomer(string customer)
        {
            string test = customer.ToUpper();
            var cekFirst = _context.ArCusts.Where(x => x.Customer == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }


        public List<ArCust> GetCustomer()
        {
            return _context.ArCusts.OrderBy(x => x.NamaCust).ToList();
        }

        public ArCust GetCustomerId(int id)
        {
            return _context.ArCusts.Where(x => x.ArCustId == id).FirstOrDefault();
        }

        public ArCust GetCustomerCode(string xKode)
        {
            return _context.ArCusts.Where(x => x.Customer == xKode).FirstOrDefault();
        }

        public bool AddCustomer(CustomerView customers)
        {
            string test = customers.Customer.ToUpper();
            var cekFirst = _context.ArCusts.Where(x => x.Customer == test).ToList();
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
                _context.ArCusts.Add(Customer);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditCustomer(CustomerView customers)
        {
            try
            {
                var ExistingCustomer = _context.ArCusts.Where(x => x.ArCustId == customers.ArCustId).FirstOrDefault();
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

                    _context.ArCusts.Update(ExistingCustomer);
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

        public async Task<bool> DelCustomer(int customers)
        {
            try
            {
                var ExistingCustomer = _context.ArCusts.Where(x => x.ArCustId == customers).FirstOrDefault();
                if (ExistingCustomer != null)
                {
                    _context.ArCusts.Remove(ExistingCustomer);
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

        #region ArAcct Class

        public bool CekAcctSet(string customer)
        {
            string test = customer.ToUpper();
            var cekFirst = _context.ArAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }


        public List<ArAcct> GetArAkunSet()
        {
            return _context.ArAccts.ToList();
        }

        public ArAcct GetArAkunSetId(int id)
        {
            return _context.ArAccts.Where(x => x.ArAcctId == id).FirstOrDefault();
        }

        public bool AddAkunSet(ArAcctView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
            var cekFirst = _context.ArAccts.Where(x => x.AcctSet == test).ToList();
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
                _context.ArAccts.Add(AcctCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditAkunSet(ArAcctView codeview)
        {
            try
            {
                var ExistingAkunSet = _context.ArAccts.Where(x => x.ArAcctId == codeview.ArAcctId).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.Description = codeview.Description;
                    ExistingAkunSet.Acct1 = codeview.Acct1;
                    ExistingAkunSet.Acct2 = codeview.Acct2;
                    ExistingAkunSet.Acct3 = codeview.Acct3;
                    ExistingAkunSet.Acct4 = codeview.Acct4;
                    ExistingAkunSet.Acct5 = codeview.Acct5;
                    ExistingAkunSet.Acct6 = codeview.Acct6;
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
                var ExistingAkunSet = _context.ArAccts.Where(x => x.ArAcctId == codeview).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    _context.ArAccts.Remove(ExistingAkunSet);
                    await _context.SaveChangesAsync();
                    return true;
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
            var cekFirst = _context.ArDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }


        public List<ArDist> GetDist()
        {
            return _context.ArDists.ToList();
        }

        public ArDist GetDistId(int id)
        {
            return _context.ArDists.Where(x => x.ArDistId == id).FirstOrDefault();
        }

        public bool AddDist(ArDistView codeview)
        {
            string test = codeview.DistCode.ToUpper();
            var cekFirst = _context.ArDists.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                ArDist AcctCode = new ArDist()
                {
                    DistCode = codeview.DistCode.ToUpper(),
                    Description = codeview.Description,
                    Dist1 = codeview.Dist1

                };
                _context.ArDists.Add(AcctCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditDist(ArDistView codeview)
        {
            try
            {
                var ExistingDist = _context.ArDists.Where(x => x.ArDistId == codeview.ArDistId).FirstOrDefault();
                if (ExistingDist != null)
                {
                    ExistingDist.Description = codeview.Description;
                    ExistingDist.Dist1 = codeview.Dist1;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception) { throw; }

            return false;

        }

        public async Task<bool> DelDist(int codeview)
        {
            try
            {
                var ExistingDist = _context.ArDists.Where(x => x.ArDistId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    _context.ArDists.Remove(ExistingDist);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception) { throw; }

            return false;

        }
        #endregion ArDist Class



        #region Transaksi Piutang Class

        public ArTransH GetTrans(int id)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.ArTransHId == id).FirstOrDefault();
        }

        public ArPiutng GetPiutang(string bukti)
        {
            return _context.ArPiutngs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }


        public List<ArTransH> GetTransH()
        {
            List<ArTransH> arTrans = new List<ArTransH>();
            try
            {
                arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "11").ToList();
                foreach (var item in arTrans)
                {
                    item.NamaCust = (from e in _context.ArCusts where e.ArCustId == item.ArCustId select e.NamaCust).FirstOrDefault();
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
            arTrans = _context.ArTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "11")
                .Select(x => new ArTransH
                {
                    ArTransHId = x.ArTransHId,
                    Bukti = x.Bukti,
                    Tanggal = x.Tanggal,
                    Keterangan = x.Keterangan,
                    Customer = x.Customer,
                    NamaCust = (from e in _context.ArCusts where e.Customer == x.Customer select e.NamaCust).SingleOrDefault(),
                    Jumlah = x.Jumlah,
                })
                .ToList();
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
            return _context.ArTransDs.ToList();
        }
        public bool BatalPiutang(ArPiutng piutang)
        {
            _context.ArPiutngs.Remove(piutang);
            _context.SaveChanges();

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

            var customer = (from e in _context.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();
            customer.Piutang += trans.Jumlah;

            _context.ArCusts.Update(customer);
            _context.ArTransHs.Add(transH);
            _context.ArPiutngs.Add(transaksi);
            _context.SaveChanges();

            var TempTrans = GetTransDoc(transH.Bukti);

            return TempTrans;


        }

        public ArTransH EditTransH(ArTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == trans.Bukti).FirstOrDefault();


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


            var ExistingTrans = _context.ArTransHs.Where(x => x.ArTransHId == trans.ArTransHId).FirstOrDefault();
            if (ExistingTrans != null)
            {
                transH.Bukti = ExistingTrans.Bukti;
                transaksi.Dokumen = ExistingTrans.Bukti;

                _context.ArTransHs.Remove(ExistingTrans);

                var customer = (from e in _context.ArCusts where e.Customer == trans.Customer select e).FirstOrDefault();

                customer.Piutang -= ExistingTrans.Jumlah;
                customer.Piutang += trans.Jumlah;

                _context.ArCusts.Update(customer);
                _context.ArPiutngs.Remove(cekFirst);

                _context.ArTransHs.Add(transH);
                _context.ArPiutngs.Add(transaksi);
                _context.SaveChanges();

                var TempTrans = GetTransDoc(transH.Bukti);

                return TempTrans;

            }
            else
            {
                return ExistingTrans;
            }


            // return false;


        }
        public bool CekJual(string noLpb)
        {
            var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == noLpb && x.Bayar == 0).FirstOrDefault();

            if (cekFirst != null)
                return true;

            return false;
        }
        public bool CekAlreadyPayment(string dokumen)
        {
            var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == dokumen).FirstOrDefault();

            if (cekFirst.SldSisa != cekFirst.Sisa)
            {
                return true;
            }
            return false;
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

        public ArTransH GetTransDoc(string docno)
        {
            return _context.ArTransHs.Include(p => p.ArTransDs).Where(x => x.Bukti == docno).FirstOrDefault();
        }

        #endregion Transaksi Piutang Class

        #region laporan piutang

        public List<ArPiutng> Detail1(string xKdHeader)
        {

            List<ArPiutng> trans = new List<ArPiutng>();

            // trans = _context.ArPiutngs.Where(x => x.Customer == xKdHeader && (x.Sisa != 0)).ToList();
            trans = _context.ArPiutngs.Where(x => x.Customer == xKdHeader).ToList();
            return trans;
        }

        public List<ArPiutngView> GetUangMuka()
        {
            List<ArPiutngView> transView = new List<ArPiutngView>();
            var trans = _context.ArPiutngs.Where(x => x.Kode == "CA" && x.KodeTran == "13" && x.Sisa != 0).ToList();
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

        public List<ArAgingView> GetAgingSchedule()
        {
            List<ArPiutng> trans = new List<ArPiutng>();
            List<ArAgingView> transaksi = new List<ArAgingView>();

            List<ArCust> supplier = _context.ArCusts.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;

            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            trans = _context.ArPiutngs
                .AsNoTracking()
                .Where(x => x.Sisa != 0)
                .OrderBy(x => x.Customer)
                .ThenByDescending(x => x.Dokumen)
                .ToList();

            // Ambil semua pembayaran (ArTransD) sekaligus, join ke ArTransH untuk tanggal
            var semuaPembayaran = _context.ArTransDs
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
            return transaksi;

        }

        #region prosesPiutang
       
        public async Task ProsesPiutang()
        {

            List<ArCust> Customers = _context.ArCusts.ToList();
            List<ArPiutng> Piutangs = _context.ArPiutngs.ToList();

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


            TransPiutang = _context.ArTransHs.OrderBy(x => x.Tanggal).Include(x => x.ArTransDs).Where(x => x.Kode != "11").ToList();


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



            _context.UpdateRange(Customers);
            _context.UpdateRange(Piutangs);



           await _context.SaveChangesAsync();


            // return Transaksi;

        }
        #endregion

        #region remarks aging

        public List<ArAgingView> GetRemarksSchedule()
        {
            List<ArPiutng> trans = new List<ArPiutng>();
            List<ArAgingView> transaksi = new List<ArAgingView>();

            List<ArCust> supplier = _context.ArCusts.ToList();

            DateTime duedate = DateTime.Today.Date;

            DateTime currentDate = DateTime.Today.Date;
            DateTime date1 = currentDate.AddMonths(1);
            DateTime date2 = currentDate.AddMonths(2);
            DateTime date3 = currentDate.AddMonths(3);

            //trans = _context.ArPiutngs.Where(x => x.Kode != "CA" && (x.Sisa != 0)).OrderBy(x => x.Customer).ToList();
            trans = _context.ArPiutngs.Where(x => (x.Sisa != 0)).OrderBy(x => x.Customer).ToList();

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
            var piutang = _context.ArPiutngs.Where(x => x.Dokumen == dokumen).FirstOrDefault();
            piutang.Remarks = remarks;

            _context.ArPiutngs.Update(piutang);
            _context.SaveChanges();

        }


        #endregion

        #region agingpiutangdetail
        public List<ArAgingDetailView> GetAgingDetailView()
        {
            var piutangs = _context.ArPiutngs
                .AsNoTracking()
                .Where(p => p.Sisa != 0) // hanya yang belum lunas
                .ToList();

            var customers = _context.ArCusts.AsNoTracking().ToList();
            var cicilanAll = _context.ArTransDs.AsNoTracking().ToList();

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

            // Ambil piutang yang belum lunas
            var piutangBelumLunas = _context.ArPiutngs
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
            var customers = _context.ArCusts
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
            return _context.ArTransDs
                .AsNoTracking()
                .Where(x => x.Lpb == dokumen)
                .OrderBy(x => x.Tanggal)
                .ToList();
        }

        public List<ArCustomerAnalysisView> GetCustomerAnalysis()
        {
            var today = DateTime.Today;

            // 1 query untuk semua piutang — outstanding dihitung dari memori
            var semuaPiutang = _context.ArPiutngs
                .AsNoTracking()
                .Where(x => x.Kode != "CA")
                .ToList();

            // Load ArTransD tanpa Include — hanya ambil kolom yang dibutuhkan
            var semuaBayarRaw = _context.ArTransDs
                .AsNoTracking()
                .Where(x => x.Bayar > 0 || x.Discount > 0)
                .Select(x => new { x.ArTransHId, x.Lpb, x.Bayar, x.Discount })
                .ToList();

            // Load ArTransH sebagai dictionary (id → tanggal)
            var transHIds = semuaBayarRaw.Select(x => x.ArTransHId).Distinct().ToList();
            var transHDict = _context.ArTransHs
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

            var customers = _context.ArCusts.AsNoTracking()
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

            // Load header yang relevan
            var headers = _context.ArTransHs
                .AsNoTracking()
                .Where(x => x.Tanggal >= cutoff)
                .Select(x => new { x.ArTransHId, x.Tanggal })
                .ToDictionary(x => x.ArTransHId, x => x.Tanggal);

            var hIds = headers.Keys.ToList();

            // Load detail bayar
            var details = _context.ArTransDs
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

        public async Task<bool> UpdateArPiutangWithPaymentAsync(string dokumen, decimal bayar, decimal discount)
        {
            try
            {
                var piutang = await _context.ArPiutngs.FirstOrDefaultAsync(p => p.Dokumen == dokumen);
                if (piutang == null) return false;

                piutang.Bayar += bayar;
                piutang.Discount += discount;
                piutang.Sisa = Math.Max(0, piutang.Jumlah - piutang.Bayar - piutang.Discount);

                _context.ArPiutngs.Update(piutang);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ArTransH> CreateArPaymentTransactionAsync(
            DateTime tanggal,
            string kdBank,
            string customer,
            string keterangan,
            List<(string dokumen, decimal bayar, decimal discount)> allocations)
        {
            try
            {
                // Generate Bukti number for payment: BMY-yy3MM-XXXXX
                string buktiNo = GetNumberPayment();

                // Get customer info
                var customerInfo = await _context.ArCusts.FirstOrDefaultAsync(c => c.Customer == customer);

                // Create ArTransH header with Kode = "14" for payment transactions (from BMA-PT)
                var transH = new ArTransH
                {
                    Bukti = buktiNo,
                    Kode = "14",  // Payment code for AR (from BMA-PT)
                    Tanggal = tanggal,
                    KdBank = kdBank,
                    Customer = customer,
                    Keterangan = keterangan,
                    ArCustId = customerInfo?.ArCustId ?? 0,
                    NamaCust = customerInfo?.NamaCust ?? customer,
                    ArTransDs = new List<ArTransD>()
                };

                decimal totalBayar = 0;
                decimal totalDiscount = 0;

                // Create ArTransD details for each allocation
                foreach (var alloc in allocations)
                {
                    // Get the outstanding doc info
                    var piutangItem = await _context.ArPiutngs.FirstOrDefaultAsync(p => p.Dokumen == alloc.dokumen);
                    if (piutangItem == null) continue;

                    var transD = new ArTransD
                    {
                        Bukti = buktiNo,
                        Tanggal = tanggal,
                        DueDate = piutangItem.Tanggal,
                        Kode = "14",
                        KodeTran = "14",  // Payment transaction code (from BMA-PT)
                        Lpb = alloc.dokumen,
                        Jumlah = piutangItem.Sisa,
                        Bayar = alloc.bayar,
                        Discount = alloc.discount,
                        Sisa = Math.Max(0, piutangItem.Sisa - alloc.bayar - alloc.discount),
                        Keterangan = keterangan
                    };
                    transH.ArTransDs.Add(transD);

                    totalBayar += alloc.bayar;
                    totalDiscount += alloc.discount;

                    // Update ArPiutang balance
                    piutangItem.Bayar += alloc.bayar;
                    piutangItem.Discount += alloc.discount;
                    piutangItem.Sisa = Math.Max(0, piutangItem.Jumlah - piutangItem.Bayar - piutangItem.Discount);
                    _context.ArPiutngs.Update(piutangItem);
                }

                // Set header totals
                transH.Jumlah = totalBayar;
                transH.Discount = totalDiscount;
                transH.Piutang = totalBayar + totalDiscount;

                // Save transaction header and details
                _context.ArTransHs.Add(transH);
                await _context.SaveChangesAsync();

                return transH;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating AR payment transaction: {ex.Message}", ex);
            }
        }

        private string GetNumber()
        {
            // Generate invoice bukti number: ARI-yy2MM-XXXXX (for AddTransH)
            string kodeno = "ARI";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ArTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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
            // Generate payment bukti number: BMY-yy3MM-XXXXX (for bank payments)
            string kodeno = "BMY";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '3' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.ArTransHs.Where(x => x.Bukti.Substring(0, 10).Equals(xbukti)).ToList();
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

        private int GetCustomerIdByCode(string customerCode)
        {
            var customer = _context.ArCusts.FirstOrDefault(c => c.Customer == customerCode);
            return customer?.ArCustId ?? 0;
        }

        private string GetCustomerNameByCode(string customerCode)
        {
            var customer = _context.ArCusts.FirstOrDefault(c => c.Customer == customerCode);
            return customer?.NamaCust ?? customerCode;
        }
    }
}
