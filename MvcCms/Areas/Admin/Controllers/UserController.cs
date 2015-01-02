using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _users;
        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _users = new UserService(ModelState, _userRepository, _roleRepository);
        }

        // GET: Admin/User
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return View(users);
        }

        [HttpGet]
        [Route("edit/{username}")]
        [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(string userName)
        {
            var currentyser = User.Identity.Name;

            if (!User.IsInRole("admin") &&
                !string.Equals(currentyser, userName, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var user = await _users.GetUserByNameAsync(userName);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }


        [HttpPost]
        [Route("edit/{username}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor, author")]
        public async  Task<ActionResult> Edit(UserViewModel model, string username)
        {
            var currentyser = User.Identity.Name;
            var isAdmin = User.IsInRole("admin");
            if (!isAdmin &&
                !string.Equals(currentyser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var userUpdated = await _users.UpdateUser(model);

            if (userUpdated)
            {
                if(isAdmin)
                    return RedirectToAction("index");
                else
                {
                    return RedirectToAction("index", "admin");
                }
            }

            return View(model);          
        }

        [HttpPost]
        [Route("delete/{username}")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Delete(string userName)
        {
            await _users.DeleteAsync(userName);

            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("create")]
        public async Task<ActionResult> Create()
        {
            var model = new UserViewModel();
            model.LoadUserRoles(await _roleRepository.GetAllRolesAsync());
            return View(model);
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (await _users.CreateAsync(model))
            {
                return RedirectToAction("index");
            }

           

            return View(model);
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _roleRepository.Dispose();
                _userRepository.Dispose();

            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}