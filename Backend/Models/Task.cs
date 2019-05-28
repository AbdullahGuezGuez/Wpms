using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Task
    {
        public int Id {get; set;}
    public string Name { get; set; }

    public string  TimeEstimate { get; set; }
    public string Description { get; set; }
    public int AccomplishedTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime FinishedDate { get; set; }
    public string Responsible { get; set; } 
    public string Status { get; set; }  
    public Priority Priority { get; set; }

    // public ICollection<TrelloListTask> TrellolistTasks { get; set; }
    // public ICollection<UserTask> UserTasks { get; set; }
    }

    public enum Priority{
        Low, Medium, High
    }
}