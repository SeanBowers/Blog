using Blog.Data;
using Blog.Models;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<BlogUser> userManager, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<IActionResult> AuthorPage(int? page)
        {
            //TODO: Create Service to get blogposts
            var blogPosts = await _context.BlogPosts!
                .Include(b => b.Category)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .ThenInclude(b => b.Author)
                .OrderByDescending(b => b.Created)
                .ToListAsync();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var onePageOfBlogPosts = blogPosts.ToPagedList(pageNumber, 5); // will only contain 25 products max because of the pageSize

            ViewBag.OnePageOfBlogPosts = onePageOfBlogPosts;

            return View(blogPosts);
        }
        public IActionResult Contact(string swalMessage = null!)
        {
            ViewData["SwalMessage"] = swalMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(EmailData emailData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BlogUser blogUser = await _userManager.GetUserAsync(User);
                    var email = blogUser.UserName;
                    var firstName = blogUser.FirstName;
                    var lastName = blogUser.LastName;

                    var adminEmail = "theseanbowers14@gmail.com";

                    emailData.Body += firstName + lastName + email;
                    
                    await _emailService.SendAdminEmailAsync(adminEmail, emailData.Subject, emailData.Body);
                    return RedirectToAction("Contact", "Home", new { swalMessage = "Email Sent!" });
                }
                catch
                {
                    return RedirectToAction("Contact", "Home", new { swalMessage = "Error: Email Send Failed!" });
                    throw;
                }
            }
            return View(emailData);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}