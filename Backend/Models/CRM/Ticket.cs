using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Ticket // Issue tracking system, ORGID
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public User Assignee { get; set; }
        public DateTime Issued { get; set; }
        public DateTime ConfirmedDate { get; set; } //? Nödvändigt
        public bool Confirmed { get; set; }
        public DateTime SolvedDate { get; set; }
        public bool Solved { get; set; }
        public string SolveDescription { get; set; }
        public DateTime Duedate { get; set; } //? Optional
        public TicketStatus Status { get; set; }
        public string IssuedBy { get; set; } //? Namn eller faktisk användare, eller båda
        public string Description { get; set; }
        public Severity ProblemSeverity { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<TicketComment> TicketComments { get; set; }
        public int OrganizationId { get; set; }
        public int? ProjectId { get; set; } //? Behövs den, eller collection på projekt, kanske båda
    }
    public enum TicketStatus { Issued, Confirmed, In_progress, Testing, Solved };
    public enum Severity { Low, Medium, High, Critical };
}