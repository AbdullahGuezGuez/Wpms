namespace Backend.Dtos
{
    public class ClaimDto
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool HasClaim { get; set; }
    }
}