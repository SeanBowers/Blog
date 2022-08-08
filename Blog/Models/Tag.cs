using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Tag
    {
        //Primary Key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }


        //Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}