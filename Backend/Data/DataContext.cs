using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using System;
using Backend.Models.PMS;
using Microsoft.EntityFrameworkCore.Design;

namespace Backend.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<TrelloBoard> TrelloBoards { get; set; }
        public DbSet<TrelloList> TrelloLists { get; set; }
        public DbSet<TrelloCard> TrelloCards { get; set; }
        public DbSet<CustomFieldItem> CustomFieldItems { get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<UserCards> UserCards { get; set; }
        public DbSet<Membership> Membership { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUser { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<DbClaim> Claims { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contactperson> Contactpersons { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<NextStep> NextStep { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<ActivityUser> ActivityUsers { get; set; }
        public DbSet<NextstepUser> NextstepUsers { get; set; }
        public DbSet<ActivityContactperson> ActivityContactpersons { get; set; }
        public DbSet<NextstepContactperson> NextstepContactpersons { get; set; }
        // public DbSet<TrelloListTask> ProjectTasks { get; set; }
        

        public DbSet<Task> Tasks { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Organization>().Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Entity<Project>().Property(e => e.Id).ValueGeneratedOnAdd();


            #region enums
            builder.Entity<Customer>().Property(e => e.CustomerStatus).HasConversion(
            v => v.ToString(),
            v => (Status)Enum.Parse(typeof(Status), v)
            );

            builder.Entity<Activity>().Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (ActivityType)Enum.Parse(typeof(ActivityType), v)
            );
            builder.Entity<NextStep>().Property(e => e.Type).HasConversion(
                v => v.ToString(),
                v => (ActivityType)Enum.Parse(typeof(ActivityType), v)
            );

            builder.Entity<Ticket>().Property(e => e.ProblemSeverity).HasConversion(
            v => v.ToString(),
            v => (Severity)Enum.Parse(typeof(Severity), v)
            );
            builder.Entity<Ticket>().Property(e => e.Status).HasConversion(
            v => v.ToString(),
            v => (TicketStatus)Enum.Parse(typeof(TicketStatus), v)
            );

            #endregion

            builder.Entity<TrelloBoard>()
            .HasMany(m => m.Memberships)
            .WithOne(t => t.TrelloBoard)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCards>(userCards =>{
                userCards.HasKey(ur => new {ur.UserId, ur.TrelloCardId });
                userCards.HasOne(ur => ur.User).WithMany(r => r.UserCards).HasForeignKey(ur => ur.UserId).IsRequired();
                userCards.HasOne(ur => ur.TrelloCard).WithMany(r => r.UserCards).HasForeignKey(ur => ur.TrelloCardId).IsRequired();
            });
            
          builder.Entity<TrelloCard>().Ignore(c => c.IdMembers);

            builder.Entity<ProjectUser>(projectUser =>
            {
                projectUser.HasKey(ur => new { ur.ProjectId, ur.UserId });

                projectUser.HasOne(ur => ur.Project).WithMany(r => r.ProjectUsers).HasForeignKey(ur => ur.ProjectId).IsRequired();

                projectUser.HasOne(ur => ur.User).WithMany(r => r.ProjectUsers).HasForeignKey(ur => ur.UserId).IsRequired();
            });


            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();

                userRole.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<OrganizationUser>(organizationUser =>
            {
                organizationUser.HasKey(ur => new { ur.OrganizationId, ur.UserId });

                organizationUser.HasOne(ur => ur.User).WithMany(r => r.Organizations).HasForeignKey(ur => ur.UserId).IsRequired();

                organizationUser.HasOne(ur => ur.Organization).WithMany(r => r.OrganizationUsers).HasForeignKey(ur => ur.OrganizationId).IsRequired();
            });

            builder.Entity<ActivityUser>(activityUser =>
            {
                activityUser.HasKey(ur => new { ur.ActivityId, ur.UserId });

                activityUser.HasOne(ur => ur.User).WithMany(r => r.ActivityUsers).HasForeignKey(ur => ur.UserId).IsRequired();

                activityUser.HasOne(ur => ur.Activity).WithMany(r => r.ActivityUsers).HasForeignKey(ur => ur.ActivityId).IsRequired();
            });

            builder.Entity<NextstepUser>(nextStepUser =>
            {
                nextStepUser.HasKey(ur => new { ur.NextstepId, ur.UserId });

                nextStepUser.HasOne(ur => ur.User).WithMany(r => r.NextstepUsers).HasForeignKey(ur => ur.UserId).IsRequired();

                nextStepUser.HasOne(ur => ur.NextStep).WithMany(r => r.NextstepUsers).HasForeignKey(ur => ur.NextstepId).IsRequired();
            });

            builder.Entity<ActivityContactperson>(activityContactperson =>
            {
                activityContactperson.HasKey(ur => new { ur.ActivityId, ur.ContactpersonId });

                activityContactperson.HasOne(ur => ur.Contactperson).WithMany(r => r.ActivityContactpersons).HasForeignKey(ur => ur.ContactpersonId).IsRequired();

                activityContactperson.HasOne(ur => ur.Activity).WithMany(r => r.ActivityContactpersons).HasForeignKey(ur => ur.ActivityId).IsRequired();
            });

            builder.Entity<NextstepContactperson>(nextstepContactperson =>
            {
                nextstepContactperson.HasKey(ur => new { ur.NextstepId, ur.ContactpersonId });

                nextstepContactperson.HasOne(ur => ur.Contactperson).WithMany(r => r.NextstepContactpersons).HasForeignKey(ur => ur.ContactpersonId).IsRequired();

                nextstepContactperson.HasOne(ur => ur.NextStep).WithMany(r => r.NextstepContactpersons).HasForeignKey(ur => ur.NextstepId).IsRequired();
            });

        }

    }
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        DataContext IDesignTimeDbContextFactory<DataContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer<DataContext>("Server=tcp:wpms-db.database.windows.net,1433;Initial Catalog=WpmsDataBase;Persist Security Info=False;User ID=wpms-admin;Password=Wpasswordms2019;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new DataContext(optionsBuilder.Options);
        }
    }
}