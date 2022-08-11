using Microsoft.EntityFrameworkCore;
using Blog.Services.Interfaces;
using Blog.Models;
using Blog.Data;
using Blog.Extensions;

namespace Blog.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBlogToTagAsync(int tagId, int blogId)
        {
            try
            {
                if (!await IsBlogInTag(tagId, blogId))
                {
                    BlogPost? blog = await _context.BlogPosts.FindAsync(blogId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blog != null && tag != null)
                    {
                        tag.BlogPosts.Add(blog);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<Tag>> GetBlogTagsAsync(int blogId)
        {
            try
            {
                BlogPost blog = await _context.BlogPosts.Include(t => t.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogId);

                return blog.Tags;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetBlogTagIdsAsync(int blogId)
        {
            try
            {
                BlogPost blog = await _context.BlogPosts.Include(t => t.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogId);
                List<int> tagIds = blog.Tags.Select(b => b.Id).ToList();
                return tagIds;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> IsBlogInTag(int tagId, int blogId)
        {
            try
            {
                BlogPost? blog = await _context.BlogPosts.FindAsync(blogId);

                return await _context.Tags
                    .Include(b => b.BlogPosts)
                    .Where(t => t.Id == tagId && t.BlogPosts.Contains(blog))
                    .AnyAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveBlogFromTagAsync(int tagId, int blogId)
        {
            BlogPost? blog = await _context.BlogPosts.FindAsync(blogId);
            Tag? tag = await _context.Tags.FindAsync(tagId);

            if(tag != null && blog != null)
            {
                tag.BlogPosts.Remove(blog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = title.Slugify();
                if(blogId == 0)
                {
                    return await (_context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if(!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
 