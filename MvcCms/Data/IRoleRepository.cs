using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Data
{
    public interface IRoleRepository : IDisposable
    {
        Task<IdentityRole> GetRoleByNameAsync(string name);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();

        Task CreateAsync(IdentityRole role);
    }
}
