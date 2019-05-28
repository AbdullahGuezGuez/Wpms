using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class UserForRegisterDto
    {
       // [Required]
        public string Username { get; set; }
        public string FullName { get; set; }

      //  [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 8 characters")]
        public string Password { get; set; }
        public string Email {get; set;}
        public string Roles {get; set;}
        public int OrganizationId { get; set; }

        public string Token { get; set; }
    }
}