using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using Microsoft.Owin.Security;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserRepository _users;
        public AdminController(IUserRepository userRepository)
        {
            _users = userRepository;
        }

        public AdminController() : this(new UserRepository())
        {
            
        }
        // GET: Admin/Admin
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var user = await _users.GetLoginUserAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "The user with supplied credentials does not exist.");
            }

            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await _users.CreateIdentityAsync(user);

            authManager.SignIn(new AuthenticationProperties()
            {
                IsPersistent = model.RememberMe 
                
            }, userIdentity);

            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;

            authManager.SignOut();


            return RedirectToAction("index", "home");
        }
    }
}