using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
         [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "The new password and confirmation password do not match")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
          [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Role")]
        public string SelectedRole { get; set; }

        private readonly List<string>  _roles = new List<string>();
        public IEnumerable<SelectListItem> Roles
        { get { return new SelectList(_roles); } }

        public void LoadUserRoles(IEnumerable<IdentityRole> roles)
        {
            _roles.AddRange(roles.Select(x=>x.Name));
        }
    }
}
