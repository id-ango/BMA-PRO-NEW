using eSoft.Company.Model;
using eSoft.Company.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eSoft.Company.Services
{
    public interface ICompanyServices
    {
        List<CoSetup> GetCompany();
        CoSetup GetCompanyId(int id);
        CoSetup GetCompanyKd(string id);
        bool CekKdCompany(string kodeBank);
        bool AddCompany(CoSetupView banks);
        Task<bool> EditCompany(CoSetupView banks);

        Task<bool> DelBank(int banks);



    }
}
