using System;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.Dtos
{
    public class UserWithRolesDto
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public List<Role> Roles {get; set;}
        public string Initials { get; set; }
        public string FullName { get; set; }
    }
}