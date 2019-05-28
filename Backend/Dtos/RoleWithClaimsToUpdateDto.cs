using System.Collections.Generic;
using System.Security.Claims;

namespace Backend.Dtos
{
    public class RoleWithClaimsToUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ClaimDto> ClaimsWithBool { get; set; }
    }
}