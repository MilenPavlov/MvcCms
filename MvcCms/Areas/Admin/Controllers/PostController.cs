using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;
using MvcCms.Models;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("post")]
    public class PostController : Controller
    {

        private readonly IPostRepository _repository;

        public PostController() : this(new PostRepository())
        {
            
        }

        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        // /admin/post
        // GET: Admin/Post
        [Route("")]
        public ActionResult Index()
        {
            var posts = _repository.GetAll();
            return View(posts);
        }
        // admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public ActionResult Edit(string postId)
        {
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }
            
            return View(post);
        }

        // admin/post/edit/post-to-edit
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit/{postId}")]
        public ActionResult Edit( string postId, Post model)
        {

          
            if (!ModelState.IsValid)
            {
                return View(model);
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
        public ActionResult Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFrendly();

            model.Tags = model.Tags.Select(x => x.MakeUrlFrendly()).ToList();
            model.Created = DateTime.Now;
            model.AuthorId = "b9fe6ea0-1dd4-47e2-b05b-bffb6568bdbd";
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
    }
}