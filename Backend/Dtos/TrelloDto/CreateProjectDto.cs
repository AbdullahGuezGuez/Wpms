using System;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.Dtos.TrelloDto
{
    public class CreateProjectDto
    {
        public string TrelloBoardName {get; set;}
        public bool IncludeTrello {get; set;}
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public int ResponsibleContactpersonId { get; set; }
        public int ResponsibleUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public List<User> UsersForProject {get; set;}
        //public string Budget { get; set; }
        public int TotalBudget { get; set; }
        public string Description { get; set; }

    }
}