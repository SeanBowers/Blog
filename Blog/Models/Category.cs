using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class Category
    {
        //Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }

        //Properties for storing image - user avatar.
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }


        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute.
        [NotMapped]
        public IFormFile? CategoryImg { get; set; }

        //Navigation Properties
        //Collection of blog posts with the ID of this category
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
    }
}