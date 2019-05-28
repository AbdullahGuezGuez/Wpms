using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend.Models.PMS;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
        public bool Masked { get; set; }
        public string TrelloMemberId { get; set; } 
        public string Initials { get; set; }
        public string FullName { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        public ICollection<OrganizationUser> Organizations { get; set; }
        public ICollection<UserCards> UserCards { get; set; }
        public ICollection<ActivityUser> ActivityUsers { get; set; }
        public ICollection<NextstepUser> NextstepUsers { get; set; }
        

    //    public ICollection<UserTask> UserTasks { get; set; }
    }
}