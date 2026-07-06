using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Model;
using eSoft.Pembelian.View;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Hutang.Model;
using eSoft.Hutang.View;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using eSoft.Hutang.Data;
using eSoft.CashBank.Data;
using eSoft.Ledger.Model;
using eSoft.Ledger.View;
using eSoft.Ledger.Data;
using eSoft.Financial.View;
using eSoft.Financial.Model;
using eSoft.Financial.Data;
using eSoft.Asset.View;
using eSoft.Asset.Model;
using eSoft.Asset.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace eSoft.Financial.Services
{
    public class FinancialServices : IFinancialServices
    {
        private readonly DbContextPersediaan _contextIC;
        private readonly DbContextBeli _contextIR;
        private readonly DbContextJual _contextOE;
        private readonly DbContextPiutang _contextAR;
        private readonly DbContextHutang _contextAP;
        private readonly DbContextBank _contextCB;
        private readonly DbContextLedger _contextGL;
        private readonly DbContextFinancial _contextFC;
        private readonly DbContextAssets _contextAS;

        public FinancialServices(DbContextPersediaan contextPersediaan,
                                 DbContextBeli contextBeli,
                                 DbContextJual contextJual,
                                 DbContextPiutang contextPiutang,
                                 DbContextHutang contextHutang,
                                 DbContextBank contextBank,
                                 DbContextLedger contextGL,
                                 DbContextAssets contextAS,
                                 DbContextFinancial contextFC)

        {
            _contextIC = contextPersediaan;
            _contextIR = contextBeli;
            _contextOE = contextJual;
            _contextAR = contextPiutang;
            _contextCB = contextBank;
            _contextAP = contextHutang;
            _contextGL = contextGL;
            _contextAS = contextAS;
            _contextFC = contextFC;
        }

        private string GetNameAccount(string kode)
        {
            return _contextGL.GlAccounts.Where(x => x.GlAcct == kode).FirstOrDefault().GlNama;
        }
        public CbSrcCode GetSrcCodeKd(string id)
        {
            return _contextCB.CbSrcCodes.Where(x => x.SrcCode == id).FirstOrDefault();
        }


        public List<FcLedgerView> CetakBukuBesar(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode)
        {

            List<FcLedgerView> Transaksi = new();


            var Rincian = from transH in _contextCB.CbTransHs
                          join transD in _contextCB.CbTransDs on transH.CbTransHId equals transD.CbTransHId
                          join srcCode in _contextCB.CbSrcCodes on transD.SrcCode equals srcCode.SrcCode

                          where (sourceCode.Contains(srcCode.GlAcct) && Tanggal1.Date <= transH.Tanggal.Date && transH.Tanggal.Date <= Tanggal2.Date)
                          select new FcLedgerView()
                          {
                              DocNo = transH.DocNo,
                              KodeBank = transH.KodeBank,
                              Tanggal = transH.Tanggal,
                              TipeGL = $"CB-{transH.KodeBank}",
                              Keterangan = transD.Keterangan,
                              SrcCode = transD.SrcCode,
                              GlAcct = srcCode.GlAcct,
                              Saldo = transD.Jumlah,
                              Balance = transH.Saldo
                          };

            string dokumen = "";
            FcLedgerView trans = new FcLedgerView();

            foreach (var item in Rincian)
            {
                if (item.DocNo != dokumen)
                {
                    Transaksi.Add(
                        new FcLedgerView()
                        {
                            KodeBank = item.KodeBank,
                            Tanggal = item.Tanggal,
                            Keterangan = item.Keterangan,
                            GlAcct = item.KodeBank,
                            DocNo = item.DocNo,
                            Balance = item.Balance,
                            Saldo = item.Saldo
                        });

                    dokumen = item.DocNo;

                }
                else
                {
                    Transaksi.Find(x => x.DocNo == item.DocNo).Saldo += item.Saldo;
                }

            }



            //var Rinci = Rincian.GroupBy(x => x.SrcCode)
            //     .Select(cl => new FcLedgerView
            //     {
            //         Keterangan = cl.First().Keterangan,
            //         SrcCode = cl.First().SrcCode,
            //         Saldo = cl.Sum(c => c.Saldo)
            //     }).ToList();

            //Transaksi.AddRange(Rinci);


            return Transaksi;
        }

        public List<FcCom> GetFcCom()
        {
            return _contextFC.FcComs.ToList();
        }

        public async Task<bool> DelFcCom(int id)
        {
            try
            {
                var ExistingCompany = _contextFC.FcComs.Where(x => x.FcComId == id).FirstOrDefault();
                if (ExistingCompany != null)
                {
                    _contextFC.FcComs.Remove(ExistingCompany);
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public bool CekCompany(string customer)
        {
            string test = customer.ToUpper();
            var cekFirst = _contextFC.FcComs.Where(x => x.FcComKode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddCompany(FcComView codeview)
        {
            string test = codeview.FcComKode.ToUpper();
            var cekFirst = _contextFC.FcComs.Where(x => x.FcComKode == test).ToList();
            if (cekFirst.Count == 0)
            {
                FcCom AcctCode = new FcCom()
                {
                    FcComKode = codeview.FcComKode.ToUpper(),
                    FcNamaPerusahaan = codeview.FcNamaPerusahaan,
                    GlAcct1 = codeview.GlAcct1,
                    GlAcct2 = codeview.GlAcct2,
                    GlAcct3 = codeview.GlAcct3,
                    GlAcct4 = codeview.GlAcct4,
                    GlAcct5 = codeview.GlAcct5,
                    GlAcct6 = codeview.GlAcct6

                };
                _contextFC.FcComs.Add(AcctCode);
                _contextFC.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public FcCom GetCompanyId(int id)
        {
            return _contextFC.FcComs.Where(x => x.FcComId == id).FirstOrDefault();
        }

        public async Task<bool> EditCompany(FcComView codeview)
        {
            try
            {
                var ExistingAkunSet = _contextFC.FcComs.Where(x => x.FcComId == codeview.FcComId).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.FcNamaPerusahaan = codeview.FcNamaPerusahaan;
                    ExistingAkunSet.GlAcct1 = codeview.GlAcct1;
                    ExistingAkunSet.GlAcct2 = codeview.GlAcct2;
                    ExistingAkunSet.GlAcct3 = codeview.GlAcct3;
                    ExistingAkunSet.GlAcct4 = codeview.GlAcct4;
                    ExistingAkunSet.GlAcct5 = codeview.GlAcct5;
                    ExistingAkunSet.GlAcct6 = codeview.GlAcct6;
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public List<FcTransH> prosesFinancial1(int tahun, string kodeCompany)
        {
            var deleteCom = _contextFC.FcAccounts.Where(x => x.FcTahun == tahun && x.FcComKode == kodeCompany).ToList();
            var deleteTransH = _contextFC.FcTransHs.Where(x => x.FcComKode == kodeCompany && x.Tanggal.Year == tahun).ToList();

            var ComClearing = _contextFC.FcComs.Where(x => x.FcComKode == kodeCompany).FirstOrDefault();



            _contextFC.FcAccounts.RemoveRange(deleteCom);
            _contextFC.FcTransHs.RemoveRange(deleteTransH);
            _contextFC.SaveChanges();

            var addAccount = _contextGL.GlAccounts.ToList();
            List<FcAccount> Accounts = new List<FcAccount>();


            List<FcTransH> FCGlTransH = new();
            List<FcTransD> FCGlTransD = new();

            List<FcTransHView> FCTransH = new();

            foreach (var item in addAccount)
            {

                var fcaccount = new FcAccount();

                fcaccount.FcTahun = tahun;
                fcaccount.FcComKode = kodeCompany;
                fcaccount.GlAcct = item.GlAcct;
                fcaccount.GlDept = item.GlDept;
                fcaccount.GlNama = item.GlNama;
                fcaccount.GlTipe = item.GlTipe;
                fcaccount.GlStatus = item.GlStatus;

                Accounts.Add(fcaccount);
            }

            #region accountbeforetahunproses
            var BeforeCom = _contextFC.FcAccounts.Where(x => x.FcTahun == (tahun - 1) && x.FcComKode == kodeCompany).ToList();

            decimal mRetained = 0;

            if (BeforeCom.Any())
            {
                foreach (var item in Accounts)
                {
                    item.GlSldAwal = BeforeCom.Find(x => x.GlAcct == item.GlAcct).GlSaldo;
                    if (item.GlTipe == 3)
                    {
                        mRetained = item.GlSldAwal;
                    }
                }
                Accounts.Where(x => x.GlTipe == 4).FirstOrDefault().GlSldAwal += mRetained;
                Accounts.Where(x => x.GlTipe == 3).FirstOrDefault().GlSldAwal = 0;
            }
            #endregion

            if (true)
            {
                #region pembelian
                var TransHBeli = _contextIR.IrTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                foreach (var item in TransHBeli)
                {

                    List<FcTransDView> FCTransD = new();

                    if (item.Jumlah != 0)
                    {
                        var IRAkunset = (from suppliers in _contextAP.ApSuppls
                                         join accts in _contextAP.ApAccts on suppliers.AcctSet equals accts.AcctSet
                                         where suppliers.Supplier == item.Supplier
                                         select new ApAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).FirstOrDefault();

                        var TransDs = _contextIR.IrTransDs.Where(x => x.IrTransHId == item.IrTransHId).ToList();


                        // Persediaan

                        foreach (var detail in TransDs)
                        {
                            if (detail.Jumlah != 0)
                            {

                                var ICAkunset = (from inventory in _contextIC.IcItems
                                                 join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                                 where inventory.ItemCode == detail.ItemCode
                                                 select new IcAcct()
                                                 {
                                                     Acct1 = accts.Acct1,
                                                     Acct2 = accts.Acct2,
                                                     Acct3 = accts.Acct3,
                                                     Acct4 = accts.Acct4,
                                                     Acct5 = accts.Acct5,
                                                     Acct6 = accts.Acct6,
                                                     AcctSet = accts.AcctSet,
                                                     Description = accts.Description
                                                 }).
                                                 FirstOrDefault();



                                // Akun IC Control //


                                var findItem = FCTransD.Find(x => x.GlAcct == ICAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (item.Kode == "82")
                                    {
                                        findItem.Debet += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Kredit += 0;

                                    }
                                    else
                                    {
                                        findItem.Kredit += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Debet += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                    if (item.Kode == "82")
                                    {
                                        GlTransD.Debet = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Kredit = 0;

                                    }
                                    else
                                    {
                                        GlTransD.Kredit = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Debet = 0;
                                    }

                                    FCTransD.Add(GlTransD);
                                }

                            }

                        }


                        // Supplier TtlJumlah
                        // AP Control
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                }
                                else
                                {
                                    findItemTtlJumlah.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct1);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Kredit += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemPPN.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Kredit += 0;

                                }
                                else
                                {
                                    findItemPPN.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct2);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemONGKIR.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Kredit += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct3);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "82" ? "IR-IN" : "IR-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransD)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit



                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion
            }

            if (true)
            {
                #region penjualan
                var TransHJual = _contextOE.OeTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                List<FcTransHView> FCTransHJual = new();


                foreach (var item in TransHJual)
                {

                    List<FcTransDView> FCTransDJual = new();

                    var OEAkunset = (from customers in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on customers.AcctSet equals accts.AcctSet
                                     where customers.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    if (item.Jumlah != 0)
                    {


                        var TransDsJual = _contextOE.OeTransDs.Where(x => x.OeTransHId == item.OeTransHId).ToList();


                        #region detailpenjualan
                        foreach (var detail in TransDsJual)
                        {
                            // Persediaan
                            var ICAkunset = (from inventory in _contextIC.IcItems
                                             join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                             where inventory.ItemCode == detail.ItemCode
                                             select new IcAcct()
                                             {
                                                 Acct1 = accts.Acct1,
                                                 Acct2 = accts.Acct2,
                                                 Acct3 = accts.Acct3,
                                                 Acct4 = accts.Acct4,
                                                 Acct5 = accts.Acct5,
                                                 Acct6 = accts.Acct6,
                                                 AcctSet = accts.AcctSet,
                                                 Description = accts.Description
                                             }).
                                                FirstOrDefault();

                            var ICCategory = (from inventory in _contextIC.IcItems
                                              join categories in _contextIC.IcCats on inventory.Category equals categories.CatCode
                                              where inventory.ItemCode == detail.ItemCode
                                              select new IcCat()
                                              {
                                                  Cat1 = categories.Cat1,
                                                  Cat2 = categories.Cat2,
                                                  Cat3 = categories.Cat3,
                                                  Cat4 = categories.Cat4,
                                                  Cat5 = categories.Cat5,
                                                  Cat6 = categories.Cat6,
                                                  CatCode = categories.CatCode,
                                                  Description = categories.Description
                                              }).
                                                FirstOrDefault();

                            if (detail.Cost != 0)
                            {


                                var findItem = FCTransDJual.Find(x => x.GlAcct == ICAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Cost;

                                    }
                                    else
                                    {
                                        findItem.Kredit = 0;
                                        findItem.Debet += detail.Cost;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Cost;

                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Cost;
                                        GlTransD.Kredit += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }

                            // Cost Of Good Sold

                            if (detail.Cost != 0)
                            {


                                // HPP //


                                var findItemHpp = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat1);

                                if (findItemHpp != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItemHpp.Kredit += 0;
                                        findItemHpp.Debet += detail.Cost;

                                    }
                                    else
                                    {
                                        findItemHpp.Debet = 0;
                                        findItemHpp.Kredit += detail.Cost;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICCategory.Cat1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICCategory.Cat1);

                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += detail.Cost;

                                    }
                                    else
                                    {
                                        GlTransD.Kredit += detail.Cost;
                                        GlTransD.Debet += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }


                            // Sales Revenue
                            if (detail.Jumlah != 0)
                            {




                                var findItem = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat2);

                                if (findItem != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        findItem.Kredit = 0;
                                        findItem.Debet += detail.Jumlah;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICCategory.Cat2;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICCategory.Cat2);
                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }
                        }
                        #endregion

                        // Customer TtlJumlah
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;
                                }
                                else
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct1);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Customer Discount
                        if (item.Discount != 0)
                        {
                            var findItemDiscount = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct5);

                            if (findItemDiscount != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemDiscount.Debet += item.Discount;
                                    findItemDiscount.Kredit += 0;

                                }
                                else
                                {
                                    findItemDiscount.Kredit += item.Discount;
                                    findItemDiscount.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct5;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", Discount, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct5);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Discount;

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Discount;
                                    GlTransD.Debet += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }

                        }

                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemPPN.Kredit += item.Ppn;
                                    findItemPPN.Debet += 0;

                                }
                                else
                                {
                                    findItemPPN.Debet += item.Ppn;
                                    findItemPPN.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct2);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet = 0;
                                    GlTransD.Kredit = item.Ppn;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ppn;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Customer ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemONGKIR.Kredit += item.Ongkos;
                                    findItemONGKIR.Debet += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Debet += item.Ongkos;
                                    findItemONGKIR.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct3);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Ongkos;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ongkos;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "94" ? "OE-IN" : "OE-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransDJual)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion

                #region inventory
                var TransHInv = _contextIC.IcTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoFaktur).ToList();

                List<FcTransHView> FCTransHInv = new();


                foreach (var item in TransHInv)
                {

                    List<FcTransDView> FCTransDInv = new();

                    //      if (true)
                    //      {

                    var TransDsInv = _contextIC.IcTransDs.Where(x => x.IcTransHId == item.IcTransHId).ToList();



                    foreach (var detail in TransDsInv)
                    {

                        var ICAkunset = (from inventory in _contextIC.IcItems
                                         join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                         where inventory.ItemCode == detail.ItemCode
                                         select new IcAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).
                                                FirstOrDefault();



                        if (detail.Jumlah != 0)
                        {



                            // Persediaan
                            #region inventoryPersediaan
                            var findItem = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct1);

                            if (findItem != null)
                            {
                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        findItem.Debet += detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }


                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ICAkunset.Acct1;
                                GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                }


                                FCTransDInv.Add(GlTransD);
                            }
                            #endregion

                            // Adjustment
                            #region inventoryAdjustment
                            var findItemAdj = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct2);

                            if (findItemAdj != null)
                            {
                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        findItemAdj.Debet += detail.Jumlah * -1;
                                        findItemAdj.Kredit += 0;
                                    }
                                    else
                                    {
                                        findItemAdj.Debet += 0;
                                        findItemAdj.Kredit += detail.Jumlah;
                                    }


                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ICAkunset.Acct2;
                                GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                GlTransD.GlDept = GetNameAccount(ICAkunset.Acct2);

                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        GlTransD.Debet += detail.Jumlah * -1;
                                        GlTransD.Kredit += 0;
                                    }
                                    else
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;
                                    }

                                }


                                FCTransDInv.Add(GlTransD);
                            }
                            #endregion
                        }


                    }

                    if (FCTransDInv != null)
                    {



                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoFaktur,
                            KodeGl = (item.Kode == "81" ? "IV-AD" : "IV-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };

                        foreach (var detail in FCTransDInv)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                        //      }
                    }
                }

                #endregion
            }

            if (true)
            {
                #region Hutang
                var TransHHutang = _contextAP.ApTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();

                List<FcTransHView> FCTransHHutang = new();


                foreach (var item in TransHHutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDHutang = new();

                    var APAkunset = (from vendors in _contextAP.ApSuppls
                                     join accts in _contextAP.ApAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Supplier == item.Supplier
                                     select new ApAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AP-IN
                    if (item.Kode == "21")
                    {


                        var TransDsAPIN = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();

                        foreach (var detail in TransDsAPIN)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextAP.ApDists
                                               where distribution.DistCode == detail.DistCode
                                               select new ApDist()
                                               {
                                                   Dist1 = distribution.Dist1,
                                                   Description = distribution.Description
                                               }).
                                                FirstOrDefault();


                            if (detail.Jumlah != 0)
                            {


                                var findItem = FCTransDHutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                if (findItem != null)
                                {
                                    if (item.Jumlah > 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += detail.Jumlah;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        findItem.Debet = 0;
                                        findItem.Kredit += -1 * detail.Jumlah;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.Dist1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                    if (item.Jumlah > 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += detail.Jumlah;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        GlTransD.Kredit += -1 * detail.Jumlah;
                                        GlTransD.Debet += 0;
                                    }

                                    if (item.Jumlah != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                            }



                        }

                    }

                    #endregion

                    #region AP-DP
                    if (item.Kode == "23")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = APAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDHutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AP-PY
                    if (item.Kode == "24")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsAPPY = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsAPPY)
                        {
                            decimal detailKurs = _contextAP.ApHutangs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            if (item.Kurs != 0)
                                detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "21" || detail.KodeTran == "82" || detail.KodeTran == "83")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "23")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == ComClearing.GlAcct3);

                        if (item.Kode == "21")
                            findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct3;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct3);

                            if (item.Kode == "21")
                            {
                                GlTransD.GlAcct = APAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);
                            }

                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDHutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = APAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(APAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDHutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "21" ? "AP-IN" : (item.Kode == "23" ? "AP-DP" : "AP-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDHutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }







                #endregion
            }


            if (true)
            {
                #region Piutang
                var TransHPiutang = _contextAR.ArTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();

                List<FcTransHView> FCTransHPiutang = new();


                foreach (var item in TransHPiutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDPiutang = new();

                    var ARAkunset = (from vendors in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AR-IN
                    if (item.Kode == "11")
                    {


                        var TransDsARIN = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();

                        foreach (var detail in TransDsARIN)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextAR.ArDists
                                               where distribution.DistCode == detail.DistCode
                                               select new ApDist()
                                               {
                                                   Dist1 = distribution.Dist1,
                                                   Description = distribution.Description
                                               }).
                                                FirstOrDefault();


                            if (detail.Jumlah != 0)
                            {


                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                if (findItem != null)
                                {
                                    if (item.Jumlah > 0)
                                    {
                                        findItem.Kredit += detail.Jumlah;
                                        findItem.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        findItem.Debet = -1 * detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.Dist1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                    if (item.Jumlah > 0)
                                    {
                                        GlTransD.Kredit += detail.Jumlah;
                                        GlTransD.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * detail.Jumlah;
                                    }

                                    if (item.Jumlah != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                            }



                        }

                    }

                    #endregion

                    #region AR-DP
                    if (item.Kode == "13")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += 0;
                                    findItem.Kredit += (item.Jumlah); ;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = 0;
                                    findItem.Debet += -1 * (item.Jumlah);
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ARAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDPiutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AR-PY
                    if (item.Kode == "14")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsARPY = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsARPY)
                        {
                            //  decimal detailKurs = _contextAR.ArPiutngs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            decimal detailKurs = 0;

                            //   if (item.Kurs != 0)
                            //      detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "11" || detail.KodeTran == "94" || detail.KodeTran == "95")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "13")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ComClearing.GlAcct2);

                        if (item.Kode == "11")
                            findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += -1 * item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct2;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct2);

                            if (item.Kode == "11")
                            {
                                GlTransD.GlAcct = ARAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);
                            }

                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += -1 * item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ARAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ARAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "11" ? "AR-IN" : (item.Kode == "13" ? "AR-DP" : "AR-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDPiutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }

                #endregion
            }

            if (true)
            {
                #region KasBank
                var TransHCB = _contextCB.CbTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                List<FcTransHView> FCTransHCB = new();


                foreach (var item in TransHCB)
                {

                    List<FcTransDView> FCTransDCB = new();

                    var CBAkunset = (from banks in _contextCB.CbBanks
                                     where banks.KodeBank == item.KodeBank
                                     select banks).FirstOrDefault();




                    #region KasbankDetail
                    if (true)
                    {



                        var TransDsCB = _contextCB.CbTransDs.Where(x => x.CbTransHId == item.CbTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsCB)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextCB.CbSrcCodes
                                               where distribution.SrcCode == detail.SrcCode
                                               select distribution).FirstOrDefault();

                            // Transaksi Kasbank
                            if (true)
                            {
                                //if (DistAkunset == null)
                                //{
                                //    var test = item.DocNo;
                                //    var test2 = item.Keterangan;
                                //}

                                //if(DistAkunset.GlAcct == null)
                                //{
                                //    var test = item.DocNo;
                                //    var test2 = item.Keterangan;
                                //}

                                var findItem = FCTransDCB.Find(x => x.GlAcct == DistAkunset.GlAcct);

                                if (findItem != null)
                                {
                                    if (detail.Jumlah > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah;


                                    }
                                    else if (detail.Jumlah < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detail.Jumlah);


                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.GlAcct;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.DocNo;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.GlAcct);

                                    if (detail.Jumlah > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;



                                    }
                                    else if (detail.Jumlah < 0)
                                    {
                                        GlTransD.Debet += -1 * (detail.Jumlah);
                                        GlTransD.Kredit += 0;

                                    }
                                    if (detail.Jumlah != 0)
                                        FCTransDCB.Add(GlTransD);
                                }

                            }


                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDCB.Find(x => x.GlAcct == CBAkunset.Acctset);


                        if (findItemTtlJumlah != null)
                        {
                            if (true)
                            {
                                if (item.Saldo > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Saldo;

                                }
                                else if (item.Saldo < 0)
                                {
                                    findItemTtlJumlah.Kredit += -1 * item.Saldo;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }



                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = CBAkunset.Acctset;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.DocNo;
                            GlTransD.GlDept = GetNameAccount(CBAkunset.Acctset);


                            if (true)
                            {
                                if (item.Saldo > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Saldo);

                                }
                                else if (item.Saldo < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Saldo);
                                }

                            }



                            if (item.Saldo != 0)
                                FCTransDCB.Add(GlTransD);
                        }
                    }
                    #endregion




                    FcTransHView GltransH = new()
                    {
                        DocNo = item.DocNo,
                        KodeGl = "CB-" + item.KodeBank.Trim(),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDCB)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }

                #endregion
            }

            if (true)
            {
                #region Asset

                var TransAS = _contextAS.AsTransaksis.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.BarcodeAssets).ToList();
                List<FcTransDView> GlTransd = new();
                FcTransHView GltransH = new();


                foreach (var item in TransAS)
                {
                    GlTransd.Clear();

                    var barcode = (from account in _contextAS.AsAssetss
                                   where account.BarcodeAssets == item.BarcodeAssets
                                   select account).FirstOrDefault();

                    var Akunset = (from account in _contextAS.AsAcctsets
                                   where account.AcctSet == barcode.Acctset
                                   select account).FirstOrDefault();

                    var Distribution = (from distribution in _contextAS.AsDistSets
                                        where distribution.DistCode == barcode.DistCode
                                        select distribution).FirstOrDefault();

                    if (item.Kode == "01")   // pembelian
                    {
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Akunset.Acct1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Akunset.Acct1),
                            Debet = 0,
                            Kredit = item.Nilai,


                        });
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Distribution.Dist1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Distribution.Dist1),
                            Debet = item.Nilai,
                            Kredit = 0,


                        });
                    }
                    //if (item.Kode == "02")  // penjualan
                    //{
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Akunset.Acct1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = item.Nilai,
                    //        Kredit = 0,


                    //    });
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Distribution.Dist1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = 0,
                    //        Kredit = item.Nilai,


                    //    });
                    //}

                    GltransH = new()
                    {
                        DocNo = item.BarcodeAssets + item.Tanggal.ToString("yyyyMM"),
                        KodeGl = "GL-AS",
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.BarcodeAssets) ? " " : item.BarcodeAssets + item.Tanggal.ToString("yyyyMM")),
                        FcTransDs = GlTransd
                    };

                    FCTransH.Add(GltransH);
                }
                #endregion
            }



            if (true)
            {
                #region generalLedger

                var TransGL = _contextGL.GlTransHs.Include(p => p.GlTransDs).Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                foreach (var item in TransGL)
                {
                    FcTransHView GltransH = new()
                    {
                        DocNo = item.DocNo,
                        KodeGl = item.KodeGl,
                        Tanggal = item.Tanggal,
                        GlMemo = item.GlMemo,
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in item.GlTransDs)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = GetNameAccount(detail.GlAcct),
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);
                }

                #endregion
            }

            // FcGLTransaksi General Ledger       
            var TransHFcGL = _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.Tanggal.Year == tahun && x.FcComKode == kodeCompany).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

            // Semua hasil proses dimasukkan ke FC Ledger  // 
            if (true)
            {
                foreach (var GlTransH in FCTransH)
                {
                    FcTransH fcGltransH = new()
                    {
                        Tanggal = GlTransH.Tanggal,
                        DocNo = GlTransH.DocNo,
                        KodeGl = GlTransH.KodeGl,
                        GlMemo = GlTransH.GlMemo,
                        Debet = GlTransH.Debet,
                        Kredit = GlTransH.Kredit,
                        Saldo = GlTransH.Saldo,
                        FcComKode = kodeCompany,
                        FcTransDs = new List<FcTransD>()

                    };
                    foreach (var detail in GlTransH.FcTransDs)
                    {
                        fcGltransH.FcTransDs.Add(new FcTransD()
                        {
                            GlAcct = detail.GlAcct,
                            FcComKode = kodeCompany,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit,
                            Jumlah = detail.Jumlah

                        });
                    }
                    FCGlTransH.Add(fcGltransH);
                }

                if (TransHFcGL.Any())
                {
                    foreach (var FcGLTransHs in TransHFcGL)
                    {
                        FcTransH fcGltransH = new()
                        {
                            Tanggal = FcGLTransHs.Tanggal,
                            DocNo = FcGLTransHs.DocNo,
                            KodeGl = FcGLTransHs.KodeGl,
                            GlMemo = FcGLTransHs.GlMemo,
                            Debet = FcGLTransHs.Debet,
                            Kredit = FcGLTransHs.Kredit,
                            Saldo = FcGLTransHs.Saldo,
                            FcComKode = FcGLTransHs.FcComKode,
                            FcTransDs = new List<FcTransD>()

                        };
                        foreach (var detail in FcGLTransHs.FcGlTransDs)
                        {
                            fcGltransH.FcTransDs.Add(new FcTransD()
                            {
                                GlAcct = detail.GlAcct,
                                FcComKode = detail.FcComKode,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit,
                                Jumlah = detail.Jumlah

                            });
                        }
                        FCGlTransH.Add(fcGltransH);
                    }
                }
            }

            foreach (var gltrans in FCGlTransH)
            {
                switch (gltrans.Tanggal.Month)
                {
                    case 1:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc1 += detail.Jumlah;
                        }
                        break;
                    case 2:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc2 += detail.Jumlah;
                        }
                        break;
                    case 3:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc3 += detail.Jumlah;
                        }
                        break;
                    case 4:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc4 += detail.Jumlah;
                        }
                        break;
                    case 5:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc5 += detail.Jumlah;
                        }
                        break;
                    case 6:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc6 += detail.Jumlah;
                        }
                        break;
                    case 7:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc7 += detail.Jumlah;
                        }
                        break;
                    case 8:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc8 += detail.Jumlah;
                        }
                        break;
                    case 9:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc9 += detail.Jumlah;
                        }
                        break;
                    case 10:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc10 += detail.Jumlah;
                        }
                        break;
                    case 11:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc11 += detail.Jumlah;
                        }
                        break;
                    case 12:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc12 += detail.Jumlah;
                        }


                        break;
                }

            }

            Accounts.ForEach(Accounts => { Accounts.GlSaldo = 0; });

            decimal mRugiLaba = 0;

            foreach (var item in Accounts.Where(x => x.GlTipe == 2))
            {
                mRugiLaba += (item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                               item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12);
            }

            foreach (var item in Accounts)
            {
                if (item.GlTipe != 2)
                {
                    item.GlSaldo = item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
                if (item.GlTipe == 3)
                {
                    item.GlSaldo = mRugiLaba + item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
            }

            _contextFC.FcAccounts.AddRange(Accounts);
            _contextFC.FcTransHs.AddRange(FCGlTransH);
            _contextFC.SaveChanges();

            return FCGlTransH;
        }

        public List<FcTransH> prosesFinancial2(int tahun, string kodeCompany)
        {
            var deleteCom = _contextFC.FcAccounts.Where(x => x.FcTahun == tahun && x.FcComKode == kodeCompany).ToList();
            var deleteTransH = _contextFC.FcTransHs.Where(x => x.FcComKode == kodeCompany && x.Tanggal.Year == tahun).ToList();

            var ComClearing = _contextFC.FcComs.Where(x => x.FcComKode == kodeCompany).FirstOrDefault();


            _contextFC.FcAccounts.RemoveRange(deleteCom);
            _contextFC.FcTransHs.RemoveRange(deleteTransH);
            _contextFC.SaveChanges();

            var addAccount = _contextGL.GlAccounts.ToList();
            List<FcAccount> Accounts = new List<FcAccount>();

            List<FcTransH> FCGlTransH = new();
            List<FcTransD> FCGlTransD = new();

            List<FcTransHView> FCTransH = new();

            foreach (var item in addAccount)
            {

                var fcaccount = new FcAccount();

                fcaccount.FcTahun = tahun;
                fcaccount.FcComKode = kodeCompany;
                fcaccount.GlAcct = item.GlAcct;
                fcaccount.GlDept = item.GlDept;
                fcaccount.GlNama = item.GlNama;
                fcaccount.GlTipe = item.GlTipe;
                fcaccount.GlStatus = item.GlStatus;

                Accounts.Add(fcaccount);
            }
            #region accountbeforetahunproses
            var BeforeCom = _contextFC.FcAccounts.Where(x => x.FcTahun == (tahun - 1) && x.FcComKode == kodeCompany).ToList();

            decimal mRetained = 0;

            if (BeforeCom.Any())
            {
                foreach (var item in Accounts)
                {
                    item.GlSldAwal = BeforeCom.Find(x => x.GlAcct == item.GlAcct).GlSaldo;
                    if (item.GlTipe == 3)
                    {
                        mRetained = item.GlSldAwal;
                    }
                }
                Accounts.Where(x => x.GlTipe == 4).FirstOrDefault().GlSldAwal += mRetained;
                Accounts.Where(x => x.GlTipe == 3).FirstOrDefault().GlSldAwal = 0;
            }

            #endregion

            if (true)
            {
                #region pembelian
                var TransHBeli = _contextIR.IrTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                foreach (var item in TransHBeli)
                {

                    List<FcTransDView> FCTransD = new();

                    if (item.Jumlah != 0)
                    {
                        var IRAkunset = (from suppliers in _contextAP.ApSuppls
                                         join accts in _contextAP.ApAccts on suppliers.AcctSet equals accts.AcctSet
                                         where suppliers.Supplier == item.Supplier
                                         select new ApAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).FirstOrDefault();

                        var TransDs = _contextIR.IrTransDs.Where(x => x.IrTransHId == item.IrTransHId).ToList();


                        // Persediaan

                        foreach (var detail in TransDs)
                        {
                            if (detail.Jumlah != 0)
                            {

                                var ICAkunset = (from inventory in _contextIC.IcItems
                                                 join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                                 where inventory.ItemCode == detail.ItemCode
                                                 select new IcAcct()
                                                 {
                                                     Acct1 = accts.Acct1,
                                                     Acct2 = accts.Acct2,
                                                     Acct3 = accts.Acct3,
                                                     Acct4 = accts.Acct4,
                                                     Acct5 = accts.Acct5,
                                                     Acct6 = accts.Acct6,
                                                     AcctSet = accts.AcctSet,
                                                     Description = accts.Description
                                                 }).
                                                 FirstOrDefault();



                                // Akun IC Control //


                                var findItem = FCTransD.Find(x => x.GlAcct == ICAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (item.Kode == "82")
                                    {
                                        findItem.Debet += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Kredit += 0;

                                    }
                                    else
                                    {
                                        findItem.Kredit += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Debet += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                    if (item.Kode == "82")
                                    {
                                        GlTransD.Debet = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Kredit = 0;

                                    }
                                    else
                                    {
                                        GlTransD.Kredit = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Debet = 0;
                                    }

                                    FCTransD.Add(GlTransD);
                                }

                            }

                        }


                        // Supplier TtlJumlah
                        // AP Control
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                }
                                else
                                {
                                    findItemTtlJumlah.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct1);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Kredit += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemPPN.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Kredit += 0;

                                }
                                else
                                {
                                    findItemPPN.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct2);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemONGKIR.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Kredit += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct3);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "82" ? "IR-IN" : "IR-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransD)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit



                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion
            }

            if (true)
            {
                #region penjualan
                var TransHJual = _contextOE.OeTransHs.Where(x => x.Tanggal.Year == tahun && x.Pajak == true).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                List<FcTransHView> FCTransHJual = new();


                foreach (var item in TransHJual)
                {

                    List<FcTransDView> FCTransDJual = new();

                    var OEAkunset = (from customers in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on customers.AcctSet equals accts.AcctSet
                                     where customers.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    if (item.Jumlah != 0)
                    {


                        var TransDsJual = _contextOE.OeTransDs.Where(x => x.OeTransHId == item.OeTransHId).ToList();


                        #region detailpenjualan
                        foreach (var detail in TransDsJual)
                        {
                            // Persediaan
                            var ICAkunset = (from inventory in _contextIC.IcItems
                                             join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                             where inventory.ItemCode == detail.ItemCode
                                             select new IcAcct()
                                             {
                                                 Acct1 = accts.Acct1,
                                                 Acct2 = accts.Acct2,
                                                 Acct3 = accts.Acct3,
                                                 Acct4 = accts.Acct4,
                                                 Acct5 = accts.Acct5,
                                                 Acct6 = accts.Acct6,
                                                 AcctSet = accts.AcctSet,
                                                 Description = accts.Description
                                             }).
                                                FirstOrDefault();

                            var ICCategory = (from inventory in _contextIC.IcItems
                                              join categories in _contextIC.IcCats on inventory.Category equals categories.CatCode
                                              where inventory.ItemCode == detail.ItemCode
                                              select new IcCat()
                                              {
                                                  Cat1 = categories.Cat1,
                                                  Cat2 = categories.Cat2,
                                                  Cat3 = categories.Cat3,
                                                  Cat4 = categories.Cat4,
                                                  Cat5 = categories.Cat5,
                                                  Cat6 = categories.Cat6,
                                                  CatCode = categories.CatCode,
                                                  Description = categories.Description
                                              }).
                                                FirstOrDefault();

                            if (detail.Cost != 0)
                            {


                                var findItem = FCTransDJual.Find(x => x.GlAcct == ICAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Cost;

                                    }
                                    else
                                    {
                                        findItem.Kredit = 0;
                                        findItem.Debet += detail.Cost;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Cost;

                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Cost;
                                        GlTransD.Kredit += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }

                            // Cost Of Good Sold

                            if (detail.Cost != 0)
                            {


                                // HPP //


                                var findItemHpp = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat1);

                                if (findItemHpp != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItemHpp.Kredit += 0;
                                        findItemHpp.Debet += detail.Cost;

                                    }
                                    else
                                    {
                                        findItemHpp.Debet = 0;
                                        findItemHpp.Kredit += detail.Cost;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICCategory.Cat1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICCategory.Cat1);

                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += detail.Cost;

                                    }
                                    else
                                    {
                                        GlTransD.Kredit += detail.Cost;
                                        GlTransD.Debet += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }


                            // Sales Revenue
                            if (detail.Jumlah != 0)
                            {




                                var findItem = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat2);

                                if (findItem != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        findItem.Kredit = 0;
                                        findItem.Debet += detail.Jumlah;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICCategory.Cat2;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICCategory.Cat2);
                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }
                        }
                        #endregion

                        // Customer TtlJumlah
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;
                                }
                                else
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct1);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemPPN.Kredit += item.Ppn;
                                    findItemPPN.Debet += 0;

                                }
                                else
                                {
                                    findItemPPN.Debet += item.Ppn;
                                    findItemPPN.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct2);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet = 0;
                                    GlTransD.Kredit = item.Ppn;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ppn;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Customer ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemONGKIR.Kredit += item.Ongkos;
                                    findItemONGKIR.Debet += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Debet += item.Ongkos;
                                    findItemONGKIR.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct3);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Ongkos;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ongkos;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "94" ? "OE-IN" : "OE-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransDJual)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion

                #region inventory
                var TransHInv = _contextIC.IcTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoFaktur).ToList();

                List<FcTransHView> FCTransHInv = new();


                foreach (var item in TransHInv)
                {

                    List<FcTransDView> FCTransDInv = new();

                    //      if (true)
                    //      {

                    var TransDsInv = _contextIC.IcTransDs.Where(x => x.IcTransHId == item.IcTransHId).ToList();



                    foreach (var detail in TransDsInv)
                    {

                        var ICAkunset = (from inventory in _contextIC.IcItems
                                         join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                         where inventory.ItemCode == detail.ItemCode
                                         select new IcAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).
                                                FirstOrDefault();



                        if (detail.Jumlah != 0)
                        {



                            // Persediaan
                            #region inventoryPersediaan
                            var findItem = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct1);

                            if (findItem != null)
                            {
                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        findItem.Debet += detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }


                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ICAkunset.Acct1;
                                GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                }


                                FCTransDInv.Add(GlTransD);
                            }
                            #endregion

                            // Adjustment
                            #region inventoryAdjustment
                            var findItemAdj = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct2);

                            if (findItemAdj != null)
                            {
                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        findItemAdj.Debet += detail.Jumlah * -1;
                                        findItemAdj.Kredit += 0;
                                    }
                                    else
                                    {
                                        findItemAdj.Debet += 0;
                                        findItemAdj.Kredit += detail.Jumlah;
                                    }


                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ICAkunset.Acct2;
                                GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                GlTransD.GlDept = GetNameAccount(ICAkunset.Acct2);

                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        GlTransD.Debet += detail.Jumlah * -1;
                                        GlTransD.Kredit += 0;
                                    }
                                    else
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;
                                    }

                                }


                                FCTransDInv.Add(GlTransD);
                            }
                            #endregion
                        }


                    }

                    if (FCTransDInv != null)
                    {



                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoFaktur,
                            KodeGl = (item.Kode == "81" ? "IV-AD" : "IV-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };

                        foreach (var detail in FCTransDInv)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                        //      }
                    }
                }

                #endregion
            }

            if (true)
            {
                #region Hutang
                var TransHHutang = _contextAP.ApTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();

                List<FcTransHView> FCTransHHutang = new();


                foreach (var item in TransHHutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDHutang = new();

                    var APAkunset = (from vendors in _contextAP.ApSuppls
                                     join accts in _contextAP.ApAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Supplier == item.Supplier
                                     select new ApAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AP-IN
                    if (item.Kode == "21")
                    {


                        var TransDsAPIN = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();

                        foreach (var detail in TransDsAPIN)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextAP.ApDists
                                               where distribution.DistCode == detail.DistCode
                                               select new ApDist()
                                               {
                                                   Dist1 = distribution.Dist1,
                                                   Description = distribution.Description
                                               }).
                                                FirstOrDefault();


                            if (detail.Jumlah != 0)
                            {


                                var findItem = FCTransDHutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                if (findItem != null)
                                {
                                    if (item.Jumlah > 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += detail.Jumlah;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        findItem.Debet = 0;
                                        findItem.Kredit += -1 * detail.Jumlah;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.Dist1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                    if (item.Jumlah > 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += detail.Jumlah;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        GlTransD.Kredit += -1 * detail.Jumlah;
                                        GlTransD.Debet += 0;
                                    }

                                    if (item.Jumlah != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                            }



                        }

                    }

                    #endregion

                    #region AP-DP
                    if (item.Kode == "23")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = APAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDHutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AP-PY
                    if (item.Kode == "24")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsAPPY = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsAPPY)
                        {
                            decimal detailKurs = _contextAP.ApHutangs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            if (item.Kurs != 0)
                                detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "21" || detail.KodeTran == "82" || detail.KodeTran == "83")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "23")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == ComClearing.GlAcct3);

                        if (item.Kode == "21")
                            findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct3;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct3);

                            if (item.Kode == "21")
                            {
                                GlTransD.GlAcct = APAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);
                            }

                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDHutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = APAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(APAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDHutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "21" ? "AP-IN" : (item.Kode == "23" ? "AP-DP" : "AP-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDHutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }







                #endregion
            }


            if (true)
            {
                #region Piutang
                var TransHPiutang = _contextAR.ArTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();

                List<FcTransHView> FCTransHPiutang = new();


                foreach (var item in TransHPiutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDPiutang = new();

                    var ARAkunset = (from vendors in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AR-IN
                    if (item.Kode == "11")
                    {


                        var TransDsARIN = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();

                        foreach (var detail in TransDsARIN)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextAR.ArDists
                                               where distribution.DistCode == detail.DistCode
                                               select new ApDist()
                                               {
                                                   Dist1 = distribution.Dist1,
                                                   Description = distribution.Description
                                               }).
                                                FirstOrDefault();


                            if (detail.Jumlah != 0)
                            {


                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                if (findItem != null)
                                {
                                    if (item.Jumlah > 0)
                                    {
                                        findItem.Kredit += detail.Jumlah;
                                        findItem.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        findItem.Debet = -1 * detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.Dist1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                    if (item.Jumlah > 0)
                                    {
                                        GlTransD.Kredit += detail.Jumlah;
                                        GlTransD.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * detail.Jumlah;
                                    }

                                    if (item.Jumlah != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                            }



                        }

                    }

                    #endregion

                    #region AR-DP
                    if (item.Kode == "13")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += 0;
                                    findItem.Kredit += (item.Jumlah); ;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = 0;
                                    findItem.Debet += -1 * (item.Jumlah);
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ARAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDPiutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AR-PY
                    if (item.Kode == "14")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsARPY = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsARPY)
                        {
                            //  decimal detailKurs = _contextAR.ArPiutngs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            decimal detailKurs = 0;

                            //   if (item.Kurs != 0)
                            //      detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "11" || detail.KodeTran == "94" || detail.KodeTran == "95")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "13")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ComClearing.GlAcct2);

                        if (item.Kode == "11")
                            findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += -1 * item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct2;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct2);

                            if (item.Kode == "11")
                            {
                                GlTransD.GlAcct = ARAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);
                            }

                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += -1 * item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ARAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ARAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "11" ? "AR-IN" : (item.Kode == "13" ? "AR-DP" : "AR-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDPiutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }

                #endregion
            }

            if (true)
            {
                #region KasBank
                var TransHCB = _contextCB.CbTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                List<FcTransHView> FCTransHCB = new();


                foreach (var item in TransHCB)
                {
                    var bankPajak = _contextCB.CbBanks.Where(x => x.KodeBank == item.KodeBank).FirstOrDefault().Pajak;

                    if (!bankPajak)
                    {
                        List<FcTransDView> FCTransDCB = new();

                        var CBAkunset = (from banks in _contextCB.CbBanks
                                         where banks.KodeBank == item.KodeBank
                                         select banks).FirstOrDefault();




                        #region KasbankDetail
                        if (true)
                        {



                            var TransDsCB = _contextCB.CbTransDs.Where(x => x.CbTransHId == item.CbTransHId).ToList();


                            #region detailPembayaran
                            foreach (var detail in TransDsCB)
                            {
                                // Distribution Code
                                var DistAkunset = (from distribution in _contextCB.CbSrcCodes
                                                   where distribution.SrcCode == detail.SrcCode
                                                   select distribution).FirstOrDefault();

                                // Transaksi Kasbank
                                if (true)
                                {
                                    //if (DistAkunset == null)
                                    //{
                                    //    var test = item.DocNo;
                                    //    var test2 = item.Keterangan;
                                    //}

                                    //if(DistAkunset.GlAcct == null)
                                    //{
                                    //    var test = item.DocNo;
                                    //    var test2 = item.Keterangan;
                                    //}

                                    var findItem = FCTransDCB.Find(x => x.GlAcct == DistAkunset.GlAcct);

                                    if (findItem != null)
                                    {
                                        if (detail.Jumlah > 0)
                                        {
                                            findItem.Debet += 0;
                                            findItem.Kredit += detail.Jumlah;


                                        }
                                        else if (detail.Jumlah < 0)
                                        {
                                            findItem.Kredit += 0;
                                            findItem.Debet += -1 * (detail.Jumlah);


                                        }
                                    }
                                    else
                                    {
                                        var GlTransD = new FcTransDView();

                                        GlTransD.GlAcct = DistAkunset.GlAcct;
                                        GlTransD.Keterangan = detail.Keterangan + ", " + item.DocNo;
                                        GlTransD.GlDept = GetNameAccount(DistAkunset.GlAcct);

                                        if (detail.Jumlah > 0)
                                        {
                                            GlTransD.Debet += 0;
                                            GlTransD.Kredit += detail.Jumlah;



                                        }
                                        else if (detail.Jumlah < 0)
                                        {
                                            GlTransD.Debet += -1 * (detail.Jumlah);
                                            GlTransD.Kredit += 0;

                                        }
                                        if (detail.Jumlah != 0)
                                            FCTransDCB.Add(GlTransD);
                                    }

                                }


                            }
                            #endregion
                        }

                        #endregion

                        #region Header
                        if (true)
                        {
                            var findItemTtlJumlah = FCTransDCB.Find(x => x.GlAcct == CBAkunset.Acctset);


                            if (findItemTtlJumlah != null)
                            {
                                if (true)
                                {
                                    if (item.Saldo > 0)
                                    {
                                        findItemTtlJumlah.Kredit += 0;
                                        findItemTtlJumlah.Debet += item.Saldo;

                                    }
                                    else if (item.Saldo < 0)
                                    {
                                        findItemTtlJumlah.Kredit += -1 * item.Saldo;
                                        findItemTtlJumlah.Debet += 0;
                                    }

                                }



                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = CBAkunset.Acctset;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.DocNo;
                                GlTransD.GlDept = GetNameAccount(CBAkunset.Acctset);


                                if (true)
                                {
                                    if (item.Saldo > 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += (item.Saldo);

                                    }
                                    else if (item.Saldo < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (item.Saldo);
                                    }

                                }



                                if (item.Saldo != 0)
                                    FCTransDCB.Add(GlTransD);
                            }
                        }
                        #endregion




                        FcTransHView GltransH = new()
                        {
                            DocNo = item.DocNo,
                            KodeGl = "CB-" + item.KodeBank.Trim(),
                            Tanggal = item.Tanggal,
                            GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransDCB)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion
            }

            if (true)
            {
                #region Asset

                var TransAS = _contextAS.AsTransaksis.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.BarcodeAssets).ToList();
                List<FcTransDView> GlTransd = new();
                FcTransHView GltransH = new();


                foreach (var item in TransAS)
                {
                    GlTransd.Clear();

                    var barcode = (from account in _contextAS.AsAssetss
                                   where account.BarcodeAssets == item.BarcodeAssets
                                   select account).FirstOrDefault();

                    var Akunset = (from account in _contextAS.AsAcctsets
                                   where account.AcctSet == barcode.Acctset
                                   select account).FirstOrDefault();

                    var Distribution = (from distribution in _contextAS.AsDistSets
                                        where distribution.DistCode == barcode.DistCode
                                        select distribution).FirstOrDefault();

                    if (item.Kode == "01")   // pembelian
                    {
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Akunset.Acct1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Akunset.Acct1),
                            Debet = 0,
                            Kredit = item.Nilai,


                        });
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Distribution.Dist1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Distribution.Dist1),
                            Debet = item.Nilai,
                            Kredit = 0,


                        });
                    }
                    //if (item.Kode == "02")  // penjualan
                    //{
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Akunset.Acct1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = item.Nilai,
                    //        Kredit = 0,


                    //    });
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Distribution.Dist1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = 0,
                    //        Kredit = item.Nilai,


                    //    });
                    //}

                    GltransH = new()
                    {
                        DocNo = item.BarcodeAssets + item.Tanggal.ToString("yyyyMM"),
                        KodeGl = "GL-AS",
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.BarcodeAssets) ? " " : item.BarcodeAssets + item.Tanggal.ToString("yyyyMM")),
                        FcTransDs = GlTransd
                    };

                    FCTransH.Add(GltransH);
                }
                #endregion
            }

            if (true)
            {
                #region generalLedger

                var TransGL = _contextGL.GlTransHs.Include(p => p.GlTransDs).Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                foreach (var item in TransGL)
                {
                    FcTransHView GltransH = new()
                    {
                        DocNo = item.DocNo,
                        KodeGl = item.KodeGl,
                        Tanggal = item.Tanggal,
                        GlMemo = item.GlMemo,
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in item.GlTransDs)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = GetNameAccount(detail.GlAcct),
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);
                }

                #endregion
            }

            // FcGLTransaksi General Ledger       
            var TransHFcGL = _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.Tanggal.Year == tahun && x.FcComKode == kodeCompany).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

            // Semua hasil proses dimasukkan ke FC Ledger  // 
            if (true)
            {
                foreach (var GlTransH in FCTransH)
                {
                    FcTransH fcGltransH = new()
                    {
                        Tanggal = GlTransH.Tanggal,
                        DocNo = GlTransH.DocNo,
                        KodeGl = GlTransH.KodeGl,
                        GlMemo = GlTransH.GlMemo,
                        Debet = GlTransH.Debet,
                        Kredit = GlTransH.Kredit,
                        Saldo = GlTransH.Saldo,
                        FcComKode = kodeCompany,
                        FcTransDs = new List<FcTransD>()

                    };
                    foreach (var detail in GlTransH.FcTransDs)
                    {
                        fcGltransH.FcTransDs.Add(new FcTransD()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit,
                            Jumlah = detail.Jumlah

                        });
                    }
                    FCGlTransH.Add(fcGltransH);
                }

                if (TransHFcGL.Any())
                {
                    foreach (var FcGLTransHs in TransHFcGL)
                    {
                        FcTransH fcGltransH = new()
                        {
                            Tanggal = FcGLTransHs.Tanggal,
                            DocNo = FcGLTransHs.DocNo,
                            KodeGl = FcGLTransHs.KodeGl,
                            GlMemo = FcGLTransHs.GlMemo,
                            Debet = FcGLTransHs.Debet,
                            Kredit = FcGLTransHs.Kredit,
                            Saldo = FcGLTransHs.Saldo,
                            FcComKode = FcGLTransHs.FcComKode,
                            FcTransDs = new List<FcTransD>()

                        };
                        foreach (var detail in FcGLTransHs.FcGlTransDs)
                        {
                            fcGltransH.FcTransDs.Add(new FcTransD()
                            {
                                GlAcct = detail.GlAcct,
                                FcComKode = detail.FcComKode,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit,
                                Jumlah = detail.Jumlah

                            });
                        }
                        FCGlTransH.Add(fcGltransH);
                    }
                }
            }

            foreach (var gltrans in FCGlTransH)
            {
                switch (gltrans.Tanggal.Month)
                {
                    case 1:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc1 += detail.Jumlah;
                        }
                        break;
                    case 2:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc2 += detail.Jumlah;
                        }
                        break;
                    case 3:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc3 += detail.Jumlah;
                        }
                        break;
                    case 4:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc4 += detail.Jumlah;
                        }
                        break;
                    case 5:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc5 += detail.Jumlah;
                        }
                        break;
                    case 6:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc6 += detail.Jumlah;
                        }
                        break;
                    case 7:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc7 += detail.Jumlah;
                        }
                        break;
                    case 8:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc8 += detail.Jumlah;
                        }
                        break;
                    case 9:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc9 += detail.Jumlah;
                        }
                        break;
                    case 10:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc10 += detail.Jumlah;
                        }
                        break;
                    case 11:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc11 += detail.Jumlah;
                        }
                        break;
                    case 12:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc12 += detail.Jumlah;
                        }


                        break;
                }

            }

            Accounts.ForEach(Accounts => { Accounts.GlSaldo = 0; });

            decimal mRugiLaba = 0;

            foreach (var item in Accounts.Where(x => x.GlTipe == 2))
            {
                mRugiLaba += (item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                               item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12);
            }

            foreach (var item in Accounts)
            {
                if (item.GlTipe != 2)
                {
                    item.GlSaldo = item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
                if (item.GlTipe == 3)
                {
                    item.GlSaldo = mRugiLaba + item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
            }

            _contextFC.FcAccounts.AddRange(Accounts);
            _contextFC.FcTransHs.AddRange(FCGlTransH);
            _contextFC.SaveChanges();

            return FCGlTransH;
        }


        public List<FcTransH> prosesFinancial3(int tahun, string kodeCompany)
        {
            var deleteCom = _contextFC.FcAccounts.Where(x => x.FcTahun == tahun && x.FcComKode == kodeCompany).ToList();
            var deleteTransH = _contextFC.FcTransHs.Where(x => x.FcComKode == kodeCompany && x.Tanggal.Year == tahun).ToList();

            var ComClearing = _contextFC.FcComs.Where(x => x.FcComKode == kodeCompany).FirstOrDefault();


            _contextFC.FcAccounts.RemoveRange(deleteCom);
            _contextFC.FcTransHs.RemoveRange(deleteTransH);
            _contextFC.SaveChanges();

            var addAccount = _contextGL.GlAccounts.ToList();
            List<FcAccount> Accounts = new List<FcAccount>();

            List<FcTransH> FCGlTransH = new();
            List<FcTransD> FCGlTransD = new();

            List<FcTransHView> FCTransH = new();

            foreach (var item in addAccount)
            {

                var fcaccount = new FcAccount();

                fcaccount.FcTahun = tahun;
                fcaccount.FcComKode = kodeCompany;
                fcaccount.GlAcct = item.GlAcct;
                fcaccount.GlDept = item.GlDept;
                fcaccount.GlNama = item.GlNama;
                fcaccount.GlTipe = item.GlTipe;
                fcaccount.GlStatus = item.GlStatus;

                Accounts.Add(fcaccount);
            }
            #region accountbeforetahunproses
            var BeforeCom = _contextFC.FcAccounts.Where(x => x.FcTahun == (tahun - 1) && x.FcComKode == kodeCompany).ToList();

            decimal mRetained = 0;

            if (BeforeCom.Any())
            {
                foreach (var item in Accounts)
                {
                    item.GlSldAwal = BeforeCom.Find(x => x.GlAcct == item.GlAcct).GlSaldo;
                    if (item.GlTipe == 3)
                    {
                        mRetained = item.GlSldAwal;
                    }
                }
                Accounts.Where(x => x.GlTipe == 4).FirstOrDefault().GlSldAwal += mRetained;
                Accounts.Where(x => x.GlTipe == 3).FirstOrDefault().GlSldAwal = 0;
            }

            #endregion

            if (true)
            {
                #region pembelian
                // var TransHBeli = _contextIR.IrTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                //var TransHBeli = (from trans in _contextIR.IrTransHs
                //                    join supplier in _contextAP.ApSuppls
                //                    on trans.Supplier equals supplier.Supplier
                //                    where trans.Tanggal.Year == tahun && supplier.Pajak == false
                //                    orderby trans.Tanggal, trans.NoLpb
                //                    select trans)  // Hanya memilih data dari ApTransHs
                //.ToList();

                var supplierIds = _contextAP.ApSuppls
                            .Where(supplier => supplier.Pajak == true)
                            .Select(supplier => supplier.Supplier)
                            .ToList();

                var TransHBeli = _contextIR.IrTransHs
                           .Where(trans => trans.Tanggal.Year == tahun && supplierIds.Contains(trans.Supplier))
                           .OrderBy(trans => trans.Tanggal)
                           .ThenBy(trans => trans.NoLpb)
                           .ToList();


                foreach (var item in TransHBeli)
                {

                    List<FcTransDView> FCTransD = new();

                    if (item.Jumlah != 0)
                    {
                        var IRAkunset = (from suppliers in _contextAP.ApSuppls
                                         join accts in _contextAP.ApAccts on suppliers.AcctSet equals accts.AcctSet
                                         where suppliers.Supplier == item.Supplier
                                         select new ApAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).FirstOrDefault();

                        var TransDs = _contextIR.IrTransDs.Where(x => x.IrTransHId == item.IrTransHId).ToList();


                        // Persediaan

                        foreach (var detail in TransDs)
                        {
                            if (detail.Jumlah != 0)
                            {

                                var ICAkunset = (from inventory in _contextIC.IcItems
                                                 join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                                 where inventory.ItemCode == detail.ItemCode
                                                 select new IcAcct()
                                                 {
                                                     Acct1 = accts.Acct1,
                                                     Acct2 = accts.Acct2,
                                                     Acct3 = accts.Acct3,
                                                     Acct4 = accts.Acct4,
                                                     Acct5 = accts.Acct5,
                                                     Acct6 = accts.Acct6,
                                                     AcctSet = accts.AcctSet,
                                                     Description = accts.Description
                                                 }).
                                                 FirstOrDefault();



                                // Akun IC Control //


                                var findItem = FCTransD.Find(x => x.GlAcct == ICAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (item.Kode == "82")
                                    {
                                        findItem.Debet += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Kredit += 0;

                                    }
                                    else
                                    {
                                        findItem.Kredit += detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        findItem.Debet += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct1;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                    if (item.Kode == "82")
                                    {
                                        GlTransD.Debet = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Kredit = 0;

                                    }
                                    else
                                    {
                                        GlTransD.Kredit = detail.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                        GlTransD.Debet = 0;
                                    }

                                    FCTransD.Add(GlTransD);
                                }

                            }

                        }


                        // Supplier TtlJumlah
                        // AP Control
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                }
                                else
                                {
                                    findItemTtlJumlah.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct1);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Debet += item.Jumlah * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Kredit += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemPPN.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Kredit += 0;

                                }
                                else
                                {
                                    findItemPPN.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemPPN.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct2);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ppn * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }
                        }
                        // Supplier ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransD.Find(x => x.GlAcct == IRAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "82")
                                {
                                    findItemONGKIR.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Kredit += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    findItemONGKIR.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = IRAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaSup.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(IRAkunset.Acct3);
                                if (item.Kode == "82")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Ongkos * (item.Kurs != 0 ? item.Kurs : 1);
                                    GlTransD.Debet += 0;
                                }

                                FCTransD.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "82" ? "IR-IN" : "IR-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaSup.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransD)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit



                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion
            }

            if (true)
            {
                #region penjualan
                var TransHJual = _contextOE.OeTransHs.Where(x => x.Tanggal.Year == tahun && x.Pajak == true).OrderBy(x => x.Tanggal).OrderBy(x => x.Tanggal).ThenBy(x => x.NoLpb).ToList();

                List<FcTransHView> FCTransHJual = new();


                foreach (var item in TransHJual)
                {

                    List<FcTransDView> FCTransDJual = new();

                    var OEAkunset = (from customers in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on customers.AcctSet equals accts.AcctSet
                                     where customers.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    if (item.Jumlah != 0)
                    {


                        var TransDsJual = _contextOE.OeTransDs.Where(x => x.OeTransHId == item.OeTransHId).ToList();


                        #region detailpenjualan
                        foreach (var detail in TransDsJual)
                        {
                            // Persediaan
                            var ICAkunset = (from inventory in _contextIC.IcItems
                                             join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                             where inventory.ItemCode == detail.ItemCode
                                             select new IcAcct()
                                             {
                                                 Acct1 = accts.Acct1,
                                                 Acct2 = accts.Acct2,
                                                 Acct3 = accts.Acct3,
                                                 Acct4 = accts.Acct4,
                                                 Acct5 = accts.Acct5,
                                                 Acct6 = accts.Acct6,
                                                 AcctSet = accts.AcctSet,
                                                 Description = accts.Description
                                             }).
                                                FirstOrDefault();

                            var ICCategory = (from inventory in _contextIC.IcItems
                                              join categories in _contextIC.IcCats on inventory.Category equals categories.CatCode
                                              where inventory.ItemCode == detail.ItemCode
                                              select new IcCat()
                                              {
                                                  Cat1 = categories.Cat1,
                                                  Cat2 = categories.Cat2,
                                                  Cat3 = categories.Cat3,
                                                  Cat4 = categories.Cat4,
                                                  Cat5 = categories.Cat5,
                                                  Cat6 = categories.Cat6,
                                                  CatCode = categories.CatCode,
                                                  Description = categories.Description
                                              }).
                                                FirstOrDefault();

                            // proses cost tidak dilakukan untuk menghitung nilai penjualan     
                            if (false)
                            {
                                if (detail.Cost != 0)
                                {


                                    var findItem = FCTransDJual.Find(x => x.GlAcct == ICAkunset.Acct1);


                                    if (findItem != null)
                                    {
                                        if (item.Kode == "94")
                                        {
                                            findItem.Debet += 0;
                                            findItem.Kredit += detail.Cost;

                                        }
                                        else
                                        {
                                            findItem.Kredit = 0;
                                            findItem.Debet += detail.Cost;
                                        }
                                    }
                                    else
                                    {
                                        var GlTransD = new FcTransDView();

                                        GlTransD.GlAcct = ICAkunset.Acct1;
                                        GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                        GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                        if (item.Kode == "94")
                                        {
                                            GlTransD.Debet += 0;
                                            GlTransD.Kredit += detail.Cost;

                                        }
                                        else
                                        {
                                            GlTransD.Debet += detail.Cost;
                                            GlTransD.Kredit += 0;
                                        }

                                        FCTransDJual.Add(GlTransD);
                                    }
                                }



                            }

                            // Cost Of Good Sold
                            if (false)
                            {
                                if (detail.Cost != 0)
                                {


                                    // HPP //


                                    var findItemHpp = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat1);

                                    if (findItemHpp != null)
                                    {
                                        if (item.Kode == "94")
                                        {
                                            findItemHpp.Kredit += 0;
                                            findItemHpp.Debet += detail.Cost;

                                        }
                                        else
                                        {
                                            findItemHpp.Debet = 0;
                                            findItemHpp.Kredit += detail.Cost;
                                        }
                                    }
                                    else
                                    {
                                        var GlTransD = new FcTransDView();

                                        GlTransD.GlAcct = ICCategory.Cat1;
                                        GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                        GlTransD.GlDept = GetNameAccount(ICCategory.Cat1);

                                        if (item.Kode == "94")
                                        {
                                            GlTransD.Kredit += 0;
                                            GlTransD.Debet += detail.Cost;

                                        }
                                        else
                                        {
                                            GlTransD.Kredit += detail.Cost;
                                            GlTransD.Debet += 0;
                                        }

                                        FCTransDJual.Add(GlTransD);
                                    }

                                }
                            }

                            // Sales Revenue
                            if (detail.Jumlah != 0)
                            {




                                var findItem = FCTransDJual.Find(x => x.GlAcct == ICCategory.Cat2);

                                if (findItem != null)
                                {
                                    if (item.Kode == "94")
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        findItem.Kredit = 0;
                                        findItem.Debet += detail.Jumlah;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICCategory.Cat2;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoLpb;
                                    GlTransD.GlDept = GetNameAccount(ICCategory.Cat2);
                                    if (item.Kode == "94")
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah;

                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                    FCTransDJual.Add(GlTransD);
                                }

                            }
                        }
                        #endregion

                        // Customer TtlJumlah
                        if (item.Jumlah != 0)
                        {
                            var findItemTtlJumlah = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct1);

                            if (findItemTtlJumlah != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;
                                }
                                else
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct1;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct1);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;

                                }
                                else
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Supplier PPN
                        if (item.Ppn != 0)
                        {
                            var findItemPPN = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct2);

                            if (findItemPPN != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemPPN.Kredit += item.Ppn;
                                    findItemPPN.Debet += 0;

                                }
                                else
                                {
                                    findItemPPN.Debet += item.Ppn;
                                    findItemPPN.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct2;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", PPN, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct2);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet = 0;
                                    GlTransD.Kredit = item.Ppn;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ppn;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }
                        }
                        // Customer ONGKIR
                        if (item.Ongkos != 0)
                        {
                            var findItemONGKIR = FCTransDJual.Find(x => x.GlAcct == OEAkunset.Acct3);

                            if (findItemONGKIR != null)
                            {
                                if (item.Kode == "94")
                                {
                                    findItemONGKIR.Kredit += item.Ongkos;
                                    findItemONGKIR.Debet += 0;

                                }
                                else
                                {
                                    findItemONGKIR.Debet += item.Ongkos;
                                    findItemONGKIR.Kredit += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = OEAkunset.Acct3;
                                GlTransD.Keterangan = item.NamaCust.Trim() + ", Ongkir, " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.NoLpb;
                                GlTransD.GlDept = GetNameAccount(OEAkunset.Acct3);
                                if (item.Kode == "94")
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += item.Ongkos;

                                }
                                else
                                {
                                    GlTransD.Debet += item.Ongkos;
                                    GlTransD.Kredit += 0;
                                }

                                FCTransDJual.Add(GlTransD);
                            }

                        }
                        //  var GlTransH = new FcTransHView();

                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoLpb,
                            KodeGl = (item.Kode == "94" ? "OE-IN" : "OE-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = item.NamaCust.Trim() + ", " + (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransDJual)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion

                #region inventory
                var TransHInv = _contextIC.IcTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.NoFaktur).ToList();

                List<FcTransHView> FCTransHInv = new();


                foreach (var item in TransHInv)
                {

                    List<FcTransDView> FCTransDInv = new();

                    //      if (true)
                    //      {

                    var TransDsInv = _contextIC.IcTransDs.Where(x => x.IcTransHId == item.IcTransHId).ToList();



                    foreach (var detail in TransDsInv)
                    {

                        var ICAkunset = (from inventory in _contextIC.IcItems
                                         join accts in _contextIC.IcAccts on inventory.AcctSet equals accts.AcctSet
                                         where inventory.ItemCode == detail.ItemCode
                                         select new IcAcct()
                                         {
                                             Acct1 = accts.Acct1,
                                             Acct2 = accts.Acct2,
                                             Acct3 = accts.Acct3,
                                             Acct4 = accts.Acct4,
                                             Acct5 = accts.Acct5,
                                             Acct6 = accts.Acct6,
                                             AcctSet = accts.AcctSet,
                                             Description = accts.Description
                                         }).
                                                FirstOrDefault();



                        if (detail.Jumlah != 0)
                        {



                            // Persediaan
                            #region inventoryPersediaan
                            var findItem = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct1);

                            if (findItem != null)
                            {
                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        findItem.Debet += detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }


                                }

                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ICAkunset.Acct1;
                                GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                GlTransD.GlDept = GetNameAccount(ICAkunset.Acct1);

                                if (item.Kode == "81")
                                {
                                    if (detail.Jumlah <= 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += detail.Jumlah * -1;
                                    }
                                    else
                                    {
                                        GlTransD.Debet += detail.Jumlah;
                                        GlTransD.Kredit += 0;
                                    }

                                }


                                FCTransDInv.Add(GlTransD);
                            }
                            #endregion

                            // Adjustment

                            #region inventoryAdjustment
                            if (false)
                            {
                                var findItemAdj = FCTransDInv.Find(x => x.GlAcct == ICAkunset.Acct2);

                                if (findItemAdj != null)
                                {
                                    if (item.Kode == "81")
                                    {
                                        if (detail.Jumlah <= 0)
                                        {
                                            findItemAdj.Debet += detail.Jumlah * -1;
                                            findItemAdj.Kredit += 0;
                                        }
                                        else
                                        {
                                            findItemAdj.Debet += 0;
                                            findItemAdj.Kredit += detail.Jumlah;
                                        }


                                    }

                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ICAkunset.Acct2;
                                    GlTransD.Keterangan = detail.NamaItem + ", " + item.NoFaktur;
                                    GlTransD.GlDept = GetNameAccount(ICAkunset.Acct2);

                                    if (item.Kode == "81")
                                    {
                                        if (detail.Jumlah <= 0)
                                        {
                                            GlTransD.Debet += detail.Jumlah * -1;
                                            GlTransD.Kredit += 0;
                                        }
                                        else
                                        {
                                            GlTransD.Debet += 0;
                                            GlTransD.Kredit += detail.Jumlah;
                                        }

                                    }


                                    FCTransDInv.Add(GlTransD);
                                }
                            }
                            #endregion
                        }


                    }

                    if (FCTransDInv != null)
                    {



                        FcTransHView GltransH = new()
                        {
                            DocNo = item.NoFaktur,
                            KodeGl = (item.Kode == "81" ? "IV-AD" : "IV-CN"),
                            Tanggal = item.Tanggal,
                            GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };

                        foreach (var detail in FCTransDInv)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                        //      }
                    }
                }

                #endregion
            }

            if (true)
            {
                #region Hutang
                //     var TransHHutang = _contextAP.ApTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();
                var supplierIds = _contextAP.ApSuppls
                                 .Where(supplier => supplier.Pajak == true)
                                 .Select(supplier => supplier.Supplier)
                                 .ToList();

                var TransHHutang = _contextAP.ApTransHs
                             .Where(trans => trans.Tanggal.Year == tahun && supplierIds.Contains(trans.Supplier))
                             .OrderBy(trans => trans.Tanggal)
                             .ThenBy(trans => trans.Bukti)
                             .ToList();

                //    var TransHHutang = (from trans in _contextAP.ApTransHs
                //                        join supplier in _contextAP.ApSuppls
                //                        on trans.Supplier equals supplier.Supplier
                //                         where trans.Tanggal.Year == tahun && supplier.Pajak == false
                //                         orderby trans.Tanggal, trans.Bukti
                //                         select trans)  // Hanya memilih data dari ApTransHs
                //         .ToList();

                List<FcTransHView> FCTransHHutang = new();


                foreach (var item in TransHHutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDHutang = new();

                    var APAkunset = (from vendors in _contextAP.ApSuppls
                                     join accts in _contextAP.ApAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Supplier == item.Supplier
                                     select new ApAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AP-IN
                    if (false)
                    {
                        if (item.Kode == "21")
                        {


                            var TransDsAPIN = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();

                            foreach (var detail in TransDsAPIN)
                            {
                                // Distribution Code
                                var DistAkunset = (from distribution in _contextAP.ApDists
                                                   where distribution.DistCode == detail.DistCode
                                                   select new ApDist()
                                                   {
                                                       Dist1 = distribution.Dist1,
                                                       Description = distribution.Description
                                                   }).
                                                    FirstOrDefault();


                                if (detail.Jumlah != 0)
                                {


                                    var findItem = FCTransDHutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                    if (findItem != null)
                                    {
                                        if (item.Jumlah > 0)
                                        {
                                            findItem.Kredit += 0;
                                            findItem.Debet += detail.Jumlah;

                                        }
                                        else if (item.Jumlah < 0)
                                        {
                                            findItem.Debet = 0;
                                            findItem.Kredit += -1 * detail.Jumlah;
                                        }
                                    }
                                    else
                                    {
                                        var GlTransD = new FcTransDView();

                                        GlTransD.GlAcct = DistAkunset.Dist1;
                                        GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                        GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                        if (item.Jumlah > 0)
                                        {
                                            GlTransD.Kredit += 0;
                                            GlTransD.Debet += detail.Jumlah;

                                        }
                                        else if (item.Jumlah < 0)
                                        {
                                            GlTransD.Kredit += -1 * detail.Jumlah;
                                            GlTransD.Debet += 0;
                                        }

                                        if (item.Jumlah != 0)
                                            FCTransDHutang.Add(GlTransD);
                                    }

                                }



                            }

                        }
                    }
                    #endregion

                    #region AP-DP
                    if (item.Kode == "23")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItem.Debet += 0;
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = APAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDHutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AP-PY
                    if (item.Kode == "24")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsAPPY = _contextAP.ApTransDs.Where(x => x.ApTransHId == item.ApTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsAPPY)
                        {
                            decimal detailKurs = _contextAP.ApHutangs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            if (item.Kurs != 0)
                                detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "21" || detail.KodeTran == "82" || detail.KodeTran == "83")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "23")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        findItem.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += 0;
                                        findItem2.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += 0;
                                        findItem2.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = APAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(APAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDHutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == ComClearing.GlAcct3);

                        if (item.Kode == "21")
                            findItemTtlJumlah = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    findItemTtlJumlah.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct3;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct3);

                            if (item.Kode == "21")
                            {
                                GlTransD.GlAcct = APAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(APAkunset.Acct1);
                            }

                            if (item.Kode == "21")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += item.Jumlah;
                                    GlTransD.Debet += 0;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += -1 * item.Jumlah;
                                }

                            }

                            if (item.Kode == "23")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                            }

                            if (item.Kode == "24")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Debet += 0;

                                    xKredit += (item.Kurs != 0 ? item.Nilai : item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                    GlTransD.Kredit += 0;

                                    xDebet += -1 * (item.Kurs != 0 ? item.Nilai : item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDHutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDHutang.Find(x => x.GlAcct == APAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = APAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(APAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDHutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "21" ? "AP-IN" : (item.Kode == "23" ? "AP-DP" : "AP-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDHutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }







                #endregion
            }


            if (true)
            {
                #region Piutang
                var TransHPiutang = _contextAR.ArTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.Bukti).ToList();

                List<FcTransHView> FCTransHPiutang = new();


                foreach (var item in TransHPiutang)
                {
                    decimal xDebet = 0;
                    decimal xKredit = 0;


                    List<FcTransDView> FCTransDPiutang = new();

                    var ARAkunset = (from vendors in _contextAR.ArCusts
                                     join accts in _contextAR.ArAccts on vendors.AcctSet equals accts.AcctSet
                                     where vendors.Customer == item.Customer
                                     select new ArAcct()
                                     {
                                         Acct1 = accts.Acct1,
                                         Acct2 = accts.Acct2,
                                         Acct3 = accts.Acct3,
                                         Acct4 = accts.Acct4,
                                         Acct5 = accts.Acct5,
                                         Acct6 = accts.Acct6,
                                         AcctSet = accts.AcctSet,
                                         Description = accts.Description
                                     }).FirstOrDefault();

                    #region AR-IN
                    if (item.Kode == "11")
                    {


                        var TransDsARIN = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();

                        foreach (var detail in TransDsARIN)
                        {
                            // Distribution Code
                            var DistAkunset = (from distribution in _contextAR.ArDists
                                               where distribution.DistCode == detail.DistCode
                                               select new ApDist()
                                               {
                                                   Dist1 = distribution.Dist1,
                                                   Description = distribution.Description
                                               }).
                                                FirstOrDefault();


                            if (detail.Jumlah != 0)
                            {


                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == DistAkunset.Dist1);

                                if (findItem != null)
                                {
                                    if (item.Jumlah > 0)
                                    {
                                        findItem.Kredit += detail.Jumlah;
                                        findItem.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        findItem.Debet = -1 * detail.Jumlah;
                                        findItem.Kredit += 0;
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = DistAkunset.Dist1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(DistAkunset.Dist1);

                                    if (item.Jumlah > 0)
                                    {
                                        GlTransD.Kredit += detail.Jumlah;
                                        GlTransD.Debet += 0;

                                    }
                                    else if (item.Jumlah < 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += -1 * detail.Jumlah;
                                    }

                                    if (item.Jumlah != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                            }



                        }

                    }

                    #endregion

                    #region AR-DP
                    if (item.Kode == "13")
                    {


                        if (item.Jumlah != 0)
                        {


                            var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                            if (findItem != null)
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItem.Debet += 0;
                                    findItem.Kredit += (item.Jumlah); ;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItem.Kredit = 0;
                                    findItem.Debet += -1 * (item.Jumlah);
                                }
                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = ARAkunset.Acct4;
                                GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += -1 * (item.Jumlah);
                                    GlTransD.Kredit += 0;
                                }

                                if (item.Jumlah != 0)
                                    FCTransDPiutang.Add(GlTransD);
                            }

                        }

                    }
                    #endregion

                    #region AR-PY
                    if (item.Kode == "14")
                    {

                        xDebet = 0;
                        xKredit = 0;

                        var TransDsARPY = _contextAR.ArTransDs.Where(x => x.ArTransHId == item.ArTransHId).ToList();


                        #region detailPembayaran
                        foreach (var detail in TransDsARPY)
                        {
                            //  decimal detailKurs = _contextAR.ArPiutngs.Where(x => x.Dokumen == detail.Lpb).FirstOrDefault().Kurs;

                            decimal detailKurs = 0;

                            //   if (item.Kurs != 0)
                            //      detailKurs = item.Kurs;

                            // Distribution Code
                            //var DistAkunset = (from distribution in _contextAP.ApDists
                            //                   where distribution.DistCode == detail.DistCode
                            //                   select new ApDist()
                            //                   {
                            //                       Dist1 = distribution.Dist1,
                            //                       Description = distribution.Description
                            //                   }).
                            //                    FirstOrDefault();

                            // Transaksi AP-IN or IR-IN
                            if (detail.KodeTran == "11" || detail.KodeTran == "94" || detail.KodeTran == "95")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct1;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                            // Transaksi AP-DP
                            if (detail.KodeTran == "13")
                            {

                                // bila ada pembayaran

                                var findItem = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct4);

                                if (findItem != null)
                                {
                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        findItem.Debet += 0;
                                        findItem.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        findItem.Kredit += 0;
                                        findItem.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct4;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct4);

                                    if (detail.Bayar + detail.Discount > 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                        xKredit += (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);

                                    }
                                    else if (detail.Bayar + detail.Discount < 0)
                                    {
                                        GlTransD.Debet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += -1 * (detailKurs != 0 ? (detail.Bayar + detail.Discount) * detailKurs : detail.Bayar + detail.Discount);
                                    }

                                    if (detail.Bayar + detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }

                                // bila ada discount

                                var findItem2 = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct5);

                                if (findItem2 != null)
                                {
                                    if (detail.Discount > 0)
                                    {
                                        findItem2.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        findItem2.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        findItem2.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }
                                }
                                else
                                {
                                    var GlTransD = new FcTransDView();

                                    GlTransD.GlAcct = ARAkunset.Acct5;
                                    GlTransD.Keterangan = detail.Keterangan + ", " + item.Bukti;
                                    GlTransD.GlDept = GetNameAccount(ARAkunset.Acct5);

                                    if (detail.Discount > 0)
                                    {
                                        GlTransD.Debet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Kredit += 0;

                                        xDebet += (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);

                                    }
                                    else if (detail.Discount < 0)
                                    {
                                        GlTransD.Kredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                        GlTransD.Debet += 0;

                                        xKredit += -1 * (detailKurs != 0 ? (detail.Discount) * detailKurs : detail.Discount);
                                    }

                                    if (detail.Discount != 0)
                                        FCTransDPiutang.Add(GlTransD);
                                }
                            }

                        }
                        #endregion
                    }

                    #endregion

                    #region Header
                    if (true)
                    {
                        var findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ComClearing.GlAcct2);

                        if (item.Kode == "11")
                            findItemTtlJumlah = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct1);

                        if (findItemTtlJumlah != null)
                        {
                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += item.Jumlah;

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Kredit += -1 * item.Jumlah;
                                    findItemTtlJumlah.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet = 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    findItemTtlJumlah.Kredit += 0;
                                    findItemTtlJumlah.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);


                                }
                                else if (item.Jumlah < 0)
                                {
                                    findItemTtlJumlah.Debet += 0;
                                    findItemTtlJumlah.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ComClearing.GlAcct2;
                            GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ComClearing.GlAcct2);

                            if (item.Kode == "11")
                            {
                                GlTransD.GlAcct = ARAkunset.Acct1;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.Bukti;
                                GlTransD.GlDept = GetNameAccount(ARAkunset.Acct1);
                            }

                            if (item.Kode == "11")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += item.Jumlah;
                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Kredit += -1 * item.Jumlah;
                                    GlTransD.Debet += 0;
                                }

                            }

                            if (item.Kode == "13")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Kode == "14")
                            {
                                if (item.Jumlah > 0)
                                {
                                    GlTransD.Kredit += 0;
                                    GlTransD.Debet += (item.Jumlah);

                                    xDebet += (item.Jumlah);

                                }
                                else if (item.Jumlah < 0)
                                {
                                    GlTransD.Debet += 0;
                                    GlTransD.Kredit += -1 * (item.Jumlah);

                                    xKredit += -1 * (item.Jumlah);
                                }

                            }

                            if (item.Jumlah != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }
                    }
                    #endregion

                    #region SelisihKurs

                    if (xDebet - xKredit != 0)
                    {
                        var findItemKurs = FCTransDPiutang.Find(x => x.GlAcct == ARAkunset.Acct6);

                        if (findItemKurs != null)
                        {
                            if (xDebet - xKredit > 0)
                            {
                                findItemKurs.Kredit += (xDebet - xKredit);
                                findItemKurs.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                findItemKurs.Debet = -1 * (xDebet - xKredit);
                                findItemKurs.Kredit += 0;
                            }
                        }
                        else
                        {
                            var GlTransD = new FcTransDView();

                            GlTransD.GlAcct = ARAkunset.Acct6;
                            GlTransD.Keterangan = item.Keterangan + ", " + item.Bukti;
                            GlTransD.GlDept = GetNameAccount(ARAkunset.Acct6);

                            if (xDebet - xKredit > 0)
                            {
                                GlTransD.Kredit += (xDebet - xKredit);
                                GlTransD.Debet += 0;

                            }
                            else if (xDebet - xKredit < 0)
                            {
                                GlTransD.Kredit += 0;
                                GlTransD.Debet += -1 * (xDebet - xKredit);
                            }

                            if (xDebet - xKredit != 0)
                                FCTransDPiutang.Add(GlTransD);
                        }

                    }

                    #endregion

                    FcTransHView GltransH = new()
                    {
                        DocNo = item.Bukti,
                        KodeGl = item.Kode == "11" ? "AR-IN" : (item.Kode == "13" ? "AR-DP" : "AR-PY"),
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in FCTransDPiutang)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);

                }

                #endregion
            }

            if (true)
            {
                #region KasBank
                var TransHCB = _contextCB.CbTransHs.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                List<FcTransHView> FCTransHCB = new();


                foreach (var item in TransHCB)
                {
                    var bankPajak = _contextCB.CbBanks.Where(x => x.KodeBank == item.KodeBank).FirstOrDefault().Pajak;

                    if (!bankPajak)
                    {
                        List<FcTransDView> FCTransDCB = new();

                        var CBAkunset = (from banks in _contextCB.CbBanks
                                         where banks.KodeBank == item.KodeBank
                                         select banks).FirstOrDefault();




                        #region KasbankDetail
                        if (true)
                        {



                            var TransDsCB = _contextCB.CbTransDs.Where(x => x.CbTransHId == item.CbTransHId).ToList();


                            #region detailPembayaran
                            foreach (var detail in TransDsCB)
                            {
                                // Distribution Code
                                var DistAkunset = (from distribution in _contextCB.CbSrcCodes
                                                   where distribution.SrcCode == detail.SrcCode
                                                   select distribution).FirstOrDefault();

                                // Transaksi Kasbank
                                if (true)
                                {
                                    //if (DistAkunset == null)
                                    //{
                                    //    var test = item.DocNo;
                                    //    var test2 = item.Keterangan;
                                    //}

                                    //if(DistAkunset.GlAcct == null)
                                    //{
                                    //    var test = item.DocNo;
                                    //    var test2 = item.Keterangan;
                                    //}

                                    var findItem = FCTransDCB.Find(x => x.GlAcct == DistAkunset.GlAcct);

                                    if (findItem != null)
                                    {
                                        if (detail.Jumlah > 0)
                                        {
                                            findItem.Debet += 0;
                                            findItem.Kredit += detail.Jumlah;


                                        }
                                        else if (detail.Jumlah < 0)
                                        {
                                            findItem.Kredit += 0;
                                            findItem.Debet += -1 * (detail.Jumlah);


                                        }
                                    }
                                    else
                                    {
                                        var GlTransD = new FcTransDView();

                                        GlTransD.GlAcct = DistAkunset.GlAcct;
                                        GlTransD.Keterangan = detail.Keterangan + ", " + item.DocNo;
                                        GlTransD.GlDept = GetNameAccount(DistAkunset.GlAcct);

                                        if (detail.Jumlah > 0)
                                        {
                                            GlTransD.Debet += 0;
                                            GlTransD.Kredit += detail.Jumlah;



                                        }
                                        else if (detail.Jumlah < 0)
                                        {
                                            GlTransD.Debet += -1 * (detail.Jumlah);
                                            GlTransD.Kredit += 0;

                                        }
                                        if (detail.Jumlah != 0)
                                            FCTransDCB.Add(GlTransD);
                                    }

                                }


                            }
                            #endregion
                        }

                        #endregion

                        #region Header
                        if (true)
                        {
                            var findItemTtlJumlah = FCTransDCB.Find(x => x.GlAcct == CBAkunset.Acctset);


                            if (findItemTtlJumlah != null)
                            {
                                if (true)
                                {
                                    if (item.Saldo > 0)
                                    {
                                        findItemTtlJumlah.Kredit += 0;
                                        findItemTtlJumlah.Debet += item.Saldo;

                                    }
                                    else if (item.Saldo < 0)
                                    {
                                        findItemTtlJumlah.Kredit += -1 * item.Saldo;
                                        findItemTtlJumlah.Debet += 0;
                                    }

                                }



                            }
                            else
                            {
                                var GlTransD = new FcTransDView();

                                GlTransD.GlAcct = CBAkunset.Acctset;
                                GlTransD.Keterangan = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan) + ", " + item.DocNo;
                                GlTransD.GlDept = GetNameAccount(CBAkunset.Acctset);


                                if (true)
                                {
                                    if (item.Saldo > 0)
                                    {
                                        GlTransD.Kredit += 0;
                                        GlTransD.Debet += (item.Saldo);

                                    }
                                    else if (item.Saldo < 0)
                                    {
                                        GlTransD.Debet += 0;
                                        GlTransD.Kredit += -1 * (item.Saldo);
                                    }

                                }



                                if (item.Saldo != 0)
                                    FCTransDCB.Add(GlTransD);
                            }
                        }
                        #endregion




                        FcTransHView GltransH = new()
                        {
                            DocNo = item.DocNo,
                            KodeGl = "CB-" + item.KodeBank.Trim(),
                            Tanggal = item.Tanggal,
                            GlMemo = (string.IsNullOrEmpty(item.Keterangan) ? " " : item.Keterangan),
                            FcTransDs = new List<FcTransDView>()
                        };
                        foreach (var detail in FCTransDCB)
                        {
                            GltransH.FcTransDs.Add(new FcTransDView()
                            {
                                GlAcct = detail.GlAcct,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit

                            });
                        }

                        FCTransH.Add(GltransH);
                    }
                }

                #endregion
            }

            if (true)
            {
                #region Asset

                var TransAS = _contextAS.AsTransaksis.Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.BarcodeAssets).ToList();
                List<FcTransDView> GlTransd = new();
                FcTransHView GltransH = new();


                foreach (var item in TransAS)
                {
                    GlTransd.Clear();

                    var barcode = (from account in _contextAS.AsAssetss
                                   where account.BarcodeAssets == item.BarcodeAssets
                                   select account).FirstOrDefault();

                    var Akunset = (from account in _contextAS.AsAcctsets
                                   where account.AcctSet == barcode.Acctset
                                   select account).FirstOrDefault();

                    var Distribution = (from distribution in _contextAS.AsDistSets
                                        where distribution.DistCode == barcode.DistCode
                                        select distribution).FirstOrDefault();

                    if (item.Kode == "01")   // pembelian
                    {
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Akunset.Acct1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Akunset.Acct1),
                            Debet = 0,
                            Kredit = item.Nilai,


                        });
                        GlTransd.Add(new FcTransDView()
                        {
                            GlAcct = Distribution.Dist1,
                            Keterangan = barcode.NamaBarang,
                            GlDept = GetNameAccount(Distribution.Dist1),
                            Debet = item.Nilai,
                            Kredit = 0,


                        });
                    }
                    //if (item.Kode == "02")  // penjualan
                    //{
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Akunset.Acct1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = item.Nilai,
                    //        Kredit = 0,


                    //    });
                    //    GlTransd.Add(new FcTransDView()
                    //    {
                    //        GlAcct = Distribution.Dist1,
                    //        Keterangan = barcode.NamaBarang,
                    //        GlDept = "",
                    //        Debet = 0,
                    //        Kredit = item.Nilai,


                    //    });
                    //}

                    GltransH = new()
                    {
                        DocNo = item.BarcodeAssets + item.Tanggal.ToString("yyyyMM"),
                        KodeGl = "GL-AS",
                        Tanggal = item.Tanggal,
                        GlMemo = (string.IsNullOrEmpty(item.BarcodeAssets) ? " " : item.BarcodeAssets + item.Tanggal.ToString("yyyyMM")),
                        FcTransDs = GlTransd
                    };

                    FCTransH.Add(GltransH);
                }
                #endregion
            }

            if (true)
            {
                #region generalLedger

                var TransGL = _contextGL.GlTransHs.Include(p => p.GlTransDs).Where(x => x.Tanggal.Year == tahun).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

                foreach (var item in TransGL)
                {
                    FcTransHView GltransH = new()
                    {
                        DocNo = item.DocNo,
                        KodeGl = item.KodeGl,
                        Tanggal = item.Tanggal,
                        GlMemo = item.GlMemo,
                        FcTransDs = new List<FcTransDView>()
                    };
                    foreach (var detail in item.GlTransDs)
                    {
                        GltransH.FcTransDs.Add(new FcTransDView()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = GetNameAccount(detail.GlAcct),
                            Debet = detail.Debet,
                            Kredit = detail.Kredit

                        });
                    }

                    FCTransH.Add(GltransH);
                }

                #endregion
            }

            // FcGLTransaksi General Ledger       
            var TransHFcGL = _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.Tanggal.Year == tahun && x.FcComKode == kodeCompany).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo).ToList();

            // Semua hasil proses dimasukkan ke FC Ledger  // 
            if (true)
            {
                foreach (var GlTransH in FCTransH)
                {
                    FcTransH fcGltransH = new()
                    {
                        Tanggal = GlTransH.Tanggal,
                        DocNo = GlTransH.DocNo,
                        KodeGl = GlTransH.KodeGl,
                        GlMemo = GlTransH.GlMemo,
                        Debet = GlTransH.Debet,
                        Kredit = GlTransH.Kredit,
                        Saldo = GlTransH.Saldo,
                        FcComKode = kodeCompany,
                        FcTransDs = new List<FcTransD>()

                    };
                    foreach (var detail in GlTransH.FcTransDs)
                    {
                        fcGltransH.FcTransDs.Add(new FcTransD()
                        {
                            GlAcct = detail.GlAcct,
                            Keterangan = detail.Keterangan,
                            GlDept = detail.GlDept,
                            Debet = detail.Debet,
                            Kredit = detail.Kredit,
                            Jumlah = detail.Jumlah

                        });
                    }
                    FCGlTransH.Add(fcGltransH);
                }

                if (TransHFcGL.Any())
                {
                    foreach (var FcGLTransHs in TransHFcGL)
                    {
                        FcTransH fcGltransH = new()
                        {
                            Tanggal = FcGLTransHs.Tanggal,
                            DocNo = FcGLTransHs.DocNo,
                            KodeGl = FcGLTransHs.KodeGl,
                            GlMemo = FcGLTransHs.GlMemo,
                            Debet = FcGLTransHs.Debet,
                            Kredit = FcGLTransHs.Kredit,
                            Saldo = FcGLTransHs.Saldo,
                            FcComKode = FcGLTransHs.FcComKode,
                            FcTransDs = new List<FcTransD>()

                        };
                        foreach (var detail in FcGLTransHs.FcGlTransDs)
                        {
                            fcGltransH.FcTransDs.Add(new FcTransD()
                            {
                                GlAcct = detail.GlAcct,
                                FcComKode = detail.FcComKode,
                                Keterangan = detail.Keterangan,
                                GlDept = detail.GlDept,
                                Debet = detail.Debet,
                                Kredit = detail.Kredit,
                                Jumlah = detail.Jumlah

                            });
                        }
                        FCGlTransH.Add(fcGltransH);
                    }
                }
            }

            foreach (var gltrans in FCGlTransH)
            {
                switch (gltrans.Tanggal.Month)
                {
                    case 1:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc1 += detail.Jumlah;
                        }
                        break;
                    case 2:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc2 += detail.Jumlah;
                        }
                        break;
                    case 3:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc3 += detail.Jumlah;
                        }
                        break;
                    case 4:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc4 += detail.Jumlah;
                        }
                        break;
                    case 5:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc5 += detail.Jumlah;
                        }
                        break;
                    case 6:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc6 += detail.Jumlah;
                        }
                        break;
                    case 7:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc7 += detail.Jumlah;
                        }
                        break;
                    case 8:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc8 += detail.Jumlah;
                        }
                        break;
                    case 9:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc9 += detail.Jumlah;
                        }
                        break;
                    case 10:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc10 += detail.Jumlah;
                        }
                        break;
                    case 11:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc11 += detail.Jumlah;
                        }
                        break;
                    case 12:
                        foreach (var detail in gltrans.FcTransDs)
                        {
                            Accounts.Find(x => x.GlAcct == detail.GlAcct).GlFisc12 += detail.Jumlah;
                        }


                        break;
                }

            }

            Accounts.ForEach(Accounts => { Accounts.GlSaldo = 0; });

            decimal mRugiLaba = 0;

            foreach (var item in Accounts.Where(x => x.GlTipe == 2))
            {
                mRugiLaba += (item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                               item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12);
            }

            foreach (var item in Accounts)
            {
                if (item.GlTipe != 2)
                {
                    item.GlSaldo = item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
                if (item.GlTipe == 3)
                {
                    item.GlSaldo = mRugiLaba + item.GlSldAwal + item.GlFisc1 + item.GlFisc2 + item.GlFisc3 + item.GlFisc4 + item.GlFisc5 + item.GlFisc6 +
                              item.GlFisc7 + item.GlFisc8 + item.GlFisc9 + item.GlFisc10 + item.GlFisc11 + item.GlFisc12;
                }
            }

            _contextFC.FcAccounts.AddRange(Accounts);
            _contextFC.FcTransHs.AddRange(FCGlTransH);
            _contextFC.SaveChanges();

            return FCGlTransH;
        }

        public List<FcGlTransH> GetTransHFc()
        {
            List<FcGlTransH> arTrans = new List<FcGlTransH>();
            try
            {
                arTrans = _contextFC.FcGlTransHs.OrderByDescending(x => x.Tanggal).ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return arTrans;

        }

        public List<FcGlTransD> GetTransDFc()
        {
            return _contextFC.FcGlTransDs.ToList();
        }

        public async Task<bool> DelTransHFc(int id)
        {
            try
            {
                var ExistingTrans = _contextFC.FcGlTransHs.Where(x => x.FcGlTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {

                    _contextFC.FcGlTransHs.Remove(ExistingTrans);
                    //    _context.ArPiutngs.Remove(cekFirst);
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public FcGlTransH GetTrans(int id)
        {
            return _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.FcGlTransHId == id).FirstOrDefault();
        }

        public FcGlTransH AddTransH(FcTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            FcGlTransH transH = new()
            {
                DocNo = GetNumber(),
                FcComKode = trans.FcComKode,
                KodeGl = String.IsNullOrEmpty(trans.KodeGl) ? " " : trans.KodeGl.ToUpper(),
                Tanggal = trans.Tanggal,
                GlMemo = trans.GlMemo,
                // Kurs = trans.Kurs,
                Debet = trans.Debet,
                Kredit = trans.Kredit,
                Saldo = trans.Saldo,
                FcGlTransDs = new List<FcGlTransD>()
            };
            foreach (var item in trans.FcTransDs)
            {
                transH.FcGlTransDs.Add(new FcGlTransD()
                {
                    GlAcct = item.GlAcct,
                    FcComKode = trans.FcComKode,
                    Keterangan = item.Keterangan,
                    Debet = item.Debet,
                    Kredit = item.Kredit,
                    Jumlah = item.Jumlah,



                });
            }

            _contextFC.FcGlTransHs.Add(transH);
            _contextFC.SaveChanges();

            var TempTrans = GetTransDoc(transH.DocNo);

            return TempTrans;
            // return true;


        }

        public FcGlTransH EditTransH(FcTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();



            try
            {
                var ExistingTrans = _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.FcGlTransHId == trans.GlTransHId).FirstOrDefault();
                if (ExistingTrans != null)
                {

                    _contextFC.FcGlTransHs.Remove(ExistingTrans);


                    /* update */

                    FcGlTransH transH = new()
                    {
                        //  transH.DocNo = ExistingTrans.DocNo;
                        DocNo = ExistingTrans.DocNo,
                        FcComKode = trans.FcComKode,
                        KodeGl = String.IsNullOrEmpty(trans.KodeGl) ? " " : trans.KodeGl.ToUpper(),
                        Tanggal = trans.Tanggal,
                        GlMemo = trans.GlMemo,
                        Kurs = trans.Kurs,
                        Kredit = trans.Kredit,
                        Debet = trans.Debet,
                        Saldo = trans.Saldo,

                        FcGlTransDs = new List<FcGlTransD>()
                    };
                    foreach (var item in trans.FcTransDs)
                    {
                        transH.FcGlTransDs.Add(new FcGlTransD()
                        {
                            GlAcct = item.GlAcct,
                            FcComKode = trans.FcComKode,
                            Keterangan = item.Keterangan,
                            Debet = item.Debet,
                            Kredit = item.Kredit,
                            Jumlah = item.Jumlah

                        });
                    }

                    _contextFC.FcGlTransHs.Add(transH);
                    _contextFC.SaveChanges();

                    return transH;
                    //   return true;
                }
                else
                {
                    return ExistingTrans;
                }
            }
            catch (Exception)
            {
                throw;
            }




        }
        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _contextFC.FcTransHs.Where(x => x.FcTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    //  var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                    //  var customer = (from e in _context.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                    //   customer.Piutang -= ExistingTrans.Jumlah;


                    //   _context.ArCusts.Update(customer);
                    _contextFC.FcTransHs.Remove(ExistingTrans);
                    //    _context.ArPiutngs.Remove(cekFirst);
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public FcTransH GetTransGL(int id)
        {
            return _contextFC.FcTransHs.Include(p => p.FcTransDs).Where(x => x.FcTransHId == id).FirstOrDefault();
        }
        public List<FcTransH> GetTransH(int Tahun)
        {
            List<FcTransH> arTrans = new List<FcTransH>();
            try
            {
                arTrans = _contextFC.FcTransHs.Where(x => x.Tanggal.Year == Tahun).OrderByDescending(x => x.Tanggal).ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return arTrans;

        }

        public List<FcTransD> GetTransD()
        {
            return _contextFC.FcTransDs.ToList();
        }
        public FcGlTransH GetTransDoc(string docno)
        {
            return _contextFC.FcGlTransHs.Include(p => p.FcGlTransDs).Where(x => x.DocNo == docno).FirstOrDefault();
        }
        public string GetNumber()
        {
            string kodeno = "FCJ";
            string kodeurut = kodeno + "-";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _contextFC.FcGlTransHs.Where(x => x.DocNo.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.DocNo);

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

        #region PrintFC Class

        public bool CekKdPrintTP(string item)
        {
            string test = item.ToUpper();
            var cekFirst = _contextFC.FcPrintTPs.Where(x => x.KodeCetak == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<FcPrintTP> GetPrintTP()
        {
            return _contextFC.FcPrintTPs.OrderBy(x => x.KodeCetak).ToList();
        }

        public FcPrintTP GetPrintTPId(int id)
        {
            return _contextFC.FcPrintTPs.Where(x => x.FcPrintTPId == id).FirstOrDefault();
        }

        public FcPrintTP GetPrintTPKode(string id)
        {
            return _contextFC.FcPrintTPs.Where(x => x.KodeCetak == id).FirstOrDefault();
        }

        public bool AddPrintTP(FcPrintTPView codeview)
        {
            string test = codeview.KodeCetak.ToUpper();
            var cekFirst = _contextFC.FcPrintTPs.Where(x => x.KodeCetak == test).ToList();
            if (cekFirst.Count == 0)
            {
                FcPrintTP Location = new FcPrintTP()
                {
                    KodeCetak = codeview.KodeCetak.ToUpper(),
                    NamaCetak = codeview.NamaCetak,
                    JnsReport = "",
                    FcComKode = ""

                };
                _contextFC.FcPrintTPs.Add(Location);
                _contextFC.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditPrintTP(FcPrintTPView codeview)
        {
            try
            {
                var ExistingDiv = _contextFC.FcPrintTPs.Where(x => x.FcPrintTPId == codeview.FcPrintTPId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaCetak = codeview.NamaCetak;


                    _contextFC.FcPrintTPs.Update(ExistingDiv);
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelPrintTP(int codeview)
        {
            try
            {
                var ExistingDiv = _contextFC.FcPrintTPs.Where(x => x.FcPrintTPId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    _contextFC.FcPrintTPs.Remove(ExistingDiv);
                    await _contextFC.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public List<FcPrintGl> GetPrintGLPKode(string id)
        {
            return _contextFC.FcPrintGls.Where(x => x.KodeCetak == id).ToList();
        }

        public bool SavePrintGL(List<FcPrintGl> codeview, string KodeCetak)
        {
            foreach (var glPrint in codeview)
            {
                glPrint.KodeCetak = KodeCetak;
            }

            var deleteGL = _contextFC.FcPrintGls.Where(x => x.KodeCetak == KodeCetak).ToList();
            if (deleteGL != null)
            {
                _contextFC.FcPrintGls.RemoveRange(deleteGL);
                _contextFC.FcPrintGls.UpdateRange(codeview);
            }
            else
            {
                _contextFC.FcPrintGls.AddRange(codeview);
            }


            _contextFC.SaveChanges();


            return true;

        }
        #endregion PrintFC Class

        #region printNeraca
        public List<FcPrintGLView> printNeraca(string kodeCetak, string company, int Bulan, int Tahun)
        {
            try
            {
                var listPrintSql = _contextFC.FcPrintGls.Where(x => x.KodeCetak == kodeCetak).ToList();
                //  var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && s).ToList();

                List<FcPrintGl> listPrint = new();

                listPrint.AddRange(listPrintSql);



                List<FcPrintGLView> TransFinancial = new();

                foreach (var glView in listPrintSql)
                {
                    TransFinancial.Add(
                        new FcPrintGLView()
                        {
                            KodeCetak = glView.KodeCetak,
                            NoBaris = glView.NoBaris,
                            Keterangan = glView.Keterangan,
                            NoRek1 = glView.NoRek1,
                            CetakDetil = glView.CetakDetil,
                            CetakGaris1 = glView.CetakGaris1,
                            CetakGaris2 = glView.CetakGaris2,
                            CetakBln1 = glView.CetakBln1,
                            CetakBln2 = glView.CetakBln2,
                            Spasi = glView.Spasi,
                            CetakTebal = glView.CetakTebal,
                            CetakHide = glView.CetakHide,
                            CetakNegatif = glView.CetakNegatif,
                            RumusBaris = glView.RumusBaris
                        });
                }
                //   TransFinancial.AddRange(listPrint);

                foreach (var item in listPrint)
                {
                    if (!string.IsNullOrEmpty(item.NoRek1))
                    {
                        item.JumRekap1 = NeracaTransBulan1(item.NoRek1, company, Bulan, Tahun);

                    }
                    if (item.CetakDetil)
                    {
                        if (!string.IsNullOrEmpty(item.NoRek1))
                        {
                            string[] selectedList = item.NoRek1.Split("-").ToArray();

                            int counting = selectedList.Count();
                            int i = 0;
                            foreach (var detail in selectedList)
                            {
                                i++;
                                if (i == counting)
                                {
                                    TransFinancial.Add(new FcPrintGLView()
                                    {
                                        NoBaris = item.NoBaris,

                                        Keterangan = GetNameAccount(detail),

                                        CetakGaris1 = item.CetakGaris1,
                                        CetakGaris2 = item.CetakGaris2,
                                        CetakHide = item.CetakHide,
                                        CetakNegatif = item.CetakNegatif,
                                        Spasi = 5,

                                        JumTran1 = NeracaDetailTransBulan1(detail, company, Bulan, Tahun)


                                    });
                                }
                                else
                                {
                                    TransFinancial.Add(new FcPrintGLView()
                                    {
                                        NoBaris = item.NoBaris,
                                        Keterangan = GetNameAccount(detail),

                                        CetakHide = item.CetakHide,
                                        CetakNegatif = item.CetakNegatif,
                                        Spasi = 5,

                                        JumTran1 = NeracaDetailTransBulan1(detail, company, Bulan, Tahun)


                                    });
                                }


                            }

                        }

                    }

                }


                foreach (var item in TransFinancial)
                {
                    if (!string.IsNullOrEmpty(item.RumusBaris))
                    {
                        string[] selectedList = item.RumusBaris.Split("+").ToArray();

                        foreach (var detail in selectedList)
                        {
                            var firstList = listPrint.Where(x => x.NoBaris == detail).FirstOrDefault();
                            item.JumRekap1 += (firstList.JumRekap1 + firstList.JumTran1);
                        }

                        listPrint.Find(x => x.NoBaris == item.NoBaris).JumRekap1 = item.JumRekap1;
                    }

                    if (!string.IsNullOrEmpty(item.NoRek1) && item.CetakDetil == false)
                    {
                        item.JumTran1 = NeracaTransBulan1(item.NoRek1, company, Bulan, Tahun);
                    }
                }

                TransFinancial = TransFinancial.OrderBy(x => x.NoBaris).ToList();


                return TransFinancial;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private decimal NeracaTransBulan1(string rekening, string company, int Bulan, int Tahun)
        {
            // string[] selectedList = new string[] { };
            var retainEarning = _contextFC.FcAccounts.Where(x => x.GlTipe == 3).FirstOrDefault();

            decimal mTotal1 = 0;
            string[] selectedList = rekening.Split("-").ToArray();
            var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && selectedList.Contains(x.GlAcct)).ToList();
            List<FcAccount> listRE = new List<FcAccount>();

            foreach (var ceking in selectedList)
            {
                if (ceking == retainEarning.GlAcct)
                {
                    listRE = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlTipe == 2).ToList();

                }
            }

            mTotal1 += listGL.Sum(x => x.GlSldAwal);

            for (int i = 0; i < Bulan; i++)
            {
                switch (i)
                {
                    case 0:
                        mTotal1 += listGL.Sum(x => x.GlFisc1);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc1);

                        break;
                    case 1:
                        mTotal1 += listGL.Sum(x => x.GlFisc2);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc2);

                        break;
                    case 2:
                        mTotal1 += listGL.Sum(x => x.GlFisc3);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc3);

                        break;
                    case 3:
                        mTotal1 += listGL.Sum(x => x.GlFisc4);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc4);

                        break;
                    case 4:
                        mTotal1 += listGL.Sum(x => x.GlFisc5);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc5);

                        break;
                    case 5:
                        mTotal1 += listGL.Sum(x => x.GlFisc6);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc6);

                        break;
                    case 6:
                        mTotal1 += listGL.Sum(x => x.GlFisc7);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc7);

                        break;
                    case 7:
                        mTotal1 += listGL.Sum(x => x.GlFisc8);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc8);

                        break;
                    case 8:
                        mTotal1 += listGL.Sum(x => x.GlFisc9);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc9);

                        break;
                    case 9:
                        mTotal1 += listGL.Sum(x => x.GlFisc10);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc10);

                        break;
                    case 10:
                        mTotal1 += listGL.Sum(x => x.GlFisc11);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc11);

                        break;
                    case 11:
                        mTotal1 += listGL.Sum(x => x.GlFisc12);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc12);

                        break;
                }
            }

            // glAccounts.Contains(rekening).
            return mTotal1;
        }
        private decimal NeracaDetailTransBulan1(string rekening, string company, int Bulan, int Tahun)
        {
            // string[] selectedList = new string[] { };
            var retainEarning = _contextFC.FcAccounts.Where(x => x.GlTipe == 3 && x.FcComKode == company && x.FcTahun == Tahun && x.GlAcct == rekening).FirstOrDefault();
            List<FcAccount> listRE = new List<FcAccount>();

            decimal mTotal1 = 0;

            if (retainEarning != null)
            {
                listRE = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlTipe == 2).ToList();
            }

            var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlAcct == rekening).ToList();

            mTotal1 += listGL.Sum(x => x.GlSldAwal);

            for (int i = 0; i < Bulan; i++)
            {
                switch (i)
                {
                    case 0:
                        mTotal1 += listGL.Sum(x => x.GlFisc1);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc1);
                        break;
                    case 1:
                        mTotal1 += listGL.Sum(x => x.GlFisc2);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc2);
                        break;
                    case 2:
                        mTotal1 += listGL.Sum(x => x.GlFisc3);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc3);
                        break;
                    case 3:
                        mTotal1 += listGL.Sum(x => x.GlFisc4);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc4);
                        break;
                    case 4:
                        mTotal1 += listGL.Sum(x => x.GlFisc5);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc5);
                        break;
                    case 5:
                        mTotal1 += listGL.Sum(x => x.GlFisc6);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc6);
                        break;
                    case 6:
                        mTotal1 += listGL.Sum(x => x.GlFisc7);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc7);
                        break;
                    case 7:
                        mTotal1 += listGL.Sum(x => x.GlFisc8);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc8);
                        break;
                    case 8:
                        mTotal1 += listGL.Sum(x => x.GlFisc9);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc9);
                        break;
                    case 9:
                        mTotal1 += listGL.Sum(x => x.GlFisc10);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc10);
                        break;
                    case 10:
                        mTotal1 += listGL.Sum(x => x.GlFisc11);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc11);
                        break;
                    case 11:
                        mTotal1 += listGL.Sum(x => x.GlFisc12);
                        if (listRE != null)
                            mTotal1 += listRE.Sum(x => x.GlFisc12);
                        break;
                }
            }

            // glAccounts.Contains(rekening).
            return mTotal1;
        }
        #endregion

        #region printRugiLaba

        public List<FcPrintGLView> printRugiLaba(string kodeCetak, string company, int Bulan, int Tahun)
        {
            try
            {
                var listPrintSql = _contextFC.FcPrintGls.Where(x => x.KodeCetak == kodeCetak).ToList();
                //  var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && s).ToList();

                List<FcPrintGl> listPrint = new();

                listPrint.AddRange(listPrintSql);



                List<FcPrintGLView> TransFinancial = new();

                foreach (var glView in listPrintSql)
                {
                    TransFinancial.Add(
                        new FcPrintGLView()
                        {
                            KodeCetak = glView.KodeCetak,
                            NoBaris = glView.NoBaris,
                            Keterangan = glView.Keterangan,
                            NoRek1 = glView.NoRek1,
                            CetakDetil = glView.CetakDetil,
                            CetakGaris1 = glView.CetakGaris1,
                            CetakGaris2 = glView.CetakGaris2,
                            CetakBln1 = glView.CetakBln1,
                            CetakBln2 = glView.CetakBln2,
                            Spasi = glView.Spasi,
                            CetakTebal = glView.CetakTebal,
                            CetakHide = glView.CetakHide,
                            CetakNegatif = glView.CetakNegatif,
                            RumusBaris = glView.RumusBaris
                        });
                }
                //   TransFinancial.AddRange(listPrint);

                foreach (var item in listPrint)
                {
                    if (!string.IsNullOrEmpty(item.NoRek1))
                    {
                        item.JumRekap1 = RugiLabaTransBulan1(item.NoRek1, company, Bulan, Tahun);

                    }
                    if (item.CetakDetil)
                    {
                        if (!string.IsNullOrEmpty(item.NoRek1))
                        {
                            string[] selectedList = item.NoRek1.Split("-").ToArray();

                            int counting = selectedList.Count();
                            int i = 0;
                            foreach (var detail in selectedList)
                            {
                                i++;
                                if (i == counting)
                                {
                                    TransFinancial.Add(new FcPrintGLView()
                                    {
                                        NoBaris = item.NoBaris,

                                        Keterangan = GetNameAccount(detail),

                                        CetakGaris1 = item.CetakGaris1,
                                        CetakGaris2 = item.CetakGaris2,
                                        CetakHide = item.CetakHide,
                                        CetakNegatif = item.CetakNegatif,
                                        Spasi = 5,

                                        JumTran1 = RugiLabaDetailTransBulan1(detail, company, Bulan, Tahun)


                                    });
                                }
                                else
                                {
                                    TransFinancial.Add(new FcPrintGLView()
                                    {
                                        NoBaris = item.NoBaris,
                                        Keterangan = GetNameAccount(detail),

                                        CetakHide = item.CetakHide,
                                        CetakNegatif = item.CetakNegatif,
                                        Spasi = 5,

                                        JumTran1 = RugiLabaDetailTransBulan1(detail, company, Bulan, Tahun)


                                    });
                                }


                            }

                        }

                    }
                }
                foreach (var item in TransFinancial)
                {
                    if (!string.IsNullOrEmpty(item.RumusBaris))
                    {
                        string[] selectedList = item.RumusBaris.Split("+").ToArray();

                        foreach (var detail in selectedList)
                        {
                            var firstList = listPrint.Where(x => x.NoBaris == detail).FirstOrDefault();
                            item.JumRekap1 += (firstList.JumRekap1 + firstList.JumTran1);
                        }

                        listPrint.Find(x => x.NoBaris == item.NoBaris).JumRekap1 = item.JumRekap1;
                    }

                    if (!string.IsNullOrEmpty(item.NoRek1) && item.CetakDetil == false)
                    {
                        item.JumTran1 = RugiLabaTransBulan1(item.NoRek1, company, Bulan, Tahun);
                    }
                }

                TransFinancial = TransFinancial.OrderBy(x => x.NoBaris).ToList();


                return TransFinancial;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private decimal RugiLabaTransBulan1(string rekening, string company, int Bulan, int Tahun)
        {
            // string[] selectedList = new string[] { };
            var retainEarning = _contextFC.FcAccounts.Where(x => x.GlTipe == 3).FirstOrDefault();

            decimal mTotal1 = 0;
            string[] selectedList = rekening.Split("-").ToArray();
            var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && selectedList.Contains(x.GlAcct)).ToList();
            List<FcAccount> listRE = new List<FcAccount>();

            foreach (var ceking in selectedList)
            {
                if (ceking == retainEarning.GlAcct)
                {
                    listRE = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlTipe == 2).ToList();

                }
            }

            //       mTotal1 += listGL.Sum(x => x.GlSldAwal);


            switch (Bulan - 1)
            {
                case 0:
                    mTotal1 += listGL.Sum(x => x.GlFisc1);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc1);

                    break;
                case 1:
                    mTotal1 += listGL.Sum(x => x.GlFisc2);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc2);

                    break;
                case 2:
                    mTotal1 += listGL.Sum(x => x.GlFisc3);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc3);

                    break;
                case 3:
                    mTotal1 += listGL.Sum(x => x.GlFisc4);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc4);

                    break;
                case 4:
                    mTotal1 += listGL.Sum(x => x.GlFisc5);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc5);

                    break;
                case 5:
                    mTotal1 += listGL.Sum(x => x.GlFisc6);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc6);

                    break;
                case 6:
                    mTotal1 += listGL.Sum(x => x.GlFisc7);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc7);

                    break;
                case 7:
                    mTotal1 += listGL.Sum(x => x.GlFisc8);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc8);

                    break;
                case 8:
                    mTotal1 += listGL.Sum(x => x.GlFisc9);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc9);

                    break;
                case 9:
                    mTotal1 += listGL.Sum(x => x.GlFisc10);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc10);

                    break;
                case 10:
                    mTotal1 += listGL.Sum(x => x.GlFisc11);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc11);

                    break;
                case 11:
                    mTotal1 += listGL.Sum(x => x.GlFisc12);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc12);

                    break;
            }


            // glAccounts.Contains(rekening).
            return mTotal1;
        }
        private decimal RugiLabaDetailTransBulan1(string rekening, string company, int Bulan, int Tahun)
        {
            // string[] selectedList = new string[] { };
            var retainEarning = _contextFC.FcAccounts.Where(x => x.GlTipe == 3 && x.FcComKode == company && x.FcTahun == Tahun && x.GlAcct == rekening).FirstOrDefault();
            List<FcAccount> listRE = new List<FcAccount>();

            decimal mTotal1 = 0;

            if (retainEarning != null)
            {
                listRE = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlTipe == 2).ToList();
            }

            var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlAcct == rekening).ToList();

            //     mTotal1 += listGL.Sum(x => x.GlSldAwal);


            switch (Bulan - 1)
            {
                case 0:
                    mTotal1 += listGL.Sum(x => x.GlFisc1);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc1);
                    break;
                case 1:
                    mTotal1 += listGL.Sum(x => x.GlFisc2);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc2);
                    break;
                case 2:
                    mTotal1 += listGL.Sum(x => x.GlFisc3);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc3);
                    break;
                case 3:
                    mTotal1 += listGL.Sum(x => x.GlFisc4);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc4);
                    break;
                case 4:
                    mTotal1 += listGL.Sum(x => x.GlFisc5);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc5);
                    break;
                case 5:
                    mTotal1 += listGL.Sum(x => x.GlFisc6);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc6);
                    break;
                case 6:
                    mTotal1 += listGL.Sum(x => x.GlFisc7);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc7);
                    break;
                case 7:
                    mTotal1 += listGL.Sum(x => x.GlFisc8);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc8);
                    break;
                case 8:
                    mTotal1 += listGL.Sum(x => x.GlFisc9);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc9);
                    break;
                case 9:
                    mTotal1 += listGL.Sum(x => x.GlFisc10);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc10);
                    break;
                case 10:
                    mTotal1 += listGL.Sum(x => x.GlFisc11);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc11);
                    break;
                case 11:
                    mTotal1 += listGL.Sum(x => x.GlFisc12);
                    if (listRE != null)
                        mTotal1 += listRE.Sum(x => x.GlFisc12);
                    break;
            }


            // glAccounts.Contains(rekening).
            return mTotal1;
        }
        #endregion

        #region bukubesar
        public List<FcLedgerView> printBukuBesar(string kodeCetak, string company, int BulanAwal, int Bulan, int Tahun)
        {
            try
            {
                //var listPrint = _contextFC.FcPrintGls.Where(x => x.KodeCetak == kodeCetak).ToList();
                var listAkun = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun && x.GlAcct == kodeCetak).FirstOrDefault();

                List<FcLedgerView> TransFinancial = new List<FcLedgerView>();

                decimal mTotal1 = 0;
                decimal mAkhir = 0;
                decimal mDebet = 0;
                decimal mKredit = 0;

                if (listAkun != null)
                {



                    mTotal1 = listAkun.GlSldAwal;
                    // saldoAwal
                    for (int i = 0; i < BulanAwal - 1; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                mTotal1 += listAkun.GlFisc1;

                                break;
                            case 1:
                                mTotal1 += listAkun.GlFisc2;

                                break;
                            case 2:
                                mTotal1 += listAkun.GlFisc3;

                                break;
                            case 3:
                                mTotal1 += listAkun.GlFisc4;

                                break;
                            case 4:
                                mTotal1 += listAkun.GlFisc5;

                                break;
                            case 5:
                                mTotal1 += listAkun.GlFisc6;

                                break;
                            case 6:
                                mTotal1 += listAkun.GlFisc7;

                                break;
                            case 7:
                                mTotal1 += listAkun.GlFisc8;

                                break;
                            case 8:
                                mTotal1 += listAkun.GlFisc9;

                                break;
                            case 9:
                                mTotal1 += listAkun.GlFisc10;

                                break;
                            case 10:
                                mTotal1 += listAkun.GlFisc11;

                                break;
                            case 11:
                                mTotal1 += listAkun.GlFisc12;

                                break;
                        }
                    };
                    // end of SaldoAwal


                    mAkhir = mTotal1;


                    var TransHeader = _contextFC.FcTransHs.Where(x => x.Tanggal.Month >= BulanAwal && x.Tanggal.Month <= Bulan && x.Tanggal.Year == Tahun && x.FcComKode == company).ToList().OrderBy(x => x.Tanggal);
                    var TransDetail = _contextFC.FcTransDs.Where(x => x.GlAcct == kodeCetak).ToList();

                    var listLedger = (from header in TransHeader
                                      join detail in TransDetail on header.FcTransHId equals detail.FcTransHId
                                      select new FcLedgerView
                                      {
                                          DocNo = header.DocNo,
                                          Tanggal = header.Tanggal,
                                          GlAcct = detail.GlAcct,
                                          Keterangan = detail.Keterangan,
                                          Debit = detail.Debet,
                                          Credit = detail.Kredit,
                                          Saldo = detail.Jumlah

                                      });

                    mAkhir += listLedger.Sum(x => x.Saldo);

                    mDebet = listLedger.Sum(x => x.Debit);
                    mKredit = listLedger.Sum(x => x.Credit);



                    TransFinancial.Add(
                        new FcLedgerView()
                        {
                            DocNo = "",
                            GlAcct = kodeCetak,
                            Keterangan = "SALDO AWAL",
                            Balance = mTotal1
                        });

                    TransFinancial.AddRange(listLedger);

                    TransFinancial.Add(
                        new FcLedgerView()
                        {
                            DocNo = "",
                            GlAcct = kodeCetak,
                            Keterangan = "SALDO AKHIR",
                            Debit = mDebet,
                            Credit = mKredit,
                            Balance = mAkhir
                        });

                    // TransFinancial = TransFinancial.OrderBy(x => x.).ToList();



                }
                return TransFinancial;
            }
            catch (Exception)
            {

                throw;
            }


        }
        #endregion

        #region trial balancce

        public List<FcAccount> printTrialBalance(string company, int Bulan, int Tahun)
        {
            try
            {
                var listGL = _contextFC.FcAccounts.Where(x => x.FcComKode == company && x.FcTahun == Tahun).OrderBy(x => x.GlAcct).ToList();



                List<FcAccount> listPrint = new();



                List<FcPrintGLView> TransFinancial = new();

                foreach (var glView in listGL)
                {
                    decimal fiscal = 0;
                    for (int nNo = 1; nNo <= Bulan - 1; nNo++)
                    {
                        switch (nNo)
                        {
                            case 1:
                                fiscal += glView.GlFisc1;
                                break;
                            case 2:
                                fiscal += glView.GlFisc2;
                                break;
                            case 3:
                                fiscal += glView.GlFisc3;
                                break;
                            case 4:
                                fiscal += glView.GlFisc4;
                                break;
                            case 5:
                                fiscal += glView.GlFisc5;
                                break;
                            case 6:
                                fiscal += glView.GlFisc6;
                                break;
                            case 7:
                                fiscal += glView.GlFisc7;
                                break;
                            case 8:
                                fiscal += glView.GlFisc8;
                                break;
                            case 9:
                                fiscal += glView.GlFisc9;
                                break;
                            case 10:
                                fiscal += glView.GlFisc10;
                                break;
                            case 11:
                                fiscal += glView.GlFisc11;
                                break;
                            case 12:
                                fiscal += glView.GlFisc12;
                                break;
                        }
                    }

                    listPrint.Add(
                        new FcAccount()
                        {
                            FcComKode = glView.FcComKode,
                            FcTahun = glView.FcTahun,
                            GlAcct = glView.GlAcct,
                            GlNama = glView.GlNama,
                            GlTipe = glView.GlTipe,
                            GlSldAwal = glView.GlSldAwal + fiscal,

                        });
                }
                //   TransFinancial.AddRange(listPrint);
                var TransHeader = _contextFC.FcTransHs.Where(x => x.Tanggal.Month == Bulan && x.Tanggal.Year == Tahun && x.FcComKode == company).ToList().OrderBy(x => x.Tanggal);
                var TransDetail = _contextFC.FcTransDs.Where(x => x.FcComKode == company).ToList();

                var listLedger = (from header in TransHeader
                                  join detail in TransDetail on header.FcTransHId equals detail.FcTransHId
                                  select new FcLedgerView
                                  {
                                      DocNo = header.DocNo,
                                      Tanggal = header.Tanggal,
                                      GlAcct = detail.GlAcct,
                                      Keterangan = detail.Keterangan,
                                      Debit = detail.Debet,
                                      Credit = detail.Kredit,
                                      Saldo = detail.Jumlah

                                  });

                foreach (var detail in listLedger)
                {
                    listPrint.Find(x => x.GlAcct == detail.GlAcct).GlPreFisc1 += detail.Debit;
                    listPrint.Find(x => x.GlAcct == detail.GlAcct).GlPreFisc2 += detail.Credit;
                }


                return listPrint;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region printJurnal

        public IEnumerable<FcTransH> printJurnal(int tahun, string kodeCompany, DateTime TglAwal, DateTime TglAkhir)
        {
            try
            {
                //  var TransHeader = _contextFC.FcTransHs.Where(x => x.FcComKode == kodeCompany && x.Tanggal >= TglAwal && x.Tanggal <= TglAkhir).OrderBy(x => x.Tanggal).ThenBy(x =>x.DocNo).ToList();


                var TransHFcGL = _contextFC.FcTransHs.Where(x => x.FcComKode == kodeCompany && x.Tanggal >= TglAwal && x.Tanggal <= TglAkhir).OrderBy(x => x.Tanggal).ThenBy(x => x.DocNo);

                return TransHFcGL;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public IEnumerable<FcTransD> JurnalDetail(int headerID)
        {
            try
            {
                var TransDFcGL = _contextFC.FcTransDs.Where(x => x.FcTransHId == headerID);

                return TransDFcGL;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}

