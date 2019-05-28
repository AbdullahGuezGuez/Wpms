using System;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Dtos
{
    public class CreateActivityDto
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public string Date { get; set; }
        public string Time { get; set; }
        public ActivityType Type { get; set; } 
        public bool Archived { get; set; }
        public int? NextStepId { get; set; }
        public int CreatorId { get; set; } 
        public int? CustomerId { get; set; }
        public List<User> UsersForActivity { get; set; }
        public List<Contactperson> ContactpersonsForActivity { get; set; }
    }
}