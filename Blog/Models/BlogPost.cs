using Blog.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class BlogPost
    {
        //Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        //Foreign Key
        public int CategoryId { get; set; }

        public BlogPostStatus BlogPostStatus { get; set; }

        //Slug to mask the url
        public string? Slug { get; set; }

        //Short description of blog post
        public string? Abstract { get; set; }

        public bool IsDeleted { get; set; }

        //Properties for storing image - user avatar.
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }


        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute.
        [NotMapped]
        public IFormFile? BlogPostImg { get; set; }

        //Navigation Properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set;} = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    }
}
