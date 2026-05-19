using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Company.Data;
using eSoft.Company.Model;
using eSoft.Company.View;
using Microsoft.EntityFrameworkCore;
namespace eSoft.Company.Services
{
    public class CompanyServices : ICompanyServices
    {
        private readonly DbContextCompany _context;

        public CompanyServices(DbContextCompany context)
        {
            _context = context;
        }

        #region Company Class

        public List<CoSetup> GetCompany()
        {
            return _context.CoSetups.ToList();
        }

        public CoSetup GetCompanyId(int id)
        {
            return _context.CoSetups.Where(x => x.CoSetupId == id).FirstOrDefault();
        }
        public CoSetup GetCompanyKd(string id)
        {
            return _context.CoSetups.Where(x => x.CoKode == id).FirstOrDefault();
        }

        public bool CekKdCompany(string kodeBank)
        {
            string test = kodeBank.ToUpper();
            var cekFirst = _context.CoSetups.Where(x => x.CoKode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddCompany(CoSetupView banks)
        {
            string test = banks.CoKode.ToUpper();
            var cekFirst = _context.CoSetups.Where(x => x.CoKode == test).ToList();
            if (cekFirst.Count == 0)
            {
                CoSetup Bank = new()
                {
                    CoKode = banks.CoKode.ToUpper(),
                    CoName = banks.CoName,
                    Account1 = banks.Account1,
                    Account2 = banks.Account2,
                    Account3 = banks.Account3,
                    Account4 = banks.Account4,
                    Account5 = banks.Account5,
                    Account6 = banks.Account6

                };
                _context.CoSetups.Add(Bank);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditCompany(CoSetupView banks)
        {
            try
            {
                var ExistingBank = _context.CoSetups.Where(x => x.CoSetupId == banks.CoSetupId).FirstOrDefault();
                if (ExistingBank != null)
                {
                    ExistingBank.CoName = banks.CoName;
                    ExistingBank.Account1 = banks.Account1;
                     ExistingBank.Account2 = banks.Account2;
                     ExistingBank.Account3 = banks.Account3;
                     ExistingBank.Account4 = banks.Account4;
                     ExistingBank.Account5 = banks.Account5;
                    ExistingBank.Account6 = banks.Account6;

                    _context.CoSetups.Update(ExistingBank);
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
                var ExistingBank = _context.CoSetups.Single(item => item.CoSetupId == banks);
                //  var ExistingBank = _context.Banks.Where(x => x.CbBankId == banks).FirstOrDefault();
                if (ExistingBank != null)
                {
                    _context.CoSetups.Remove(ExistingBank);
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
        #endregion
    }
}
