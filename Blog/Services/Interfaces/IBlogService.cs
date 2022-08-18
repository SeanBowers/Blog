using Blog.Models;
namespace Blog.Services.Interfaces
{
    public interface IBlogService
    {
        public Task AddBlogToTagAsync(int categoryId, int contactId);

        public Task<IEnumerable<Tag>> GetBlogTagsAsync(int contactId);

        public Task RemoveBlogFromTagAsync(int categoryId, int contactId);

        public Task <IEnumerable<int>> GetBlogTagIdsAsync(int contactId);

        public Task<bool> ValidateSlugAsync(string title, int blogId);

        public Task<List<Category>> GetCategoriesAsync();
        public Task<List<Tag>> GetTagsAsync(int count);
        public Task<List<BlogPost>> GetAllBlogPostsAsync(); //All posts regardless of deleted/published
        public Task<List<BlogPost>> GetPopularBlogPostsAsync(int count); // Defined by number of comments
        public Task<List<BlogPost>> GetRecentBlogPostsAsync(int count); // Defined by date created
        public Task<List<BlogPost>> GetBlogPostsInCategoryAsync(int id); // Defined by date created
        public IEnumerable<BlogPost> Search(string searchString);
    }
}
