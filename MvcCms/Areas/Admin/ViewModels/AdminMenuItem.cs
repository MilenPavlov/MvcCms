using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCms.Areas.Admin.ViewModels
{
    public class AdminMenuItem
    {
        public string Text { get; set; }
        public string Action { get; set; }

        public object RouteInfo { get; set; }
    }
}
