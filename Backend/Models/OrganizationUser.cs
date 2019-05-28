namespace Backend.Models
{
    public class OrganizationUser
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}