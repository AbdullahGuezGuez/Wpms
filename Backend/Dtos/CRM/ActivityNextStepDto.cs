
using System;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Dtos
{
    public class ActivityNextStepDto
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } // Utfall för aktivitet
        public DateTime Date { get; set; }
        public string Type { get; set; } 
        public bool Archived { get; set; }
        public bool TodoChecked { get; set; }
        public List<Contactperson> CustomerParticipiants { get; set; }
        public List<User> BusinessParticipants { get; set; }
        public string Creator { get; set; } // User that created the activity

        //Next Step
        public int NextStepId { get; set; }
        public string NextStepTitle { get; set; }
        public string NextStepDescription { get; set; } 
        public DateTime? NextStepDate { get; set; }
        public string NextStepType { get; set; }
        public List<Contactperson> NextStepCustomerParticipiants { get; set; }
        public List<User> NextStepBusinessParticipants { get; set; }
        public string NextStepCreator { get; set; } // User that created the activity
    }

    public class Senduser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
    }
}