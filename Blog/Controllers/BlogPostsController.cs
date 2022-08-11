using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Blog.Extensions;

namespace Blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IBlogService _blogService;

        public BlogPostsController(ApplicationDbContext context,
            IImageService imageService,
            UserManager<BlogUser> userManager,
            IBlogService blogService)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _blogService = blogService;
        }

        // GET: BlogPosts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<BlogPost> blogPosts = new List<BlogPost>();

            blogPosts = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Tags)
                .ToListAsync();

            return View(blogPosts);

        }

        // GET: BlogPosts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .ThenInclude(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagId"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CategoryId,Abstract,IsPublished,BlogPostImg")] BlogPost blogPost, List<int> TagId)
        {
            if (ModelState.IsValid)
            {
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.Now);

                if(!await _blogService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or Slug has already been used!");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                    ViewData["TagId"] = new MultiSelectList(_context.Tags, "Id", "Name");
                    return View();
                }

                blogPost.Slug = blogPost.Title!.Slugify();

                if (blogPost.BlogPostImg != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImg);
                    blogPost.ImageType = blogPost.BlogPostImg.ContentType;
                }

                _context.Add(blogPost);

                //foreach (int tagId in TagId)
                //{
                //    blogPost.Tags.Add(_context.Tags.Find(tagId)!);
                //}

                await _context.SaveChangesAsync();

                foreach (int tagId in TagId)
                {
                    await _blogService.AddBlogToTagAsync(tagId, blogPost.Id);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: BlogPosts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            //var blogPost = await _context.BlogPosts.FindAsync(id);
            var blogPost = await _context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagId"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags.Select(t => t.Id));

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Created,Content,CategoryId,Abstract,IsDeleted,IsPublished,ImageData,ImageType,BlogPostImg")] BlogPost blogPost, List<int> TagId)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.Now);
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);

                    if (blogPost.BlogPostImg != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImg);
                        blogPost.ImageType = blogPost.BlogPostImg.ContentType;
                    }

                    _context.Update(blogPost);

                    List<Tag> oldTags = (await _blogService.GetBlogTagsAsync(blogPost.Id)).ToList();
                    foreach (Tag tag in oldTags)
                    {
                        await _blogService.RemoveBlogFromTagAsync(tag.Id, blogPost.Id);
                    }

                    foreach (int tagId in TagId)
                    {
                        await _blogService.AddBlogToTagAsync(tagId, blogPost.Id);
                    }
                    //blogPost.Tags.Clear();

                    //List<Tag> oldTags = _context.Tags!.Where(t => t.BlogPosts.Contains(blogPost)).ToList();
                    //foreach (Tag tag in oldTags)
                    //{
                    //    blogPost.Tags.Remove(tag);
                    //}
                    //foreach (int tag in TagId)
                    //{
                    //    blogPost.Tags.Add(_context.Tags!.Find(tag)!);
                    //}

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
