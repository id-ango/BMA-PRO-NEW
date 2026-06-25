using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.CashBank.Services
{
    public class CashBankServices : ICashBankServices
    {
        private readonly DbContextBank _context;

        public CashBankServices(DbContextBank context)
        {
            _context = context;
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
            // Filter transactions that are selected in the page
            var filteredTransactions = transactions?.Where(t => t.IsSelected).ToList() ?? new List<BankTransactionView>();

            if (!filteredTransactions.Any())
            {
                return;
            }

            // Group transactions by date
            var groupedTransactions = filteredTransactions.GroupBy(t => t.Tanggal).ToList();

            foreach (var group in groupedTransactions)
            {
                var tgl = group.Key;
                var dokumen = GenerateDocumentNumber(kodeBank, tgl);

                // Find existing header for the same document number
                var existingCbTransH = await _context.CbTransHs
                    .Include(h => h.CbTransDs)
                    .SingleOrDefaultAsync(h => h.DocNo == dokumen);

                // If existing header found, remove it and its details
                if (existingCbTransH != null)
                {
                    // Update bank balance before removal
                    var bank = await _context.CbBanks.SingleOrDefaultAsync(b => b.KodeBank == existingCbTransH.KodeBank);
                    if (bank != null)
                    {
                        bank.Saldo -= existingCbTransH.Saldo;
                    }

                    _context.CbTransHs.Remove(existingCbTransH);
                    _context.CbTransDs.RemoveRange(existingCbTransH.CbTransDs);
                }

                // Calculate saldo for the new header
                decimal totalSaldo = group.Sum(trx => trx.Type == "CR" ? trx.Amount : -trx.Amount);

                var newCbTransH = new CbTransH
                {
                    KodeBank = kodeBank,
                    DocNo = dokumen,
                    Tanggal = tgl,
                    Keterangan = "Transaksi " + tgl.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Saldo = totalSaldo,
                    CbTransDs = new List<CbTransD>() // Initialize CbTransDs list
                };

                foreach (var transaction in group)
                {
                    var newCbTransD = new CbTransD
                    {
                        NoPrj = transaction.NoPrj,
                        SrcCode = (string.IsNullOrEmpty(transaction.SrcCode) ? (transaction.Type == "CR" ? tambah : kurang) : transaction.SrcCode),
                        Keterangan = transaction.Description,
                        Jumlah = transaction.Type == "CR" ? transaction.Amount : -transaction.Amount,
                        Terima = transaction.Type == "CR" ? transaction.Amount : 0,
                        Bayar = transaction.Type == "CR" ? 0 : transaction.Amount,
                        CbTransH = newCbTransH // Associate the detail with the header
                    };

                    newCbTransH.CbTransDs.Add(newCbTransD);

                    var bankUpdate = await _context.CbBanks.SingleOrDefaultAsync(b => b.KodeBank == kodeBank);
                    if (bankUpdate != null)
                    {
                        if (transaction.Type == "CR")
                        {
                            bankUpdate.Saldo += transaction.Amount;
                        }
                        else
                        {
                            bankUpdate.Saldo -= transaction.Amount;
                        }
                    }
                }

                _context.CbTransHs.Add(newCbTransH);
            }

            await _context.SaveChangesAsync(); // Save changes after processing all groups
        }

        private string GenerateDocumentNumber(string kodeBank, DateTime tgl)
        {
            var cTh = tgl.Year.ToString();
            var cBl = tgl.Month.ToString("D2");
            var cDay = tgl.Day.ToString("D2");

            return kodeBank + cTh + cBl + cDay;
        }

        #endregion
    }

}
