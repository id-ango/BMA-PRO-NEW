
using eSoft.Ledger.Model;
using eSoft.Ledger.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Ledger.Data;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Ledger.Services
{
   
    public class LedgerServices : ILedgerServices
    {

        private readonly IDbContextFactory<DbContextLedger> _context;

        public LedgerServices(IDbContextFactory<DbContextLedger> context)
        {
            _context = context;
        }

        public List<GlAccount> GetGlAccount()
        {
            using var context = CreateContext();
            return context.GlAccounts.OrderBy(x => x.GlAcct).ToList();
        }

        public bool CekKdAkun(string kodeAkun)
        {
            string test = kodeAkun.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.GlAccounts.Where(x => x.GlAcct == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public GlAccount GetGlAccountId(int id)
        {
            using var context = CreateContext();
            return context.GlAccounts.Where(x => x.GlAccountId == id).FirstOrDefault();
        }

        public bool AddGlAccount(GlAccountView glakun)
        {
            string test = glakun.GlAcct.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.GlAccounts.Where(x => x.GlAcct == test).ToList();
            if (cekFirst.Count == 0)
            {
                GlAccount Akun = new GlAccount()
                {
                    GlAcct = glakun.GlAcct.ToUpper(),
                    GlNama = glakun.GlNama,
                 
                    GlTipe = (int)glakun.GlStatus,
                    NamaLengkap = glakun.NamaLengkap



                };
                context.GlAccounts.Add(Akun);
                 context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditGlAccount(GlAccountView glakun)
        {
            try
            {
                using var context = CreateContext();
                var ExistingBank = context.GlAccounts.Where(x => x.GlAccountId == glakun.GlAccountId).FirstOrDefault();
                if (ExistingBank != null)
                {
                    ExistingBank.GlNama = glakun.GlNama;
                    ExistingBank.GlTipe = (int)glakun.GlStatus;
                    ExistingBank.NamaLengkap = glakun.NamaLengkap;
                   
                    context.GlAccounts.Update(ExistingBank);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelGlAccount(int banks)
        {
            try
            {
                using var context = CreateContext();
                var ExistingBank = context.GlAccounts.Where(x => x.GlAccountId == banks).FirstOrDefault();
                if (ExistingBank != null)
                {
                    context.GlAccounts.Remove(ExistingBank);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #region GlCode Class

        public bool CekKodeGL(string kodeAkun)
        {
            string test = kodeAkun.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.GlCodes.Any(x => x.KodeGl == test);
            if (!cekFirst)
            {
                return false;
            }
            return true;
        }

        public List<GlCode> GetGlCode()
        {
            using var context = CreateContext();
            return context.GlCodes.OrderBy(x => x.KodeGl).ToList();
        }

        public GlCode GetGlCodeId(int id)
        {
            using var context = CreateContext();
            return context.GlCodes.Where(x => x.GlCodeId == id).FirstOrDefault();
        }

        public async Task<bool> AddGlCode(GlCodeView codeview)
        {
            string test = codeview.KodeGl.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.GlCodes.Where(x => x.KodeGl == test).ToList();
            if (cekFirst.Count == 0)
            {
                GlCode Division = new GlCode()
                {
                    KodeGl = codeview.KodeGl.ToUpper(),
                    NamaGl = codeview.NamaGl

                };
                context.GlCodes.Add(Division);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditGlCode(GlCodeView codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDiv = context.GlCodes.Where(x => x.GlCodeId == codeview.GlCodeId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaGl = codeview.NamaGl;


                    context.GlCodes.Update(ExistingDiv);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelGlCode(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDiv = context.GlCodes.Where(x => x.GlCodeId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    context.GlCodes.Remove(ExistingDiv);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #endregion GlCode Class

        #region TransLedger
        public GlTransH GetTrans(int id)
        {
            using var context = CreateContext();
            return context.GlTransHs.Include(p => p.GlTransDs).Where(x => x.GlTransHId == id).FirstOrDefault();
        }
        public GlTransH GetTransDoc(string docno)
        {
            using var context = CreateContext();
            return context.GlTransHs.Include(p => p.GlTransDs).Where(x => x.DocNo == docno).FirstOrDefault();
        }

        public List<GlTransH> GetTransH()
        {
            List<GlTransH> arTrans = new List<GlTransH>();
            try
            {
                using var context = CreateContext();
                arTrans = context.GlTransHs.OrderByDescending(x => x.Tanggal).ToList();
               
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

        public List<GlTransD> GetTransD()
        {
            using var context = CreateContext();
            return context.GlTransDs.ToList();
        }

        public GlTransH AddTransH(GlTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            GlTransH transH = new()
            {
                DocNo = GetNumber(),
                KodeGl = String.IsNullOrEmpty(trans.KodeGl) ? " " : trans.KodeGl.ToUpper() ,
                Tanggal = trans.Tanggal,
                GlMemo = trans.GlMemo,
               // Kurs = trans.Kurs,
                Debet = trans.Debet,
                Kredit = trans.Kredit,
                Saldo = trans.Saldo,
                GlTransDs = new List<GlTransD>()
            };
            foreach (var item in trans.GlTransDs)
            {
                transH.GlTransDs.Add(new GlTransD()
                {
                    GlAcct = item.GlAcct,
                    Keterangan = item.Keterangan,
                    Debet = item.Debet,
                    Kredit = item.Kredit,                  
                    Jumlah = item.Jumlah,
                    
                    
                 
                });
            }
            
                using var context = CreateContext();
                context.GlTransHs.Add(transH);
                context.SaveChanges();

            var TempTrans = GetTransDoc(transH.DocNo);

            return TempTrans;
            // return true;


        }

        public GlTransH EditTransH(GlTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();



            try
            {
                using var context = CreateContext();
                var ExistingTrans = context.GlTransHs.Where(x => x.GlTransHId == trans.GlTransHId).FirstOrDefault();
                if (ExistingTrans != null)
                {

                    context.GlTransHs.Remove(ExistingTrans);

                  
                    /* update */

                    GlTransH transH = new()
                    {
                        //  transH.DocNo = ExistingTrans.DocNo;
                        DocNo = ExistingTrans.DocNo,
                       
                        KodeGl = String.IsNullOrEmpty(trans.KodeGl) ? " " : trans.KodeGl.ToUpper(),
                        Tanggal = trans.Tanggal,
                        GlMemo = trans.GlMemo,
                        Kurs = trans.Kurs,
                        Kredit = trans.Kredit,
                        Debet = trans.Debet,
                        Saldo = trans.Saldo,

                        GlTransDs = new List<GlTransD>()
                    };
                    foreach (var item in trans.GlTransDs)
                    {
                        transH.GlTransDs.Add(new GlTransD()
                        {
                            GlAcct = item.GlAcct,
                            Keterangan = item.Keterangan,
                            Debet = item.Debet,
                            Kredit = item.Kredit,
                            Jumlah = item.Jumlah
                            
                        });
                    }
                   
                    context.GlTransHs.Add(transH);
                    context.SaveChanges();

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
                using var context = CreateContext();
                var ExistingTrans = context.GlTransHs.Where(x => x.GlTransHId == id).FirstOrDefault();
                if (ExistingTrans != null)
                {
                  //  var cekFirst = _context.ArPiutngs.Where(x => x.Dokumen == ExistingTrans.Bukti).FirstOrDefault();
                  //  var customer = (from e in _context.ArCusts where e.Customer == ExistingTrans.Customer select e).FirstOrDefault();

                 //   customer.Piutang -= ExistingTrans.Jumlah;


                 //   _context.ArCusts.Update(customer);
                    context.GlTransHs.Remove(ExistingTrans);
                //    _context.ArPiutngs.Remove(cekFirst);
                    await context.SaveChangesAsync();
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

        public string GetNumber()
        {
            using var context = CreateContext();
            string kodeno = "GLJ";
            string kodeurut = kodeno + "-";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = context.GlTransHs.Where(x => x.DocNo.Substring(0, 10).Equals(xbukti)).ToList();
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

        private DbContextLedger CreateContext()
        {
            return _context.CreateDbContext();
        }

    }
}
