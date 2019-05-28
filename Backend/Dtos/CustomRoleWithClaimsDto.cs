using System.Collections.Generic;
using System.Security.Claims;
using Backend.Models;

namespace Backend.Dtos
{
    public class CustomRoleWithClaimsDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Claim> Claims { get; set; }
    }
}