using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Attachments { get; set; } //? Nödvändig, List?
    }
}