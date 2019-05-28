using System;
using System.Collections.Generic;
using Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TrelloBoardId { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        //public string Budget { get; set; }
        public int? TotalBudget { get; set; }
        public string Toggl { get; set; } //? Whut
        public string CriticalDescription { get; set; }
        public ICollection<CustomField> CustomFields { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public int CreatorId { get; set; }
        public int? CustomerId { get; set; }
        public int OrganizationId { get; set; }
        public int? ResponsibleUserId { get; set; }
        public int? ResponsibleContactpersonId { get; set; }

        //Generated Values
        public int EstimatedTime { get; set; }
        public int? Progress { get; set; } // TODO: Ta bort progress
        public int UsedTime { get; set; }
        
    }

    public enum Criticalness { Low, Medium, High }; //TODO: Fixa enum!
}