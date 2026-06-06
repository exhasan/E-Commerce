using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MVCProject.Dtos
{
    public class UserDto
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Phone]
        public string Mobilenumber {get;set;}
        public string ImagePath { get; set; }
    }
}
