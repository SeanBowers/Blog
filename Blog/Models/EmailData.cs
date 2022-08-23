using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class EmailData
    {
        [Required]
        public string Subject { get; set; } = "";
        [Required]
        public string Body { get; set; } = "";


        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
    }
}
