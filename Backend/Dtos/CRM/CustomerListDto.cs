namespace Backend.Dtos
{
    public class CustomerListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string OrganizationNumber { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }
    }
}