using System;
using System.Collections.Generic;
using Backend.Models;


namespace Backend.Dtos.TrelloDto
{
    public class ProjectListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrelloUrl { get; set; }
        public string TogglUrl { get; set; }
        public int Priority { get; set; }   
        public int EstimatedTime { get; set; }
        public int UsedTime { get; set; }
        public string CustomerName { get; set; }
        public string AllMembers { get; set; }
        public int? TotalBudget { get; set; }
        public int EstimatedTimeOver { get; set; }

      
    }
}