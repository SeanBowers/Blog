using Blog.Data;
using Blog.Models;
using Blog.Services;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

// Add services to the container.
//Define a db connection - DefaultConnection defined in app secrets. Pulled from DataUtility helper.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

//Establish the connection to db - useNpgsql since we are using postgres
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString,
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));

//Push errors to browser.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Change default identity to our new BlogUser
builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddMvc();

var app = builder.Build();

var scope = app.Services.CreateScope();
await DataUtility.SeedDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Custom",
    pattern: "Content/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details"}
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AuthorPage}/{id?}");

app.MapRazorPages();

app.Run();
