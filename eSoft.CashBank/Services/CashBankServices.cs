using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.Services
{
    public class CashBankServices : ICashBankServices
    {
        private readonly DbContextBank _context;
        private readonly IServiceProvider _serviceProvider;

        public CashBankServices(DbContextBank context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        #region Bank Class

        public List<CbBank> GetBank()
        {
          
           return _context.CbBanks.OrderBy(x =>x.KodeBank).ToList();
        }

        // Reflection helper removed from here and declared at namespace level below.

        // Check duplicates: return list of booleans corresponding to samples order.
        public async Task<List<bool>> CheckDuplicatesAsync(List<BankTransactionView> samples, string kodeBank)
        {
            var result = new List<bool>();
            if (samples == null || !samples.Any()) return result;

            foreach (var s in samples)
            {
                var date = s.Tanggal.Date;
                var amt = Math.Abs(s.Amount);
                var desc = (s.Description ?? string.Empty).Trim();

                var exists = await _context.CbTransHs
                    .Include(h => h.CbTransDs)
                    .Where(h => h.KodeBank == kodeBank && h.Tanggal.Date == date)
                    .SelectMany(h => h.CbTransDs)
                    .AnyAsync(d => Math.Abs(d.Jumlah) == amt || (d.Keterangan != null && d.Keterangan.Contains(desc)));

                result.Add(exists);
            }

            return result;
        }

        public CbBank GetBankId(int id)
        {
            return _context.CbBanks.Where(x => x.CbBankId == id).FirstOrDefault();
        }
        public CbBank GetBankKd(string id)
        {
            return _context.CbBanks.Where(x => x.KodeBank == id).FirstOrDefault();
        }

        public bool CekKdBank(string kodeBank)
        {
            string test = kodeBank.ToUpper();
            var cekFirst = _context.CbBanks.Where(x => x.KodeBank == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddBank(BankView banks)
        {
            string test = banks.Kdbank.ToUpper();
            var cekFirst = _context.CbBanks.Where(x => x.KodeBank == test).ToList();
            if (cekFirst.Count == 0)
            {
                CbBank Bank = new()
                {
                    KodeBank = banks.Kdbank.ToUpper(),
                    NmBank = banks.Namabank,
                    Kurs = banks.Kurs,
                    Acctset = banks.Acctset,
                    ClrDate = banks.ClrDate,
                    SldAwal = banks.SldAwal,
                    KSldAwal = banks.KSldAwal,
                    Saldo = banks.SldAwal,
                    KSaldo = banks.KSldAwal,
                    Status = banks.Status,
                    Pajak = banks.Pajak

                };
                _context.CbBanks.Add(Bank);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditBank(BankView banks)
        {
            try
            {
                var ExistingBank = _context.CbBanks.Where(x => x.CbBankId == banks.BankId).FirstOrDefault();
                if (ExistingBank != null)
                {
                    ExistingBank.NmBank = banks.Namabank;
                    ExistingBank.Kurs = banks.Kurs;
                    ExistingBank.Acctset = banks.Acctset;
                    ExistingBank.ClrDate = banks.ClrDate;

                    ExistingBank.Saldo -= ExistingBank.SldAwal;
                    ExistingBank.KSaldo -= ExistingBank.KSldAwal;

                    ExistingBank.SldAwal = banks.SldAwal;
                    ExistingBank.KSldAwal = banks.KSldAwal;
                    ExistingBank.Saldo += banks.SldAwal;
                    ExistingBank.KSaldo += banks.KSldAwal;
                    ExistingBank.Pajak = banks.Pajak;
                    ExistingBank.Kurs = banks.Kurs;

                    _context.CbBanks.Update(ExistingBank);
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

        public async Task<bool> DelBank(int banks)
        {
            try
            {
                var ExistingBank = _context.CbBanks.Single(item => item.CbBankId == banks);
                //  var ExistingBank = _context.Banks.Where(x => x.CbBankId == banks).FirstOrDefault();
                if (ExistingBank != null && ExistingBank.Saldo == 0)
                {
                    _context.CbBanks.Remove(ExistingBank);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }



        }
        #endregion Bank Class

        #region SrcGrp 
        public List<CbGrp> GetSrcGroup()
        {
            return _context.CbGrps.ToList();
        }

        public bool CekSrcGroup(string kodeBank)
        {
            string test = kodeBank.ToUpper();
            var cekFirst = _context.CbGrps.Where(x => x.Grp == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public CbGrp GetSrcGroupId(int id)
        {
            return _context.CbGrps.Where(x => x.CbGrpId == id).FirstOrDefault();
        }

        public CbGrp GetSrcGroupKd(string id)
        {
            return _context.CbGrps.Where(x => x.Grp == id).FirstOrDefault();
        }
        public bool AddSrcGroup(SrcGroupView codeview)
        {
            string test = codeview.Grp.ToUpper();
            var cekFirst = _context.CbGrps.Where(x => x.Grp == test).ToList();
            if (cekFirst.Count == 0)
            {
                CbGrp BankCode = new()
                {
                    Grp = codeview.Grp.ToUpper(),
                    NamaGrp = codeview.NamaGrp
                   

                };
                _context.CbGrps.Add(BankCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditSrcGroup(SrcGroupView codeview)
        {
            try
            {
                var ExistingSrcCode = _context.CbGrps.Where(x => x.CbGrpId == codeview.CbGrpId).FirstOrDefault();
                if (ExistingSrcCode != null)
                {
                    ExistingSrcCode.NamaGrp = codeview.NamaGrp;
                   

                    _context.CbGrps.Update(ExistingSrcCode);
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

        public async Task<bool> DelSrcGroup(int codeview)
        {
            try
            {
                var ExistingSrcCode = _context.CbGrps.Where(x => x.CbGrpId == codeview).FirstOrDefault();
                if (ExistingSrcCode != null)
                {
                    _context.CbGrps.Remove(ExistingSrcCode);
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
        #endregion

        #region SrcCode Class

        public List<CbSrcCode> GetSrcCode()
        {
            return _context.CbSrcCodes.ToList();
        }

        public bool CekSrcCode(string kodeBank)
        {
            string test = kodeBank.ToUpper();
            var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public CbSrcCode GetSrcCodeId(int id)
        {
            return _context.CbSrcCodes.Where(x => x.CbSrcCodeId == id).FirstOrDefault();
        }

        public CbSrcCode GetSrcCodeKd(string id)
        {
            return _context.CbSrcCodes.Where(x => x.SrcCode == id).FirstOrDefault();
        }

        public bool AddSrcCode(SrcCodeView codeview)
        {
            string test = codeview.SrcCode.ToUpper();
            var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                CbSrcCode BankCode = new()
                {
                    SrcCode = codeview.SrcCode.ToUpper(),
                    NamaSrc = codeview.NamaSrc,
                    GlAcct = codeview.GlAcct

                };
                _context.CbSrcCodes.Add(BankCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditSrcCode(SrcCodeView codeview)
        {
            try
            {
                var ExistingSrcCode = _context.CbSrcCodes.Where(x => x.CbSrcCodeId == codeview.SrcCodeId).FirstOrDefault();
                if (ExistingSrcCode != null)
                {
                    ExistingSrcCode.NamaSrc = codeview.NamaSrc;
                    ExistingSrcCode.GlAcct = codeview.GlAcct;

                    _context.CbSrcCodes.Update(ExistingSrcCode);
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

        public async Task<bool> DelSrcCode(int codeview)
        {
            try
            {
                var ExistingSrcCode = _context.CbSrcCodes.Where(x => x.CbSrcCodeId == codeview).FirstOrDefault();
                if (ExistingSrcCode != null)
                {
                    _context.CbSrcCodes.Remove(ExistingSrcCode);
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
        #endregion srcode Class

        #region Transfer Antar Bank
        public CbTransfer GetTransferDoc(string docno)
        {
            return _context.CbTransfers.Where(x => x.DocNo == docno).FirstOrDefault();
        }

        public CbTransfer GetTransferId(int id)
        {
            return _context.CbTransfers.Where(x => x.CbTransferId == id).FirstOrDefault();
        }

        public List<CbTransfer> GetTransfer()
        {

            return _context.CbTransfers.OrderByDescending(x => x.Tanggal).ToList();

        }


        public CbTransfer AddTransfer(TransferView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            CbTransfer transfer = new()
            {
                DocNo = GetNumberTrf("TRF"),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kurs = trans.Kurs,
                Kurs2 = trans.Kurs2,
                KValue = trans.KValue,
                Saldo = trans.Saldo,
                KSaldo = trans.KSaldo,
                KodeBank1 = trans.KodeBank1.ToUpper(),
                KodeBank2 = trans.KodeBank2.ToUpper()
            };

            _context.CbTransfers.Add(transfer);

            CbTransH transH = new()
            {
                DocNo = GetNumberTr2('T' + trans.KodeBank1.ToUpper().Trim()),
                KodeBank = trans.KodeBank1.ToUpper(),
                Refno = transfer.DocNo,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kurs = trans.Kurs,
                Saldo = -1 * trans.Saldo,
                KSaldo = -1 * trans.KSaldo,
                CbTransDs = new List<CbTransD>()
            };

            transH.CbTransDs.Add(new CbTransD()
            {
                SrcCode = "CB",
                Keterangan = trans.Keterangan,
                Terima = 0,
                Bayar = trans.Saldo,
                KTerima = 0,
                KBayar = trans.KSaldo,
                KValue = trans.KValue,
                Jumlah = -1 * trans.Saldo,
                KJumlah = -1 * trans.KSaldo,
                Kurs = trans.Kurs
            });

            var bank = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank1 select e).FirstOrDefault();
            bank.Saldo -= trans.Saldo;
            bank.KSaldo -= trans.KSaldo;
            _context.CbBanks.Update(bank);
            _context.CbTransHs.Add(transH);
            _context.SaveChanges();

            /* ke bank */
            CbTransH transHd = new()
            {
                DocNo = GetNumberTr2('T' + trans.KodeBank2.ToUpper().Trim()),
                KodeBank = trans.KodeBank2.ToUpper(),
                Refno = transfer.DocNo,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kurs = trans.Kurs2,
                Saldo = trans.Saldo,
                KSaldo = trans.KSaldo,
                CbTransDs = new List<CbTransD>()
            };

            transHd.CbTransDs.Add(new CbTransD()
            {
                SrcCode = "CB",
                Keterangan = trans.Keterangan,
                Terima = trans.Saldo,
                Bayar = 0,
                KTerima = trans.KSaldo,
                KBayar = 0,
                KValue = trans.KValue,
                Jumlah = trans.Saldo,
                KJumlah = trans.KSaldo,
                Kurs = trans.Kurs2
            });

            var bankd = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank2 select e).FirstOrDefault();
            bankd.Saldo += trans.Saldo;
            bankd.KSaldo += trans.KSaldo;
            _context.CbBanks.Update(bankd);
            _context.CbTransHs.Add(transHd);

            _context.SaveChanges();

            var TempTrans = GetTransferDoc(transfer.DocNo);

            return TempTrans;
            // return true;


        }

        public CbTransfer EditTransfer(TransferView trans)
        {


            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            var ExistingTrans = _context.CbTransfers.Where(x => x.CbTransferId == trans.CbTransferId).FirstOrDefault();
            if (ExistingTrans != null)
            {
                _context.CbTransfers.Remove(ExistingTrans);

                var listTrans1 = _context.CbTransHs.Where(x => x.Refno == ExistingTrans.DocNo && x.KodeBank == ExistingTrans.KodeBank1).FirstOrDefault();
                if (listTrans1 != null)
                {
                    _context.CbTransHs.Remove(listTrans1);
                    var bank1 = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank1 select e).FirstOrDefault();
                    bank1.Saldo += ExistingTrans.Saldo;
                    bank1.KSaldo += ExistingTrans.KSaldo;
                    _context.CbBanks.Update(bank1);
                    _context.SaveChanges();
                }

                var listTrans2 = _context.CbTransHs.Where(x => x.Refno == ExistingTrans.DocNo && x.KodeBank == ExistingTrans.KodeBank2).FirstOrDefault();
                if (listTrans2 != null)
                {
                    _context.CbTransHs.Remove(listTrans2);
                    var bank2 = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank2 select e).FirstOrDefault();
                    bank2.Saldo -= ExistingTrans.Saldo;
                    bank2.KSaldo -= ExistingTrans.KSaldo;
                    _context.CbBanks.Update(bank2);
                    _context.SaveChanges();
                }
                //    _context.SaveChanges();

                CbTransfer transfer = new()
                {
                    DocNo = ExistingTrans.DocNo,
                    Tanggal = trans.Tanggal,
                    Keterangan = trans.Keterangan,
                    Kurs = trans.Kurs,
                    Kurs2 = trans.Kurs2,
                    KValue = trans.KValue,
                    Saldo = trans.Saldo,
                    KSaldo = trans.KSaldo,
                    KodeBank1 = trans.KodeBank1.ToUpper(),
                    KodeBank2 = trans.KodeBank2.ToUpper()
                };

                _context.CbTransfers.Add(transfer);

                CbTransH transH = new()
                {
                    DocNo = GetNumberTr2('T' + trans.KodeBank1.ToUpper().Trim()),
                    KodeBank = trans.KodeBank1.ToUpper(),
                    Tanggal = trans.Tanggal,
                    Refno = transfer.DocNo,
                    Keterangan = trans.Keterangan,
                    Kurs = trans.Kurs,
                    Saldo = -1 * trans.Saldo,
                    KSaldo = -1 * trans.KSaldo,
                    CbTransDs = new List<CbTransD>()
                };

                transH.CbTransDs.Add(new CbTransD()
                {
                    SrcCode = "CB",
                    Keterangan = trans.Keterangan,
                    Terima = 0,
                    Bayar = trans.Saldo,
                    KTerima = 0,
                    KBayar = trans.KSaldo,
                    KValue = trans.KValue,
                    Jumlah = -1 * trans.Saldo,
                    KJumlah = -1 * trans.KSaldo,
                    Kurs = trans.Kurs
                });

                var bank = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank1 select e).FirstOrDefault();
                bank.Saldo -= trans.Saldo;
                bank.KSaldo -= trans.KSaldo;
                _context.CbBanks.Update(bank);
                _context.CbTransHs.Add(transH);
                _context.SaveChanges();

                /* ke bank */
                CbTransH transHd = new()
                {
                    DocNo = GetNumberTr2('T' + trans.KodeBank2.ToUpper().Trim()),
                    KodeBank = trans.KodeBank2.ToUpper(),
                    Refno = transfer.DocNo,
                    Tanggal = trans.Tanggal,
                    Keterangan = trans.Keterangan,
                    Kurs = trans.Kurs2,
                    Saldo = trans.Saldo,
                    KSaldo = trans.KSaldo,
                    CbTransDs = new List<CbTransD>()
                };

                transHd.CbTransDs.Add(new CbTransD()
                {
                    SrcCode = "CB",
                    Keterangan = trans.Keterangan,
                    Terima = trans.Saldo,
                    Bayar = 0,
                    KTerima = trans.KSaldo,
                    KBayar = 0,
                    KValue = trans.KValue,
                    Jumlah = trans.Saldo,
                    KJumlah = trans.KSaldo,
                    Kurs = trans.Kurs2
                });

                var bankd = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank2 select e).FirstOrDefault();
                bankd.Saldo += trans.Saldo;
                bankd.KSaldo += trans.KSaldo;
                _context.CbBanks.Update(bankd);
                _context.CbTransHs.Add(transHd);

                _context.SaveChanges();

                var TempTrans = GetTransferDoc(transfer.DocNo);

                return TempTrans;


            }
            else
            {
                return ExistingTrans;
            }


            // return true;


        }

        public async Task<bool> DelTransfer(int id)
        {
            try
            {

                var ExistingTrans = _context.CbTransfers.Where(x => x.CbTransferId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    _context.CbTransfers.Remove(ExistingTrans);

                    var listTrans1 = _context.CbTransHs.Where(x => x.Refno == ExistingTrans.DocNo && x.KodeBank == ExistingTrans.KodeBank1).FirstOrDefault();
                    if (listTrans1 != null)
                    {
                        _context.CbTransHs.Remove(listTrans1);
                        var bank1 = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank1 select e).FirstOrDefault();
                        bank1.Saldo += ExistingTrans.Saldo;
                        bank1.KSaldo += ExistingTrans.KSaldo;
                        _context.CbBanks.Update(bank1);
                        _context.SaveChanges();
                    }

                    var listTrans2 = _context.CbTransHs.Where(x => x.Refno == ExistingTrans.DocNo && x.KodeBank == ExistingTrans.KodeBank2).FirstOrDefault();
                    if (listTrans2 != null)
                    {
                        _context.CbTransHs.Remove(listTrans2);
                        var bank2 = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank2 select e).FirstOrDefault();
                        bank2.Saldo -= ExistingTrans.Saldo;
                        bank2.KSaldo -= ExistingTrans.KSaldo;
                        _context.CbBanks.Update(bank2);
                        _context.SaveChanges();
                    }

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
        #endregion Transfer Antar Bank

        #region Transaksi Bank Class
        public CbTransH GetTransDoc(string docno)
        {
            return _context.CbTransHs.Include(p => p.CbTransDs).Where(x => x.DocNo == docno).FirstOrDefault();
        }
        public CbTransH GetTrans(int id)
        {
            return _context.CbTransHs.Include(p => p.CbTransDs).Where(x => x.CbTransHId == id).FirstOrDefault();
        }

        public List<CbTransH> GetTransH()
        {
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            var test = (from e in _context.CbTransHs orderby e.Tanggal.Date descending select e).ToList();

            return test;

            //   return _context.CbTransHs.OrderByDescending(x => x.Tanggal).ToList();

        }

        public List<SearchTransHView> GetTransHSearch()
        {
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            var test = (from e in _context.CbTransHs
                        orderby e.Tanggal.Date
                        select new SearchTransHView
                        {
                            CbTransHId = e.CbTransHId,
                            DocNo = e.DocNo,
                            Tanggal = e.Tanggal,
                            Keterangan = e.Keterangan,
                            Kurs = e.Kurs,
                            KodeBank = e.KodeBank
                        }).ToList();

            return test;

            //   return _context.CbTransHs.OrderByDescending(x => x.Tanggal).ToList();

        }

        public List<CbTransH> Get3TransH(DateTime tgl1, DateTime tgl2)
        {
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return _context.CbTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3)).ToList();

            //  List<CbTransH> arTrans = new List<CbTransH>();

            return _context.CbTransHs.OrderByDescending(x => x.Tanggal).Where(x => (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();

        }

        public List<CbTransD> GetTransD()
        {
            return _context.CbTransDs.ToList();
        }

        public List<CbTransD> GetTransDdetail(int Id)
        {
            return _context.CbTransDs.Where(x => x.CbTransHId == Id).ToList();
        }

        public CbTransH AddTransH(TransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            CbTransH transH = new()
            {
                DocNo = GetNumber(trans.KodeDoc.ToUpper()),
                KodeBank = trans.KodeBank.ToUpper(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kurs = trans.Kurs,
                Saldo = trans.Saldo,
                KSaldo = trans.KSaldo,
                CbTransDs = new List<CbTransD>()
            };
            foreach (var item in trans.TransDs)
            {
                transH.CbTransDs.Add(new CbTransD()
                {
                    SrcCode = item.SrcCode,
                    Keterangan = item.Keterangan,
                    Terima = item.Terima,
                    Bayar = item.Bayar,
                    KTerima = item.KTerima,
                    KBayar = item.KBayar,
                    KValue = item.KValue,
                    Jumlah = item.Jumlah,
                    KJumlah = item.KJumlah,
                    Kurs = item.Kurs
                });
            }
            var bank = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank select e).FirstOrDefault();
            bank.Saldo += trans.Saldo;
            bank.KSaldo += trans.KSaldo;
            _context.CbBanks.Update(bank);
            _context.CbTransHs.Add(transH);
            _context.SaveChanges();

            var TempTrans = GetTransDoc(transH.DocNo);

            return TempTrans;
            // return true;


        }

        public CbTransH EditTransH(TransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();



            try
            {
                var ExistingTrans = _context.CbTransHs.Where(x => x.CbTransHId == trans.CbTransHId).FirstOrDefault();
                if (ExistingTrans != null)
                {

                    _context.CbTransHs.Remove(ExistingTrans);

                    var Oldbank = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank select e).FirstOrDefault();
                    Oldbank.Saldo -= ExistingTrans.Saldo;
                    Oldbank.KSaldo -= ExistingTrans.KSaldo;
                    _context.CbBanks.Update(Oldbank);
                    _context.SaveChanges();


                    /* update */

                    CbTransH transH = new()
                    {
                        //  transH.DocNo = ExistingTrans.DocNo;
                        DocNo = ExistingTrans.DocNo,
                        Refno = ExistingTrans.Refno,
                        KodeBank = trans.KodeBank.ToUpper(),
                        Tanggal = trans.Tanggal,
                        Keterangan = trans.Keterangan,
                        Kurs = trans.Kurs,
                        Saldo = trans.Saldo,
                        KSaldo = trans.KSaldo,
                        CbTransDs = new List<CbTransD>()
                    };
                    foreach (var item in trans.TransDs)
                    {
                        transH.CbTransDs.Add(new CbTransD()
                        {
                            SrcCode = item.SrcCode,
                            Keterangan = item.Keterangan,
                            Terima = item.Terima,
                            Bayar = item.Bayar,
                            KTerima = item.KTerima,
                            KBayar = item.KBayar,
                            KValue = item.KValue,
                            Jumlah = item.Jumlah,
                            KJumlah = item.KJumlah,
                            Kurs = item.Kurs
                        });
                    }
                    var Newbank = (from e in _context.CbBanks where e.KodeBank == trans.KodeBank select e).FirstOrDefault();

                    Newbank.Saldo += trans.Saldo;
                    Newbank.KSaldo += trans.KSaldo;

                    _context.CbBanks.Update(Newbank);
                    _context.CbTransHs.Add(transH);
                    _context.SaveChanges();

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
               // Console.WriteLine("caught exception" + e.Message);
            }




        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _context.CbTransHs.Include(x =>x.CbTransDs).Where(x => x.CbTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                    var bank = (from e in _context.CbBanks where e.KodeBank == ExistingTrans.KodeBank select e).FirstOrDefault();
                    bank.Saldo -= ExistingTrans.Saldo;
                    bank.KSaldo -= ExistingTrans.KSaldo;
                    _context.CbBanks.Update(bank);
                    _context.CbTransHs.Remove(ExistingTrans);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw new Exception("Terjadi kesalahan saat Menghapus Transaksi");
            }

            return false;

        }

        #endregion Transaksi Bank Class

        public string GetNumber(string kodeno)
        {
            string kodeurut = kodeno + "-";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.CbTransHs.Where(x => x.DocNo.Substring(0, 10).Equals(xbukti)).ToList();
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

        public string GetNumberTrf(string kodeno)
        {
            string kodeurut = kodeno + "-";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.CbTransfers.Where(x => x.DocNo.Substring(0, 10).Equals(xbukti)).ToList();
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

        public string GetNumberTr2(string kodeno)
        {
            string kodeurut = kodeno + "-";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.CbTransHs.Where(x => x.DocNo.Substring(0, 10).Equals(xbukti)).ToList();
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

        #region cetak

        public List<RekeningView> CetakMutasi(DateTime Tanggal1, DateTime Tanggal2, string kodeBank)
        {

            List<RekeningView> Transaksi = new List<RekeningView>();
            //     TransHView Transh = new TransHView() { TransDs = new List<TransDView>() };

            var bankawal = _context.CbBanks
                .Where(x => x.KodeBank == kodeBank).FirstOrDefault();

            var TransAwal = _context.CbTransHs
               .Where(x => x.KodeBank == kodeBank && (x.Tanggal.Date > bankawal.ClrDate.Date && x.Tanggal.Date < Tanggal1.Date))
              .Select(x => new RekeningView
              {
                  KodeBank = x.KodeBank,
                  DocNo = x.DocNo,
                  Tanggal = x.Tanggal,
                  Keterangan = x.Keterangan,
                  Kurs = x.Kurs,
                  Saldo = (string.IsNullOrEmpty(bankawal.Kurs) ? x.Saldo : x.KSaldo)

              })
               .ToList();

            var SaldoAwal = TransAwal.Sum(x => x.Saldo) + (string.IsNullOrEmpty(bankawal.Kurs) ? bankawal.SldAwal : bankawal.KSldAwal);

            Transaksi.Add(new RekeningView
            {
                KodeBank = kodeBank,
                Tanggal = Tanggal1,
                DocNo = "Saldo Awal",
                Saldo = SaldoAwal

            }
                );


            var Rincian = _context.CbTransHs
                .Where(x => x.KodeBank == kodeBank && (Tanggal1.Date <= x.Tanggal.Date && x.Tanggal.Date <= Tanggal2.Date))
                .OrderBy(x => x.Tanggal)
               .Select(x => new RekeningView
               {
                   KodeBank = x.KodeBank,
                   CbTransHId = x.CbTransHId,
                   DocNo = x.DocNo,
                   Tanggal = x.Tanggal,
                   Keterangan = x.Keterangan,
                   Kurs = x.Kurs,
                   Saldo = string.IsNullOrEmpty(bankawal.Kurs) ? x.Saldo : x.KSaldo


               })
                .ToList();

            Transaksi.AddRange(Rincian);
            SaldoAwal = 0;

            //  Transaksi = Transaksi.Select(i => { SaldoAwal += i.Saldo; i.Balance = SaldoAwal; return i; }).ToList();
            foreach (var item in Transaksi)
            {

                SaldoAwal = SaldoAwal + item.Saldo;
                item.Balance = SaldoAwal;
            }

            return Transaksi;
        }


        public List<RekeningView> CetakSourceBank(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode, string[] kodeBanks)
        {

            List<RekeningView> Transaksi = new List<RekeningView>();


            var Rincian = from transH in _context.CbTransHs
                          join transD in _context.CbTransDs on transH.CbTransHId equals transD.CbTransHId
                          where sourceCode.Contains(transD.SrcCode) && kodeBanks.Contains(transH.KodeBank) && (Tanggal1.Date <= transH.Tanggal.Date && transH.Tanggal.Date <= Tanggal2.Date)
                          select new RekeningView()
                          {
                              CbTransHId = transD.CbTransHId,
                              KodeBank = transH.KodeBank,
                              DocNo = transH.DocNo,
                              Tanggal = transH.Tanggal,
                              Keterangan = transD.Keterangan,
                              SrcCode = transD.SrcCode,
                              Saldo = transD.Jumlah
                          };



            Transaksi.AddRange(Rincian);


            return Transaksi;
        }

        public List<RekeningView> CetakSourceRekapBank(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode, string[] kodeBanks)
        {

            List<RekeningView> Transaksi = new List<RekeningView>();


            var Rincian = from transH in _context.CbTransHs
                          join transD in _context.CbTransDs on transH.CbTransHId equals transD.CbTransHId
                          join srcCode in _context.CbSrcCodes on transD.SrcCode equals srcCode.SrcCode
                          where sourceCode.Contains(transD.SrcCode) && kodeBanks.Contains(transH.KodeBank) && (Tanggal1.Date <= transH.Tanggal.Date && transH.Tanggal.Date <= Tanggal2.Date)
                        
                          select new RekeningView()
                          {
                              
                              Keterangan = srcCode.NamaSrc,
                              SrcCode = transD.SrcCode,
                              Saldo = transD.Jumlah
                          };

            var Rinci = Rincian.GroupBy(x => x.SrcCode)
                 .Select(cl => new RekeningView
                 {
                     Keterangan = cl.First().Keterangan,
                     SrcCode = cl.First().SrcCode,
                     Balance = cl.Sum(c => c.Saldo)
                 }).ToList();

            Transaksi.AddRange(Rinci);


            return Transaksi;
        }
        #endregion

        #region prosesCashBank

        public void prosesCashBank()
        {

            List<CbBank> MasterStock = _context.CbBanks.ToList();

            List<CbTransH> TransJual = new List<CbTransH>();


            MasterStock.ForEach(i => { i.Saldo = 0; i.KSaldo = 0; });


            MasterStock.ForEach(i => { i.Saldo = i.SldAwal; i.KSaldo = i.KSldAwal; });


            TransJual = _context.CbTransHs.OrderBy(x => x.Tanggal).Include(x => x.CbTransDs).ToList();


            foreach (var trans in TransJual)
            {
                //foreach(var item in trans.CbTransDs)
                //{
                //    MasterStock.Find(x => x.KodeBank == trans.KodeBank).Saldo += item.Jumlah;
                //    MasterStock.Find(x => x.KodeBank == trans.KodeBank).KSaldo += item.KJumlah;
                //}

                MasterStock.Find(x => x.KodeBank == trans.KodeBank).Saldo += trans.Saldo;
                MasterStock.Find(x => x.KodeBank == trans.KodeBank).KSaldo += trans.KSaldo;


            }

            foreach (var trans in MasterStock)
            {
                if (string.IsNullOrEmpty(trans.Kurs))
                {
                    trans.KSaldo = 0;
                    trans.KSldAwal = 0;
                }
            }


            _context.UpdateRange(MasterStock);


            _context.SaveChanges();


            // return Transaksi;

        }
        #endregion

        #region tarikcsv

        public async Task SaveTransactionsAsync(List<BankTransactionView> transactions, DateTime formDate, string kodeBank, string tambah, string kurang)
        {
            // Filter transactions that are selected (IsSelected == true)
            var filteredTransactions = transactions?.Where(t => t != null && t.IsSelected).ToList() ?? new List<BankTransactionView>();

            // Group transactions by date (date part only) so different times do not split groups
            var groupedTransactions = filteredTransactions.GroupBy(t => t.Tanggal.Date).ToList();

            try
            {
                foreach (var group in groupedTransactions)
                {
                    var tgl = group.Key; // DateTime representing date part
                    var dokumen = await GenerateDocumentNumberSequenceAsync(kodeBank, tgl);

                    // Determine AP/AR transactions using per-row Target when present, otherwise fall back to SrcCode
                    var apArTransactions = group.Where(t =>
                                                            ((t.Target != null) && (t.Target.Equals("AP", StringComparison.OrdinalIgnoreCase) || t.Target.Equals("AR", StringComparison.OrdinalIgnoreCase)))
                                                            || (string.IsNullOrEmpty(t.Target) && !string.IsNullOrEmpty(t.SrcCode) && (t.SrcCode.Equals("AP", StringComparison.OrdinalIgnoreCase) || t.SrcCode.Equals("AR", StringComparison.OrdinalIgnoreCase)))
                                                        ).ToList();

                    // Transactions that should be created directly in CashBank
                    var cashBankTransactions = group.Except(apArTransactions).ToList();

                    // First, handle AP/AR transactions by calling respective payment services.
                    // Call payment services before starting any local DB transaction to avoid transaction conflicts across DbContexts.
                    foreach (var trx in apArTransactions)
                    {
                        try
                        {
                            // AP/AR payments from CSV must follow the payment date in each CSV row.
                            var paymentDate = ResolveCsvPaymentDate(trx, formDate);

                            // Route by Target first, fall back to SrcCode for legacy rows
                            var effectiveTarget = !string.IsNullOrEmpty(trx.Target)
                                ? trx.Target
                                : (trx.SrcCode ?? string.Empty);

                            if (string.Equals(effectiveTarget, "AP", StringComparison.OrdinalIgnoreCase))
                            {
                                var apServiceType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Hutang.Services.IPaymentApServices" || t.FullName == "eSoft.Hutang.Services.PaymentApServices");

                                var apViewType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Hutang.View.ApTransHView");

                                var apDType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Hutang.View.ApTransDView");

                                if (apServiceType == null || apViewType == null || apDType == null)
                                    throw new InvalidOperationException("AP payment service or view types not found in loaded assemblies.");

                                var apService = _serviceProvider.GetService(apServiceType) ?? _serviceProvider.GetService(apServiceType.GetInterfaces().FirstOrDefault());
                                if (apService == null)
                                    throw new InvalidOperationException("AP payment service not registered in DI container.");

                                var apInstance = Activator.CreateInstance(apViewType);
                                var apPaymentDate = paymentDate;
                                apViewType.GetProperty("Tanggal")?.SetValue(apInstance, apPaymentDate);
                                var apHeaderDate = (DateTime)(apViewType.GetProperty("Tanggal")?.GetValue(apInstance) ?? apPaymentDate);
                                apViewType.GetProperty("KdBank")?.SetValue(apInstance, kodeBank);
                                var supplierCode = !string.IsNullOrWhiteSpace(trx.PartyCode) ? trx.PartyCode : (string.IsNullOrWhiteSpace(trx.NoPrj) ? trx.Description : trx.NoPrj);
                                apViewType.GetProperty("Supplier")?.SetValue(apInstance, supplierCode);
                                apViewType.GetProperty("Keterangan")?.SetValue(apInstance, trx.Description);

                                var listType = typeof(List<>).MakeGenericType(apDType);
                                var listInstance = Activator.CreateInstance(listType);
                                var apTransDsProp = apViewType.GetProperty("ApTransDs");
                                if (apTransDsProp != null)
                                {
                                    if (apTransDsProp.CanWrite)
                                    {
                                        apTransDsProp.SetValue(apInstance, listInstance);
                                    }
                                    else
                                    {
                                        // read-only collection: get existing and copy items into it
                                        var existing = apTransDsProp.GetValue(apInstance);
                                        if (existing != null)
                                        {
                                            var addItemMethod = existing.GetType().GetMethod("Add");
                                            if (addItemMethod != null)
                                            {
                                                var enumItems = listInstance as System.Collections.IEnumerable;
                                                if (enumItems != null)
                                                {
                                                    foreach (var it in enumItems)
                                                    {
                                                        addItemMethod.Invoke(existing, new[] { it });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var selDocsAp = trx.OutstandingDocs?.Where(d => d.IsSelected || d.Bayar > 0 || d.Discount > 0).ToList();
                                if (selDocsAp != null && selDocsAp.Any())
                                {
                                    foreach (var sdoc in selDocsAp)
                                    {
                                        var apd = Activator.CreateInstance(apDType);
                                        // payment header date
                                        apDType.GetProperty("Tanggal")?.SetValue(apd, apHeaderDate);
                                        // store original invoice date into DueDate (per UX expectation)
                                        apDType.GetProperty("DueDate")?.SetValue(apd, sdoc.Tanggal);
                                        apDType.GetProperty("Jumlah")?.SetValue(apd, sdoc.Sisa);
                                        apDType.GetProperty("Bayar")?.SetValue(apd, sdoc.Bayar);
                                        apDType.GetProperty("Discount")?.SetValue(apd, sdoc.Discount);
                                        var prop = apDType.GetProperty("Lpb") ?? apDType.GetProperty("Dokumen");
                                        prop?.SetValue(apd, sdoc.Dokumen);
                                        apDType.GetProperty("Keterangan")?.SetValue(apd, trx.Description);
                                        // set transaction code for AP details from source document if available
                                        apDType.GetProperty("KodeTran")?.SetValue(apd, string.IsNullOrEmpty(sdoc.KodeTran) ? "24" : sdoc.KodeTran);
                                        listType.GetMethod("Add")?.Invoke(listInstance, new[] { apd });
                                    }
                                }
                                else
                                {
                                    var apd = Activator.CreateInstance(apDType);
                                    apDType.GetProperty("Tanggal")?.SetValue(apd, apHeaderDate);
                                    apDType.GetProperty("Jumlah")?.SetValue(apd, trx.Amount);
                                    apDType.GetProperty("Bayar")?.SetValue(apd, trx.Amount);
                                    apDType.GetProperty("Keterangan")?.SetValue(apd, trx.Description);
                                    apDType.GetProperty("KodeTran")?.SetValue(apd, "24");
                                    listType.GetMethod("Add")?.Invoke(listInstance, new object[] { apd });
                                }

                                decimal totalBayarAp = 0m;
                                decimal totalDiscountAp = 0m;
                                var selDocsForAp = trx.OutstandingDocs?.Where(d => d.IsSelected || d.Bayar > 0 || d.Discount > 0).ToList();
                                if (selDocsForAp != null && selDocsForAp.Any())
                                {
                                    totalBayarAp = selDocsForAp.Sum(s => s.Bayar);
                                    totalDiscountAp = selDocsForAp.Sum(s => s.Discount);
                                }
                                else
                                {
                                    totalBayarAp = trx.Amount;
                                    totalDiscountAp = 0m;
                                }

                                var propJumBayar = apViewType.GetProperty("JumBayar");
                                if (propJumBayar != null && propJumBayar.CanWrite)
                                    propJumBayar.SetValue(apInstance, totalBayarAp);

                                var propJumDiskon = apViewType.GetProperty("JumDiskon");
                                if (propJumDiskon != null && propJumDiskon.CanWrite)
                                    propJumDiskon.SetValue(apInstance, totalDiscountAp);

                                var propJumHutang = apViewType.GetProperty("JumHutang");
                                if (propJumHutang != null && propJumHutang.CanWrite)
                                    propJumHutang.SetValue(apInstance, totalBayarAp + totalDiscountAp);

                                var addMethod = apService.GetType().GetMethod("AddTransH");
                                if (addMethod == null) throw new InvalidOperationException("AddTransH method not found on AP service.");

                                addMethod.Invoke(apService, new object[] { apInstance });
                            }
                            else if (string.Equals(effectiveTarget, "AR", StringComparison.OrdinalIgnoreCase))
                            {
                                var arServiceType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Piutang.Services.IPaymentArServices" || t.FullName == "eSoft.Piutang.Services.PaymentArServices");

                                var arViewType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Piutang.View.ArTransHView");

                                var arDType = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(a => a.GetTypesSafe())
                                    .FirstOrDefault(t => t.FullName == "eSoft.Piutang.View.ArTransDView");

                                if (arServiceType == null || arViewType == null || arDType == null)
                                    throw new InvalidOperationException("AR payment service or view types not found in loaded assemblies.");

                                var arService = _serviceProvider.GetService(arServiceType) ?? _serviceProvider.GetService(arServiceType.GetInterfaces().FirstOrDefault());
                                if (arService == null)
                                    throw new InvalidOperationException("AR payment service not registered in DI container.");

                                var arInstance = Activator.CreateInstance(arViewType);
                                arViewType.GetProperty("Tanggal")?.SetValue(arInstance, paymentDate);
                                var arHeaderDate = (DateTime)(arViewType.GetProperty("Tanggal")?.GetValue(arInstance) ?? paymentDate);
                                arViewType.GetProperty("KdBank")?.SetValue(arInstance, kodeBank);
                                var customerCode = !string.IsNullOrWhiteSpace(trx.PartyCode) ? trx.PartyCode : (string.IsNullOrWhiteSpace(trx.NoPrj) ? trx.Description : trx.NoPrj);
                                arViewType.GetProperty("Customer")?.SetValue(arInstance, customerCode);
                                arViewType.GetProperty("Keterangan")?.SetValue(arInstance, trx.Description);

                                var listType = typeof(List<>).MakeGenericType(arDType);
                                var listInstance = Activator.CreateInstance(listType);
                                var arTransDsProp = arViewType.GetProperty("ArTransDs");
                                if (arTransDsProp != null)
                                {
                                    if (arTransDsProp.CanWrite)
                                    {
                                        arTransDsProp.SetValue(arInstance, listInstance);
                                    }
                                    else
                                    {
                                        var existing = arTransDsProp.GetValue(arInstance);
                                        if (existing != null)
                                        {
                                            var addItemMethod = existing.GetType().GetMethod("Add");
                                            if (addItemMethod != null)
                                            {
                                                var enumItems = listInstance as System.Collections.IEnumerable;
                                                if (enumItems != null)
                                                {
                                                    foreach (var it in enumItems)
                                                    {
                                                        addItemMethod.Invoke(existing, new[] { it });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var selDocsAr = trx.OutstandingDocs?.Where(d => d.IsSelected || d.Bayar > 0 || d.Discount > 0).ToList();
                                if (selDocsAr != null && selDocsAr.Any())
                                {
                                    foreach (var sdoc in selDocsAr)
                                    {
                                        var ard = Activator.CreateInstance(arDType);
                                        // payment header date
                                        arDType.GetProperty("Tanggal")?.SetValue(ard, arHeaderDate);
                                        // store original invoice date into DueDate (per UX expectation)
                                        arDType.GetProperty("DueDate")?.SetValue(ard, sdoc.Tanggal);
                                        arDType.GetProperty("Jumlah")?.SetValue(ard, sdoc.Sisa);
                                        arDType.GetProperty("Bayar")?.SetValue(ard, sdoc.Bayar);
                                        arDType.GetProperty("Discount")?.SetValue(ard, sdoc.Discount);
                                        var prop = arDType.GetProperty("Lpb") ?? arDType.GetProperty("Dokumen");
                                        prop?.SetValue(ard, sdoc.Dokumen);
                                        arDType.GetProperty("Keterangan")?.SetValue(ard, trx.Description);
                                        // set transaction code for AR details from source document if available
                                        arDType.GetProperty("KodeTran")?.SetValue(ard, string.IsNullOrEmpty(sdoc.KodeTran) ? "14" : sdoc.KodeTran);
                                        listType.GetMethod("Add")?.Invoke(listInstance, new[] { ard });
                                    }
                                }
                                else
                                {
                                    var ard = Activator.CreateInstance(arDType);
                                    arDType.GetProperty("Tanggal")?.SetValue(ard, arHeaderDate);
                                    arDType.GetProperty("Jumlah")?.SetValue(ard, trx.Amount);
                                    arDType.GetProperty("Bayar")?.SetValue(ard, trx.Amount);
                                    arDType.GetProperty("Keterangan")?.SetValue(ard, trx.Description);
                                    arDType.GetProperty("KodeTran")?.SetValue(ard, "14");
                                    listType.GetMethod("Add")?.Invoke(listInstance, new[] { ard });
                                }

                                decimal totalBayarAr = 0m;
                                decimal totalDiscountAr = 0m;
                                var selDocsForAr = trx.OutstandingDocs?.Where(d => d.IsSelected || d.Bayar > 0 || d.Discount > 0).ToList();
                                if (selDocsForAr != null && selDocsForAr.Any())
                                {
                                    totalBayarAr = selDocsForAr.Sum(s => s.Bayar);
                                    totalDiscountAr = selDocsForAr.Sum(s => s.Discount);
                                }
                                else
                                {
                                    totalBayarAr = trx.Amount;
                                    totalDiscountAr = 0m;
                                }

                                var propArJumBayar = arViewType.GetProperty("JumBayar");
                                if (propArJumBayar != null && propArJumBayar.CanWrite)
                                    propArJumBayar.SetValue(arInstance, totalBayarAr);

                                var propArJumDiskon = arViewType.GetProperty("JumDiskon");
                                if (propArJumDiskon != null && propArJumDiskon.CanWrite)
                                    propArJumDiskon.SetValue(arInstance, totalDiscountAr);

                                var propArJumPiutang = arViewType.GetProperty("JumPiutang");
                                if (propArJumPiutang != null && propArJumPiutang.CanWrite)
                                    propArJumPiutang.SetValue(arInstance, totalBayarAr + totalDiscountAr);

                                var addMethod = arService.GetType().GetMethod("AddTransH");
                                if (addMethod == null) throw new InvalidOperationException("AddTransH method not found on AR service.");

                                addMethod.Invoke(arService, new[] { arInstance });
                            }
                        }
                        catch (Exception)
                        {
                            // Payment service failed - rethrow so caller can handle/report. Do not attempt to rollback here because services manage their own transactions.
                            throw;
                        }
                    }

                    // If there are no cash-bank transactions for this date, skip creating a CbTransH here
                    if (!cashBankTransactions.Any())
                    {
                        continue;
                    }

                    // Retrieve bank info for kurs and update operations
                    var bankInfo = await _context.CbBanks.SingleOrDefaultAsync(b => b.KodeBank == kodeBank);
                    decimal bankKurs = 0m;
                    if (bankInfo != null && !string.IsNullOrWhiteSpace(bankInfo.Kurs))
                    {
                        var tmp = System.Text.RegularExpressions.Regex.Replace(bankInfo.Kurs ?? string.Empty, "[^0-9,.-]", "");
                        tmp = tmp.Replace(',', '.');
                        if (!decimal.TryParse(tmp, NumberStyles.Any, CultureInfo.InvariantCulture, out bankKurs))
                            bankKurs = 0m;
                    }

                    // Calculate saldo for the new header (base currency) only for cash-bank transactions
                    decimal totalSaldo = cashBankTransactions.Sum(trx => trx.Type == "CR" ? trx.Amount : -trx.Amount);
                    decimal totalKSaldo = bankKurs != 0m ? cashBankTransactions.Sum(trx => (trx.Type == "CR" ? trx.Amount : -trx.Amount)) : 0m;

                    // Use a local DB transaction for cash-bank DB updates for this group
                    await using (var dbTrans = await _context.Database.BeginTransactionAsync())
                    {
                        // Find existing header for the same document number
                        var existingCbTransH = await _context.CbTransHs
                            .Include(h => h.CbTransDs)
                            .SingleOrDefaultAsync(h => h.DocNo == dokumen);

                        if (existingCbTransH != null)
                        {
                            var bank = await _context.CbBanks.SingleOrDefaultAsync(b => b.KodeBank == existingCbTransH.KodeBank);
                            if (bank != null)
                            {
                                bank.Saldo -= existingCbTransH.Saldo;
                                bank.KSaldo -= existingCbTransH.KSaldo;
                                _context.CbBanks.Update(bank);
                            }

                            _context.CbTransDs.RemoveRange(existingCbTransH.CbTransDs);
                            _context.CbTransHs.Remove(existingCbTransH);
                        }

                        var newCbTransH = new CbTransH
                        {
                            KodeBank = kodeBank,
                            DocNo = dokumen,
                            Tanggal = tgl,
                            Keterangan = "Transaksi " + tgl.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Saldo = totalSaldo,
                            KSaldo = totalKSaldo,
                            CbTransDs = new List<CbTransD>()
                        };

                        foreach (var transaction in cashBankTransactions)
                        {
                            var jumlah = transaction.Type == "CR" ? transaction.Amount : -transaction.Amount;
                            var kjumlah = bankKurs != 0m ? jumlah : 0m;

                            var newCbTransD = new CbTransD
                            {
                                NoPrj = transaction.NoPrj,
                                SrcCode = (string.IsNullOrEmpty(transaction.SrcCode) ? (transaction.Type == "CR" ? tambah : kurang) : transaction.SrcCode),
                                Keterangan = transaction.Description,
                                Jumlah = jumlah,
                                Terima = transaction.Type == "CR" ? transaction.Amount : 0,
                                Bayar = transaction.Type == "CR" ? 0 : transaction.Amount,
                                KTerima = (bankKurs != 0m && transaction.Type == "CR") ? transaction.Amount : 0,
                                KBayar = (bankKurs != 0m && transaction.Type != "CR") ? transaction.Amount : 0,
                                KJumlah = kjumlah,
                                KValue = bankKurs != 0m ? bankKurs : 0m,
                                Kurs = bankInfo?.Kurs,
                                CbTransH = newCbTransH
                            };

                            newCbTransH.CbTransDs.Add(newCbTransD);

                            if (bankInfo != null)
                            {
                                if (transaction.Type == "CR")
                                {
                                    bankInfo.Saldo += transaction.Amount;
                                    if (bankKurs != 0m) bankInfo.KSaldo += transaction.Amount;
                                }
                                else
                                {
                                    bankInfo.Saldo -= transaction.Amount;
                                    if (bankKurs != 0m) bankInfo.KSaldo -= transaction.Amount;
                                }
                                _context.CbBanks.Update(bankInfo);
                            }
                        }

                        _context.CbTransHs.Add(newCbTransH);
                        await _context.SaveChangesAsync();
                        await dbTrans.CommitAsync();
                    }
                }
            }
            catch (Exception)
            {
                // Rethrow to caller; individual group DB transactions are rolled back locally.
                throw;
            }
        }

        
        private string GenerateDocumentNumber(string kodeBank, DateTime tgl)
        {
            var cTh = tgl.Year.ToString();
            var cBl = tgl.Month.ToString("D2");
            var cDay = tgl.Day.ToString("D2");

            return kodeBank + cTh + cBl + cDay;
        }

        private async Task<string> GenerateDocumentNumberSequenceAsync(string kodeBank, DateTime tgl)
        {
            // base format: KodeBank + yyyyMMdd
            var baseNo = kodeBank + tgl.ToString("yyyyMMdd");

            // get existing docnos that start with the base
            var existing = await _context.CbTransHs
                .Where(h => h.DocNo.StartsWith(baseNo))
                .Select(h => h.DocNo)
                .ToListAsync();

            if (existing == null || existing.Count == 0)
            {
                return baseNo + "-00001";
            }

            int maxSeq = 0;
            foreach (var doc in existing)
            {
                var idx = doc.LastIndexOf('-');
                if (idx >= 0 && idx + 1 < doc.Length)
                {
                    var suffix = doc.Substring(idx + 1);
                    if (int.TryParse(suffix, out int seq))
                    {
                        if (seq > maxSeq) maxSeq = seq;
                    }
                }
            }

            return baseNo + "-" + (maxSeq + 1).ToString("00000");
        }

        private DateTime ResolveCsvPaymentDate(BankTransactionView trx, DateTime fallbackDate)
        {
            if (trx != null && trx.Tanggal != default)
                return trx.Tanggal.Date;

            if (!string.IsNullOrWhiteSpace(trx?.Date))
            {
                var raw = trx.Date.Trim();
                if (DateTime.TryParseExact(raw,
                                           new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" },
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out var parsed))
                {
                    return parsed.Date;
                }
            }

            return fallbackDate.Date;
        }


        #endregion
    }

}
