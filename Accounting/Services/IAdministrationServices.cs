using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.Services.View;

namespace Accounting.Services
{
    public interface IAdministrationServices
    {
        List<IdentityRole> GetRoles();
        IdentityRole GetRoleId(string id);
        IdentityRole GetRoleName(string id);
        bool CekNameRole(string kodeBank);
        bool AddRoles(IdentityRole banks);
        Task<bool> EditRoles(IdentityRole banks);
        bool DelRoles(string banks);

        List<IdentityView> GetUsersRole();
        bool SaveRole(string idUser, string idRole);
        Task<bool> DeleteUser(string idUser);
        Task<bool> ResetUserPassword(string idUser, string newPassword);
        Task SeedUserRole();

    }
}
