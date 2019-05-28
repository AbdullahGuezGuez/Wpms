namespace Backend.Models
{
    public class NextstepUser
    {
        public int NextstepId { get; set; }
        public NextStep NextStep { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } 
    }
}