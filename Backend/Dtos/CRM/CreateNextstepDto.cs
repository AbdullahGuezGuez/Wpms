using System.Collections.Generic;
using Backend.Models;

namespace Backend.Dtos
{
    public class CreateNextstepDto
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public string Date { get; set; }
        public string Time { get; set; }
        public ActivityType Type { get; set; } 
        public int? ActivityId { get; set; }
        public int CreatorId { get; set; } 
        public List<User> UsersForActivity { get; set; }
        public List<Contactperson> ContactpersonsForActivity { get; set; }
    }
}