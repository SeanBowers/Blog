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


    }
}
