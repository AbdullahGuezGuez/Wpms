using System;

namespace Backend.Dtos
{
    public class ActivityNotificationDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; } // Utfall för aktivitet
            public DateTime Date { get; set; }
            public string Type { get; set; }
            public bool Archived { get; set; }
            public string Creator { get; set; } // User that created the activity
            public bool Delayed { get; set; }
        }
}