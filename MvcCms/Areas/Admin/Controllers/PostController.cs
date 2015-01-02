using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;
using MvcCms.Models;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("post")]
    [Authorize]
    public class PostController : Controller
    {

        private readonly IPostRepository _repository;
        private readonly IUserRepository _users;

        public PostController() : this(new PostRepository(), new UserRepository())
        {
            
        }

        public PostController(IPostRepository repository, IUserRepository users)
        {
            _repository = repository;
            _users = users;
        }

        // /admin/post
        // GET: Admin/Post
        [Route("")]
        public async Task<ActionResult> Index()
        {
            if (!User.IsInRole("author"))
            {
                return View( await _repository.GetAllAsync());
            }

            var user = await GetLoggedIdUser();

            var posts = await _repository.GetPostsByAuthorAsync(user.Id);

            return View(posts);
        }
        // admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public async Task<ActionResult> Edit(string postId)
        {
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedIdUser();

                if (post.AuthorId != user.Id)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            return View(post);
        }

        // admin/post/edit/post-to-edit
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit/{postId}")]
        public async Task<ActionResult> Edit( string postId, Post model)
        {

          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (User.IsInRole("author"))
            {
                var user = await GetLoggedIdUser();
                var post = _repository.Get(postId);
                try
                {
                    if (post.AuthorId != user.Id)
                    {
                        return new HttpUnauthorizedResult();
                    }
                }
                catch {}
              
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFrendly();

            model.Tags = model.Tags.Select(x => x.MakeUrlFrendly()).ToList();
            try
            {
                _repository.Edit(postId, model);

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException e)
            {

                return HttpNotFound();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }

           
        }
        // /admin/post/create
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new Post());
        }

        // /admin/post/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<ActionResult> Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            var user = await GetLoggedIdUser();

            model.Id = model.Id.MakeUrlFrendly();

            model.Tags = model.Tags.Select(x => x.MakeUrlFrendly()).ToList();
            model.Created = DateTime.Now;
            model.AuthorId = user.Id;
            try
            {
                _repository.Create(model);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("key", e);
                return View(model);
            }                
        }

        [HttpGet]
        [Route("delete/{postId}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string postId)
        {
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // admin/post/delete/post-to-edit
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("delete/{postId}")]
        [Authorize(Roles = "admin, editor")]
        public ActionResult Delete(string postId, string foo)
        {
            try
            {
                _repository.Delete(postId);

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException e)
            {

                return HttpNotFound();
            }

        }

        private async Task<CmsUser> GetLoggedIdUser()
        {
            return await _users.GetUserByNameAsync(User.Identity.Name);
        }

        private bool _isDisposed;
        protected override void Dispose(bool disposing)
        {

            if (!_isDisposed)
            {
                _users.Dispose();
            }
            _isDisposed = true;

            base.Dispose(disposing);
        }
    }
}