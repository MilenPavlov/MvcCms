using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Models;

namespace MvcCms.Data
{
    public class CmsUserStore : UserStore<CmsUser>
    {
        public CmsUserStore(CmsContext context): base(context)
        {
            
        }

        public CmsUserStore() :this(new CmsContext())
        {
            
        }
    }

    public class CmsUserManager : UserManager<CmsUser>
    {
        public CmsUserManager(IUserStore<CmsUser> userStore)
            : base(userStore)
        {
        }

        public CmsUserManager()
            :this(new CmsUserStore())
        {
            
        }
    }
}
