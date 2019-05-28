using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Contactperson //? Attitude på contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string Telephone { get; set; } //? Flera telefonnummer
        public string SecTelephone { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public bool Responsible { get; set; }
        public int CustomerId { get; set; }
        public bool Masked { get; set; }
        public ICollection<NextstepContactperson> NextstepContactpersons { get; set; }
        public ICollection<ActivityContactperson> ActivityContactpersons { get; set; }
    }
}