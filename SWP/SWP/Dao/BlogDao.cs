using SWP.Models;

namespace SWP.Dao
{
    public class BlogDao
    {
        SWPContext context;

        public BlogDao()
        {
            context= new SWPContext();
        }
        public static List<Blog> GetBlogs()
        {
            try
            {
                using(var connection = new SWPContext())
                {
                    var list = connection.Blogs.ToList();
                    return list;
                }
            }catch(Exception ex)
            {
                return null;
            }
        }
        public static Blog GetBlogById(int id) {
            try
            {
               using (var connection = new SWPContext())
                {
                    var blog = connection.Blogs.Where(x => x.BlogId== id).FirstOrDefault();
                    return blog;
                }
            }catch(Exception ex)
            {
                return null;
            }
        }
        public static void SaveBlog(Blog blog)
        {
            try
            {
                using (var connection = new SWPContext())
                {
                    connection.Blogs.Add(blog);
                    connection.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void UpdateBlog(Blog blog)
        {
            try
            {
                using(var context = new SWPContext())
                {
                    context.Entry<Blog>(blog).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void DeleteBlog(Blog blog)
        {
            try
            {
                using(var context = new SWPContext())
                {
                    var p1 = context.Blogs.SingleOrDefault(b => b.BlogId == blog.BlogId);
                    context.Blogs.Remove(p1);
                    context.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
