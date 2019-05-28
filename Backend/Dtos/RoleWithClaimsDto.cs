using System.Collections.Generic;
using System.Security.Claims;

namespace Backend.Dtos
{
    public class RoleWithClaimsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OrganizationId { get; set; }
        public IList<Claim> Claims { get; set; }
        public List<ClaimDto> ClaimsWithBool { get; set; }
    }
}