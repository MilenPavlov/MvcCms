using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Models;

namespace MvcCms.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly CmsUserStore _store;
        private readonly CmsUserManager _manager;

        public UserRepository()
        {
            _store = new CmsUserStore();
            _manager = new CmsUserManager(_store);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }

        public async Task<CmsUser> GetUserByNameAsync(string username)
        {
            return await _store.FindByNameAsync(username);
        }

        public async Task<IEnumerable<CmsUser>> GetAllUsersAsync()
        {
            return await _store.Users.ToArrayAsync();
        }

        public async Task CreateAsync(CmsUser user, string password)
        {
             await _manager.CreateAsync(user, password);
        }

        public async Task DeleteAsync(CmsUser user)
        {
            var result = await _manager.DeleteAsync(user);
        }

        public async Task UpdateAsync(CmsUser user)
        {
            var result = await  _manager.UpdateAsync(user);
        }

        public bool VerifyUserPassword(string hashedPassword, string providedPassword)
        {
            return _manager.PasswordHasher.VerifyHashedPassword(hashedPassword, providedPassword) ==
                   PasswordVerificationResult.Success;
        }

        public string HashPassword(string password)
        {
            return _manager.PasswordHasher.HashPassword(password);
        }

        public async Task AddUserToRoleAsync(CmsUser newUser, string role)
        {
            await _manager.AddToRoleAsync(newUser.Id, role);
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public async Task RemoveUserFromRoleAsync(CmsUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
        }

        public async Task<CmsUser> GetLoginUserAsync(string userName, string password)
        {
            return await _manager.FindAsync(userName, password);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user)
        {
            return await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
