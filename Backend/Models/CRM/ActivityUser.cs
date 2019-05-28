namespace Backend.Models
{
    public class ActivityUser
    {
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } 
    }
}