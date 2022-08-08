using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Comment
    {
        //Primary Key
        public int Id { get; set; }
        
        //Foreign Key
        public int BlogPostId { get; set; }

        [Required]
        public string? AuthorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }

        public string? UpdateReason { get; set; }

        [Required]
        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Body { get; set; }


        //Navigation Property
        public virtual BlogPost? BlogPost { get; set; }
        public virtual BlogUser? Author { get; set; }
    }
}