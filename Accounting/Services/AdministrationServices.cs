using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Accounting.Data;
using Accounting.Services.View;


namespace Accounting.Services
{
    public  class AdministrationServices : IAdministrationServices
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdministrationServices(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region Roles Class

        public List<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public IdentityRole GetRoleId(string id)
        {
            return _context.Roles.Where(x => x.Id == id).First();
        }

        public IdentityRole GetRoleName(string id)
        {
            return _context.Roles.Where(x => x.NormalizedName == id).First();
        }

        public bool CekNameRole(string kodeBank)
        {
            string test = kodeBank?.Trim().ToUpper() ?? string.Empty;
            var cekFirst = _context.Roles.Where(x => x.NormalizedName == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddRoles(IdentityRole banks)
        {
            string roleName = banks.Name?.Trim() ?? string.Empty;
            string test = roleName.ToUpper();
            var cekFirst = _context.Roles.Where(x => x.NormalizedName == test).ToList();
            if (cekFirst.Count == 0 && !string.IsNullOrWhiteSpace(roleName))
            {
                IdentityRole Bank = new()
                {
                    Name = roleName,
                   NormalizedName = roleName.ToUpper(),
                    

                };
                _context.Roles.Add(Bank);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditRoles(IdentityRole banks)
        {
            try
            {
                var ExistingBank = _context.Roles.Where(x =>x.Id == banks.Id).FirstOrDefault();
                var roleName = banks.Name?.Trim() ?? string.Empty;
                if (ExistingBank != null && !string.IsNullOrWhiteSpace(roleName))
                {
                    ExistingBank.Name = roleName;
                    ExistingBank.NormalizedName = roleName.ToUpper();
                    

                    _context.Roles.Update(ExistingBank);
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

        public bool DelRoles(string banks)
        {
            try
            {
                var ExistingBank = _context.Roles.Single(item => item.Id == banks);
                //  var ExistingBank = _context.Banks.Where(x => x.CbBankId == banks).FirstOrDefault();
                if (ExistingBank != null)
                {
                    _context.Roles.Remove(ExistingBank);
                    _context.SaveChanges();
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
        #endregion Roles Class

        #region User class
        public List<IdentityView> GetUsersRole()
        {
            List<IdentityView> Banks = new List<IdentityView>();
            var userRoles = _context.UserRoles.ToList();

            var userList =  _context.Users.ToList();

            Banks = (from e in _context.Users                   
                       select new IdentityView
                       {
                           IdUser = e.Id,
                           Name = e.UserName,
                          
                       }).ToList();
           
            foreach(var user in Banks)
            {
                user.IdRole = GetIdRole(user.IdUser);
            }

            return Banks;
        }

       private string GetIdRole(string id)
        {
           var userRoles =  _context.UserRoles.Where(x => x.UserId == id).FirstOrDefault();
            if (userRoles != null)
            {
                return userRoles.RoleId;
            }
            return " ";
        }

        public bool SaveRole(string idUser, string idRole)
        {
            IdentityUserRole<string> role = new IdentityUserRole<string>();

            if (idRole != null && idRole != string.Empty)
            {
              var userrole =   _context.UserRoles.Where(x => x.UserId == idUser).FirstOrDefault();
                if(userrole != null)
                {
                  //  userrole.RoleId = idRole;
                    _context.UserRoles.Remove(userrole);
                }
                    
           //     else
          //      {
                    role = new()
                    {
                        RoleId = idRole,
                        UserId = idUser,


                    };
                    _context.UserRoles.Add(role);
          //      }
                _context.SaveChanges();
            }
            return true;

        }

        public async Task<bool> DeleteUser(string idUser)
        {
            var user = await _userManager.FindByIdAsync(idUser);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ResetUserPassword(string idUser, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(idUser);
            if (user == null || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        #endregion

        #region seedUserRole

        public async Task SeedUserRole()
        {
            var users = _context.Users.ToList();
            var roles = _context.Roles.ToList();
          //  var userRole = _context.UserRoles.ToList();
            IdentityUserRole<string> role = new IdentityUserRole<string>();
            IdentityUser user = new IdentityUser();
            IdentityRole role2 = new IdentityRole();

            if (!roles.Any() && !users.Any())
            {
                role2 = new()
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };

                user = new()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    
                   
                };
                PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
                user.PasswordHash = hasher.HashPassword(user, "ADMIN01");
                

                _context.Users.Add(user);
                _context.Roles.Add(role2);

               await _context.SaveChangesAsync();

                var idRoles = _context.Roles.Where(x => x.NormalizedName == "ADMIN").FirstOrDefault();
                var idUsers = _context.Users.Where(x => x.NormalizedUserName == "ADMIN@ADMIN.COM").FirstOrDefault();

                role = new()
                {
                    RoleId = idRoles.Id,
                    UserId = idUsers.Id
                };
                
                _context.UserRoles.Add(role);
              await  _context.SaveChangesAsync();
            }          

        }

        #endregion
    }
}
