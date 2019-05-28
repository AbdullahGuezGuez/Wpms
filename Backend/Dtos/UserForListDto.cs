using System;

namespace Backend.Dtos
{
public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
    
        public string[] Roles {get; set;}
    }
}