using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public class Customer // ORGID
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerDescription { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Customermail { get; set; }
        public string Attitude { get; set; }
        public string OrganizationNumber { get; set; }
        public DateTime FirstContacted { get; set; }
        public Status CustomerStatus { get; set; }
        public ICollection<Contactperson> Contacts { get; set; }
        public ICollection<Project> CustomerProjects { get; set; } //* Koppla många till en sammanband mellan projects och customer */
        public ICollection<CustomField> CustomFields { get; set; }
        public int OrganizationId { get; set; }
                  
    }
    public enum Status { Suspect, Prospect, Customer, Inactive };
}