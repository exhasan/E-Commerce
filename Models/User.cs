using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
    public class User
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Key]
        public int Id { get; set; }
        public string Password { get; set; }
        public string ImagePath { get; set; }
        [Phone]
        public string Mobilenumber { get; set; }
    }
}
