using System;
using System.Collections.Generic;
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

        public IEnumerable<Post> GetAll()
        {
            using (var db = new CmsContext())
            {
                return db.Posts.Include("Author").OrderByDescending(x => x.Created).ToArray();
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
    }
}
