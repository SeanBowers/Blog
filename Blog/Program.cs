using Blog.Data;
using Blog.Models;
using Blog.Services;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

//Services
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IEmailSender, EmailService>();


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//Google Auth
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? Environment.GetEnvironmentVariable("GoogleClientId");
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("GoogleClientSecret");
});

builder.Services.AddMvc();

// Swagger interface for API access
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1",
        Description = "Serving up Blog data using .Net 6",
        Contact = new OpenApiContact
        {
            Name = "S.Bowers",
            Email = "seanbowers14@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/sean-bowers/")
        }
    });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(obj =>
{
    obj.AddPolicy("DefaultPolicy",
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("DefaultPolicy");

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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublicAPI v1");
    c.InjectStylesheet("/css/swagger.css");
    c.InjectJavascript("/js/swagger.js");
    c.DocumentTitle = "Blog Public API";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Custom",
    pattern: "Content/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details" }
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AuthorPage}/{id?}");

app.MapRazorPages();

app.Run();
