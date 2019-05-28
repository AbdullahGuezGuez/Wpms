using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class CustomField 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public int CustomerId { get; set; }
    }
}