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
                    BlogPost? blog = await _context.BlogPosts!.FindAsync(blogId);
                    Tag? tag = await _context.Tags!.FindAsync(tagId);

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
                var blog = await _context.BlogPosts!.Include(t => t.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogId);

                return blog!.Tags;
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
                var blog = await _context.BlogPosts!.Include(t => t.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogId);
                List<int> tagIds = blog!.Tags.Select(b => b.Id).ToList();
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
                BlogPost? blog = await _context.BlogPosts!.FindAsync(blogId);

                return await _context.Tags!
                    .Include(b => b.BlogPosts)
                    .Where(t => t.Id == tagId && t.BlogPosts.Contains(blog!))
                    .AnyAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveBlogFromTagAsync(int tagId, int blogId)
        {
            BlogPost? blog = await _context.BlogPosts!.FindAsync(blogId);
            Tag? tag = await _context.Tags!.FindAsync(tagId);

            if (tag != null && blog != null)
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
                if (blogId == 0)
                {
                    return !(await _context.BlogPosts!.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts!.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if (!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts!.AnyAsync(b => b.Slug == newSlug));
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Categories!.Include(c => c.BlogPosts).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Tag>> GetTagsAsync(int count)
        {
            try
            {
                return await _context.Tags!.Include(c => c.BlogPosts).OrderByDescending(b => b.BlogPosts.Count).ThenBy(b => b.Name).Take(count).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            try
            {
                return await _context.BlogPosts!
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Category)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetPopularBlogPostsAsync(int count)
        {
            try
            {
                return await _context.BlogPosts!
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.Comments.Count)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetRecentBlogPostsAsync(int count)
        {
            try
            {
                return await _context.BlogPosts!
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.Created)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetBlogPostsInCategoryAsync(int id)
        {
            try
            {
                return await _context.BlogPosts!
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Category)
                    .Where(b => b.CategoryId == id)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<BlogPost> Search(string searchString)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();
                    return _context.BlogPosts!
                        .Where(b => b.IsPublished && !b.IsDeleted)
                        .Where(
                            b => b.Title!.ToLower().Contains(searchString) ||
                            b.Abstract!.ToLower().Contains(searchString) ||
                            b.Content!.ToLower().Contains(searchString) ||
                            b.Category!.Name!.ToLower().Contains(searchString) ||
                            b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)) ||
                            b.Comments.Any(
                                c => c.Body!.ToLower().Contains(searchString) ||
                                    c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                    c.Author.LastName!.ToLower().Contains(searchString))
                            )
                        .Include(b => b.Comments)
                            .ThenInclude(c => c.Author)
                        .Include(b => b.Category)
                        .Include(b => b.Tags)
                        .AsNoTracking()
                        .OrderByDescending(b => b.Created)
                        .AsEnumerable();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
