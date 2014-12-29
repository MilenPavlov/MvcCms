using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Models;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {
        Task<CmsUser> GetUserByNameAsync(string username);
        Task<IEnumerable<CmsUser>> GetAllUsersAsync();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);

        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);

        Task AddUserToRoleAsync(CmsUser newUser, string role);

        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);

        Task RemoveUserFromRoleAsync(CmsUser user, params string[] roleNames);

        Task<CmsUser> GetLoginUserAsync(string userName, string password);

        Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user);
    }
}
