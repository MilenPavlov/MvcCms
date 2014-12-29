using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Models
{
    public class CmsUser : IdentityUser
    {
        //public virtual MyUserInfo MyUserInfo { get; set; }
    //}

    //public class MyUserInfo
    //{
        public string DisplayName { get; set; }
    }
}
