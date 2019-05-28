using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Tag //OrgId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; } //? Färgkod på ticket
    }
}