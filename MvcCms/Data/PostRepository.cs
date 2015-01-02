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
        public int CountPublished
        {
            get
            {
                using (var db = new CmsContext())
                {
                    return db.Posts
                        .Count(p => p.Published <= DateTime.Now);
                }
                    
            }
        }

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

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            using (var db = new CmsContext())
            {
                var posts =  await db.Posts
                    .Include("Author")
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p=>p.Published)
                    .ToArrayAsync();

                return posts;
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByTagAsync(string tagId)
        {
            using (var db = new CmsContext())
            {
                var posts = await db.Posts
                    .Include("Author")
                    .Where(p => p.CombinedTags.Contains(tagId)).ToListAsync();

                return posts
                       .Where(t => t.Tags.Contains(tagId, StringComparer.CurrentCultureIgnoreCase))
                       .ToList();
            }
        }

        public async Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1)*pageSize;

            using (var db = new CmsContext())
            {
                return await db.Posts
                    .Include("Author")
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToArrayAsync();
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
