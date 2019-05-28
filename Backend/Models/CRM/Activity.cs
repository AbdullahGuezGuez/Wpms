using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public DateTime Date { get; set; }
        public ActivityType Type { get; set; } 
        public bool TodoChecked { get; set; }
        public bool Archived { get; set; }
        public int? NextStepId { get; set; }
        public int CreatorId { get; set; } 
        public Customer Customer { get; set; }
        public int? CustomerId { get; set; }
        public int OrganizationId { get; set; }
        public ICollection<ActivityUser> ActivityUsers { get; set; }
        public ICollection<ActivityContactperson> ActivityContactpersons { get; set; }

    }

    public class NextStep // Nästa steg för aktiviteten
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public DateTime? Date { get; set; }
        public ActivityType? Type { get; set; }
        public int? CreatorId { get; set; } 
        public int ActivityId { get; set; }
        public ICollection<NextstepUser> NextstepUsers { get; set; }
        public ICollection<NextstepContactperson> NextstepContactpersons { get; set; }
    }
    public enum ActivityType { Meeting, Telephone, Email, ToDo };
}