using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Administration.Services
{
    public interface IAdministrationServices
    {
        List<IdentityRole> GetRoles();
        IdentityRole GetRoleId(string id);
        IdentityRole GetRoleName(string id);
        bool CekNameRole(string kodeBank);
        bool AddRoles(IdentityRole banks);
        Task<bool> EditRoles(IdentityRole banks);
        Task<bool> DelRoles(string banks);
    }
}
