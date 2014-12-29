using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;

namespace MvcCms.Data
{
    public interface IPostRepository
    {
        Post Get(string id);

        void Edit(string id, Post updatedItem);

        void Create(Post model);

        IEnumerable<Post> GetAll();

        void Delete(string id);
    }
}
