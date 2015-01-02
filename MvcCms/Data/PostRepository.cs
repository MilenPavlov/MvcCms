using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public Post Get(string id)
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author").SingleOrDefault(x => x.Id == id);
            }
        }

        public void Edit(string id, Post updatedItem)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    throw  new KeyNotFoundException("A post with the id of: " 
                        + id + " does not exist in the data store.");
                }

               

                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                db.SaveChanges();
            }
        }

        public void Create(Post model)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);

                if (post != null)
                {
                    throw new ArgumentException("A posy of the id of " + model.Id + " already exists.");
                }

                

                db.Posts.Add(model);
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author").OrderByDescending(x => x.Created).ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId)
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                    .Where(x=>x.AuthorId == authorId)
                    .OrderByDescending(x => x.Created).ToArrayAsync();
            }
        }


        public void Delete(string id)
        {
            using (var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);

                if (post == null)
                {
                    throw new KeyNotFoundException("Post with id of " + id + " does not exists.");
                }

                db.Posts.Remove(post);
                db.SaveChanges();
            }
        }


        //private bool _isDisposed;
        //public void Dispose(bool disposing)
        //{
        //    if (!_isDisposed)
        //    {
        //        //_repository.Dispose();
        //        _users.Dispose();

        //    }

        //    _isDisposed = true;
        //    base.Dispose(disposing);
        //}

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
