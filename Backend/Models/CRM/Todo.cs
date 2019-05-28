using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime Duedate { get; set; }
        public int UserId { get; set; }
        public int UserCreator { get; set; }
        public bool Done { get; set; }
    }
}