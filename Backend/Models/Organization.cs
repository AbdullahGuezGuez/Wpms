using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.Models 
{
    public class Organization 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string TrelloTeamName { get; set; }
        public string Trellokey { get; set; }
        public string Trellotoken { get; set; }
        public ICollection<OrganizationUser> OrganizationUsers { get; set; }
        public ICollection<Project> OrganizationProjects { get; set; }
        public ICollection<TrelloBoard> OrganizationTrelloBoards { get; set; }
        public ICollection<Role> OrganizationRoles { get; set; }
        public ICollection<Customer> OrganizationCustomer { get; set; }
        public ICollection<Ticket> OrganizationTicket { get; set; }
        public ICollection<Tag> OrganizationTag { get; set; }
    }
}