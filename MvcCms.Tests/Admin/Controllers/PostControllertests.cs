using System;
using System.ComponentModel;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCms.Areas.Admin.Controllers;
using MvcCms.Data;
using MvcCms.Models;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace MvcCms.Tests.Admin.Controllers
{
    [TestClass]
    public class PostControllertests
    {
        [TestMethod]
        public void Edit_GetRequest()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(id)).Returns(new Post {Id = "test-post"});
            var controller = new PostController(repo);

            var result = (ViewResult)controller.Edit(id);

            var model = (Post) result.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(id)).Returns((Post)null);
            var controller = new PostController(repo);

            var result = controller.Edit(id);

            //var model = (Post)result.Model;

            Assert.IsTrue(result is HttpNotFoundResult);
        }


        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(id)).Returns((Post)null);
            var controller = new PostController(repo);

            var result = controller.Edit(id, new Post());

            //var model = (Post)result.Model;

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Get(id)).Returns(new Post { Id = "test-post" });
            var controller = new PostController(repo);
            controller.ViewData.ModelState.AddModelError("", "some error");
            var result = (ViewResult)controller.Edit(id, new Post(){Id = "test-post2"});

            var model = (Post)result.Model;

            Assert.AreEqual("test-post2", model.Id);
        }

        [TestMethod]
        public void Edit_PostRequestCallsEditAndRedirects()
        {
            //ar id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<Post>())).MustBeCalled();
             
            var controller = new PostController(repo);
            //controller.ViewData.ModelState.AddModelError("", "some error");
            var result = controller.Edit("foo", new Post() { Id = "test-post2" });

            Mock.Assert(repo);

            Assert.IsTrue(result is RedirectToRouteResult);
        }
    }
}
