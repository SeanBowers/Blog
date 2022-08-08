
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    //Inherit from IdentityUser because we are adding more properties to our default users.
    public class BlogUser : IdentityUser
    {
        //Add First & Last name to the default IdentityUser.
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }


        //Properties for storing image - user avatar.
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }


        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute.
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        //Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
