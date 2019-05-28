namespace Backend.Dtos
{
    public class ChangePasswordDto
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
    }
}