using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Backend.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly DataContext _dataContext;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var organizations = new List<Organization>
                {
                    new Organization{Id = 1, Name = "SUP-Gruppen", Trellokey = "8647cda40947c5f59daaa1c3f5173a1a", Trellotoken = "5e73a3d20653d1e9f97812fa1572a61499b84ffd6954f1b33f4f93d69fd0fdff", TrelloTeamName = "wpms2"},
                    new Organization{Id = 2, Name = "Wiccon", Trellokey = "9dc5e140e89bd1ff02b261a4ebeaf519", Trellotoken = "6469b110f8d594da9783fed645a2a3753ee6920dcfe0c40ef38e61bdaf8454c1", TrelloTeamName = "wiccon2"},
                    new Organization{Id = 3, Name = "Bolaget", Trellokey = "6f43a3cb27280f6f0096e0261de07c68", Trellotoken = "3502b598cea60458ba4edff6248e822eb349495d435354bff9611ee63564546f", TrelloTeamName = "bolaget6"},
                };
                
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach(var org in organizations)
                {
                    _dataContext.Organizations.AddAsync(org).Wait();
                }
                
                
                foreach (var organization in organizations)
                {
                    var roles = new List<Role>
                    {
                        new Role{Name = "Member"},
                        new Role{Name = "PMS-Admin"},
                        new Role{Name = "CRM-Admin"},
                    };

                   foreach (var role in roles)
                    {
                        if(role.Name == "Member")
                        {
                            role.Name = organization.Name +"-Member";
                            role.NormalizedName = organization.Name +"-Member";
                        }
                        if(role.Name == "PMS-Admin")
                        {
                            role.Name = organization.Name +"-PMS-Admin";
                            role.NormalizedName = organization.Name +"-PMS-Admin";
                        }
                        if(role.Name == "CRM-Admin")
                        {
                            role.Name = organization.Name +"-CRM-Admin";
                            role.NormalizedName = organization.Name +"-CRM-Admin";
                        }
                        
                        role.OrganizationId = organization.Id;
                        _roleManager.CreateAsync(role).Wait();

                        //! ALLAS CLAIMS
                        _roleManager.AddClaimAsync(role, new Claim("R-Project", "See All Projects"));
                        _roleManager.AddClaimAsync(role, new Claim("R-AssignedProject", "See assigned Users in projects"));
                        _roleManager.AddClaimAsync(role, new Claim("R-Customer", "See All Customers"));
                        _roleManager.AddClaimAsync(role, new Claim("R-Contactperson", "See All Contactpersons"));
                        _roleManager.AddClaimAsync(role, new Claim("R-Activity", "See All Activities"));
                        _roleManager.AddClaimAsync(role, new Claim("CU-Activity", "Create/Update Activities"));

                        if(role.Name.Contains("PMS"))
                        {
                            //! BARA PMS-ADMIN CLAIMS
                            _roleManager.AddClaimAsync(role, new Claim("CU-Project", "Create/Update Projects"));
                            _roleManager.AddClaimAsync(role, new Claim("C-User", "Create Users"));
                            _roleManager.AddClaimAsync(role, new Claim("C-Admin","Create Admins"));
                            _roleManager.AddClaimAsync(role, new Claim("D-User", "Delete Users"));
                            _roleManager.AddClaimAsync(role, new Claim("U-User", "Update Users"));
                            _roleManager.AddClaimAsync(role, new Claim("CRUD-Role", "Create/Update/Delete Roles"));
                            _roleManager.AddClaimAsync(role, new Claim("RU-UserToRole", "Change Roles on Users"));
                            _roleManager.AddClaimAsync(role, new Claim("U-AssingedMembers", "Assign and Remove Users from projects"));
                        }
                        if(role.Name.Contains("CRM"))
                        {
                            //! BARA CRM-ADMIN CLAIMS
                            _roleManager.AddClaimAsync(role, new Claim("CU-Project", "Create/Update Projects"));
                            _roleManager.AddClaimAsync(role, new Claim("C-User", "Create Users"));
                            _roleManager.AddClaimAsync(role, new Claim("C-Admin","Create Admins"));
                            _roleManager.AddClaimAsync(role, new Claim("D-User", "Delete Users"));
                            _roleManager.AddClaimAsync(role, new Claim("U-User", "Update Users"));
                            _roleManager.AddClaimAsync(role, new Claim("CRUD-Role", "Create/Update/Delete Roles"));
                            _roleManager.AddClaimAsync(role, new Claim("RU-UserToRole", "Change Roles on Users"));
                            _roleManager.AddClaimAsync(role, new Claim("CUD-Customer", "Create/Update/Delete Customers"));
                            _roleManager.AddClaimAsync(role, new Claim("CUD-Contactperson", "Create/Update/Delete Contactpersons"));
                            _roleManager.AddClaimAsync(role, new Claim("U-AssingedMembers", "Assign and Remove Users from projects"));
                            _roleManager.AddClaimAsync(role, new Claim("D-Activity", "Delete Activities"));
                        }
                    } 
                }
                

                var systemAdmin = new Role{Name = "SystemAdmin"};
                _roleManager.CreateAsync(systemAdmin).Wait();
                _roleManager.AddClaimAsync(systemAdmin, new Claim("CRUD-Role", "Create/Update/Delete Roles"));
                _roleManager.AddClaimAsync(systemAdmin, new Claim("C-User", "Create Users"));
                _roleManager.AddClaimAsync(systemAdmin, new Claim("C-Admin","Create Admins"));
                _roleManager.AddClaimAsync(systemAdmin, new Claim("RU-UserToRole", "Change Roles on Users"));

                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "password").Wait();

                    
                    if(user.Id == 7)
                    {
                        _userManager.AddToRoleAsync(user, "SUP-Gruppen-CRM-Admin").Wait();
                    }
                    else if(user.Id == 13)
                    {
                        _userManager.AddToRoleAsync(user, "SUP-Gruppen-PMS-Admin").Wait();
                    }
                    else if(user.Id == 2 || user.Id == 3)
                    {
                        _userManager.AddToRoleAsync(user, "Wiccon-Member").Wait();
                    }
                    else if(user.Id == 6)
                    {
                        _userManager.AddToRoleAsync(user, "SUP-Gruppen-Member").Wait();
                        _userManager.AddToRoleAsync(user, "Wiccon-CRM-Admin").Wait();
                        _userManager.AddToRoleAsync(user, "Bolaget-CRM-Admin").Wait();
                    }
                    else if(user.Id == 8)
                    {
                        _userManager.AddToRoleAsync(user, "Bolaget-CRM-Admin").Wait();
                    }
                    else if(user.Id == 10 || user.Id == 5)
                    {
                        _userManager.AddToRoleAsync(user, "Bolaget-Member").Wait();
                    }
                    else
                    {
                        _userManager.AddToRoleAsync(user, "SUP-Gruppen-Member").Wait();
                    }
                }

                var adminUser = new User{UserName = "SystemAdmin"};

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("SystemAdmin").Result;
                    _userManager.AddToRoleAsync(admin, "SystemAdmin").Wait();
                }

                var projects = new List<Project> //TODO: Fixa seedern för projekt
                {
                    new Project{Id = 1, Name = "WPMS", OrganizationId = organizations[0].Id, Priority = 1, Active = true, TotalBudget = 1500, CustomerId = 1, Progress = 93, UsedTime = 50, EstimatedTime = 200, TrelloBoardId ="5c519ac39453e81aa659c0af", CreatorId = 13, ResponsibleUserId = 11, StartDate = new DateTime(2019, 1, 15), EndDate = new DateTime(2019, 6, 1), Description = "Systemutvecklings projekt där vi ska göra ett PMS / CRM system"},
                    new Project{Id = 2, Name = "Appen B", OrganizationId = organizations[0].Id, Priority = 2, Active = true, TotalBudget = 140, CustomerId = 1, Progress = 29, UsedTime = 100, EstimatedTime = 250, TrelloBoardId ="5ceba33b81c94254fe09af2e", StartDate = new DateTime(2018, 2, 15), EndDate = new DateTime(2019, 6, 1)},
                    new Project{Id = 3, Name = "Appen C", OrganizationId = organizations[0].Id, Priority = 3, Active = true, TotalBudget = 250, CustomerId = 2, Progress = 58, UsedTime = 210, EstimatedTime = 300, TrelloBoardId ="5ceba40e48efb24c5db81ee2", StartDate = new DateTime(2012, 7, 28), EndDate = new DateTime(2019, 6, 1)},
                    new Project{Id = 4, Name = "Appen D", OrganizationId = organizations[0].Id, Priority = 4, Active = true, TotalBudget = 200, CustomerId = 2, Progress = 12, UsedTime = 10, EstimatedTime = 290, TrelloBoardId ="5ceba4783509f47ee51e96d8", StartDate = new DateTime(2015, 2, 27), EndDate = new DateTime(2019, 6, 1)},
                    new Project{Id = 11, Name = "Wikipedia", OrganizationId = organizations[0].Id, Priority = 5, Active = false, TotalBudget = 300, CustomerId = 3, Progress = 58, UsedTime = 210, EstimatedTime = 300, StartDate = new DateTime(2016, 4, 25), EndDate = new DateTime(2017, 6, 1)},
                    new Project{Id = 12, Name = "Blocket", OrganizationId = organizations[0].Id, Priority = 6, Active = false, TotalBudget = 300, CustomerId = 3, Progress = 12, UsedTime = 10, EstimatedTime = 290, StartDate = new DateTime(2016, 1, 15), EndDate = new DateTime(2017, 6, 1)},

                    new Project{Id = 5, Name = "Hemsidan A", OrganizationId = organizations[1].Id, Active = true, Priority = 1, UsedTime = 50, EstimatedTime = 200},
                    new Project{Id = 6, Name = "Hemsidan B", OrganizationId = organizations[1].Id, Active = true, Priority = 2, UsedTime = 10, EstimatedTime = 200},
                    new Project{Id = 7, Name = "Hemsidan C", OrganizationId = organizations[1].Id, Active = true, Priority = 3, UsedTime = 190, EstimatedTime = 200},

                    new Project{Id = 8, Name = "Projektet A", OrganizationId = organizations[2].Id, Active = true, Priority = 1, UsedTime = 10, EstimatedTime = 200},
                    new Project{Id = 9, Name = "Projektet B", OrganizationId = organizations[2].Id, Active = true, Priority = 2, UsedTime = 20, EstimatedTime = 200},
                    new Project{Id = 10, Name = "Projektet C", OrganizationId = organizations[2].Id, Active = true, Priority = 3, UsedTime = 180, EstimatedTime = 200},
                };

                foreach (var project in projects)
                {
                    _dataContext.Projects.AddAsync(project).Wait();
                }

                var projectUser = new List<ProjectUser>
                {
                    new ProjectUser{Project = projects[0], User = _dataContext.Users.Single(x => x.Id == 11)},
                    new ProjectUser{Project = projects[1], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ProjectUser{Project = projects[2], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ProjectUser{Project = projects[3], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ProjectUser{Project = projects[5], User = _dataContext.Users.Single(x => x.Id == 3)},
                    new ProjectUser{Project = projects[6], User = _dataContext.Users.Single(x => x.Id == 3)},
                    new ProjectUser{Project = projects[4], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new ProjectUser{Project = projects[0], User = _dataContext.Users.Single(x => x.Id == 12)},
                    new ProjectUser{Project = projects[1], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new ProjectUser{Project = projects[3], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new ProjectUser{Project = projects[5], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new ProjectUser{Project = projects[0], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ProjectUser{Project = projects[0], User = _dataContext.Users.Single(x => x.Id == 14)},
                    new ProjectUser{Project = projects[7], User = _dataContext.Users.Single(x => x.Id == 8)},
                    new ProjectUser{Project = projects[8], User = _dataContext.Users.Single(x => x.Id == 8)},
                    new ProjectUser{Project = projects[9], User = _dataContext.Users.Single(x => x.Id == 8)},
                };

                foreach (var pu in projectUser)
                {
                    _dataContext.ProjectUsers.AddAsync(pu).Wait();
                }

                var organizationUser = new List<OrganizationUser>
                {
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 1)},// SUP-Gruppen
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 4)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 9)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 11)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 12)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new OrganizationUser{Organization = organizations[0], User = _dataContext.Users.Single(x => x.Id == 14)},


                    new OrganizationUser{Organization = organizations[1], User = _dataContext.Users.Single(x => x.Id == 2)},// Wiccon
                    new OrganizationUser{Organization = organizations[1], User = _dataContext.Users.Single(x => x.Id == 3)},
                    new OrganizationUser{Organization = organizations[1], User = _dataContext.Users.Single(x => x.Id == 6)},


                    new OrganizationUser{Organization = organizations[2], User = _dataContext.Users.Single(x => x.Id == 10)},// Bolaget
                    new OrganizationUser{Organization = organizations[2], User = _dataContext.Users.Single(x => x.Id == 5)},
                    new OrganizationUser{Organization = organizations[2], User = _dataContext.Users.Single(x => x.Id == 6)},
                    new OrganizationUser{Organization = organizations[2], User = _dataContext.Users.Single(x => x.Id == 8)},
                };

                 foreach (var ou in organizationUser)
                {
                    _dataContext.OrganizationUser.AddAsync(ou).Wait();
                }

                var dbClaims = new List<DbClaim>
                {
                    new DbClaim{Id = 1, ClaimType = "R-Project", ClaimValue = "See All Projects"},
                    new DbClaim{Id = 2, ClaimType = "U-AssingedMembers", ClaimValue = "Assign and Remove Users from projects"},
                    new DbClaim{Id = 3, ClaimType = "CU-Project", ClaimValue = "Create/Update Projects"},
                    new DbClaim{Id = 4, ClaimType = "C-User", ClaimValue = "Create Users"},
                    new DbClaim{Id = 5, ClaimType = "C-Admin", ClaimValue = "Create Admins"},
                    new DbClaim{Id = 6, ClaimType = "D-User", ClaimValue = "Delete Users"},
                    new DbClaim{Id = 7, ClaimType = "U-User", ClaimValue = "Update Users"},
                    new DbClaim{Id = 8, ClaimType = "CRUD-Role", ClaimValue = "Create/Update/Delete Roles"},
                    new DbClaim{Id = 9, ClaimType = "RU-UserToRole", ClaimValue = "Change Roles on Users"},
                    new DbClaim{Id = 10, ClaimType = "R-AssignedProject", ClaimValue = "See assigned Users in projects"},
                    new DbClaim{Id = 11, ClaimType = "CUD-Customer", ClaimValue = "Create/Update/Delete Customers"},
                    new DbClaim{Id = 12, ClaimType = "R-Customer", ClaimValue = "See All Customers"},
                    new DbClaim{Id = 13, ClaimType = "CUD-Contactperson", ClaimValue = "Create/Update/Delete Contactpersons"},
                    new DbClaim{Id = 14, ClaimType = "R-Contactperson", ClaimValue = "See All Contactpersons"},
                    new DbClaim{Id = 15, ClaimType = "CU-Activity", ClaimValue = "Create/Update Activities"},
                    new DbClaim{Id = 16, ClaimType = "R-Activity", ClaimValue = "See All Activities"},
                    new DbClaim{Id = 17, ClaimType = "D-Activity", ClaimValue = "Delete Activities"},
                };

                foreach (var dbClaim in dbClaims)
                {
                    _dataContext.Claims.AddAsync(dbClaim).Wait();
                }

                //_________________________CRM-SEED__________________________


                var customers = new List<Customer>
                {
                    new Customer{Id = 1, Name = "Bolaget B", Region = "Örebro", Customermail = "bolaget@mail.com", OrganizationId = 1, CustomerStatus = Status.Customer, Telephone = "070-32488873", Address = "Storgatan 94", OrganizationNumber = "1337-8948394", FirstContacted = new DateTime(2008, 5, 1, 8, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                    new Customer{Id = 2, Name = "Företaget A", Region = "Borås", Customermail = "foretaget@mail.com", OrganizationId = 1, CustomerStatus = Status.Customer, OrganizationNumber = "1227-8111114", Address = "Storgatan 4", FirstContacted = new DateTime(2004, 2, 1, 11, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                    new Customer{Id = 3, Name = "Koncernen C", Region = "Luleå", Customermail = "konc@mail.com", OrganizationId = 1, CustomerStatus = Status.Prospect, OrganizationNumber = "1337-8214594", Address = "Storgatan 9", FirstContacted = new DateTime(2005, 5, 1, 14, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                    new Customer{Id = 4, Name = "Monopolet D", Region = "Stockholm", Customermail = "mono@mail.com", OrganizationId = 1, CustomerStatus = Status.Prospect, OrganizationNumber = "1337-2214594", Address = "Storgatan 99", FirstContacted = new DateTime(2018, 3, 1, 8, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                    new Customer{Id = 5, Name = "Oligopolet E", Region = "Kungsör", Customermail = "oligo@mail.com", OrganizationId = 1, CustomerStatus = Status.Suspect, OrganizationNumber = "1337-1243554", Address = "Storgatan 88", FirstContacted = new DateTime(2012, 5, 2, 8, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                    new Customer{Id = 6, Name = "Privatpersonen F", Region = "Rotebro", Customermail = "thatguy@mail.com", OrganizationId = 1, CustomerStatus = Status.Inactive, OrganizationNumber = "1337-8234594", Address = "Storgatan 66", FirstContacted = new DateTime(2010, 2, 1, 8, 30, 52), CustomerDescription = "Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type.Deo,has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type."},
                };

                foreach (var customer in customers)
                {
                    _dataContext.Customers.AddAsync(customer).Wait();
                }

                var contacts = new List<Contactperson>
                {
                    new Contactperson{Id = 1, Name = "Jesus Kristersson",Responsible = true ,Initials = "J.K", Telephone = "070-18376222", Mail = "jes@mail.com", CustomerId = 1, Role = "VD"},
                    new Contactperson{Id = 2, Name = "Moses Exempelsson",Responsible = false , Initials = "M.E", Telephone = "071-84389233", Mail = "mos@mail.com", CustomerId = 1, Role = "Religös Ledare"},
                    new Contactperson{Id = 3, Name = "Rogga Svensson",Responsible = false , Initials = "R.S", Telephone = "072-423566111", Mail = "rogge@mail.com", CustomerId = 1, Role = "Ekomomichef"},
                    new Contactperson{Id = 4, Name = "Manfred Ek",Responsible = true , Initials = "M.E", Telephone = "073-99932001", Mail = "manyman@mail.com", CustomerId = 2, Role = "VD"},
                    new Contactperson{Id = 5, Name = "Gudrun Johansen",Responsible = false , Initials = "G.J", Telephone = "074-11730356", Mail = "gud@mail.com", CustomerId = 3, Role = "Sanitetstekniker"},
                    new Contactperson{Id = 6, Name = "Mike Smith",Responsible = true , Initials = "M.S", Telephone = "075-22505191", SecTelephone = "079-22504491", Mail = "thisguy@mail.com", CustomerId = 3, Role = "Platschef"},
                    new Contactperson{Id = 7, Name = "Tobias Kristersson",Responsible = false , Initials = "T.K", Telephone = "070-17776222", Mail = "tobbe@mail.com", CustomerId = 4, Role = "VD"},
                    new Contactperson{Id = 8, Name = "Kalle Exempelsson",Responsible = true , Initials = "J.E", Telephone = "071-84385233", Mail = "kalle@mail.com", CustomerId = 4, Role = "Ekonomichef"},
                    new Contactperson{Id = 9, Name = "Eva Svensson",Responsible = true , Initials = "E.S", Telephone = "072-423523111", Mail = "eva@mail.com", CustomerId = 5, Role = "Säljare"},
                    new Contactperson{Id = 10, Name = "Manfred Bok",Responsible = false , Initials = "M.B", Telephone = "073-99932441", Mail = "boken@mail.com", CustomerId = 5, Role = "Säljare"},
                    new Contactperson{Id = 11, Name = "Bakayoko Bah",Responsible = false , Initials = "B.B", Telephone = "074-18734356", Mail = "bakabakabaka@mail.com", CustomerId = 5, Role = "Grundare"},
                    new Contactperson{Id = 12, Name = "Ricky Von Schak",Responsible = false , Initials = "R.V.S", Telephone = "075-26555191", SecTelephone = "078-22533491", Mail = "someguy@mail.com", CustomerId = 5, Role = "VD"},
                    new Contactperson{Id = 13, Name = "Bobby Kanel",Responsible = true , Initials = "B.K", Telephone = "075-22533191", Mail = "thatguy@mail.com", CustomerId = 6, Role = "Arbetslös"},
                };

                foreach (var contact in contacts)
                {
                    _dataContext.Contactpersons.AddAsync(contact).Wait();
                }

                var activities = new List<Activity>
                {
                    new Activity{Id = 1, Title = "Intresse", Description = "Företaget vill ha ett CRM system", Date = new DateTime(2019, 4, 20, 8, 30, 52), Type = ActivityType.Email, NextStepId = 1, CustomerId = 1, CreatorId = 7, OrganizationId = 1},
                    new Activity{Id = 2, Title = "Lunchmöte", Description = "Dem har beslutat att ett CRM/PMS system vore bäst", Date = new DateTime(2019, 5, 1, 13, 30, 52), Type = ActivityType.Meeting, NextStepId = 2, CustomerId = 1, CreatorId = 13, OrganizationId = 1},
                    new Activity{Id = 3, Title = "Uppföljning", Description = "Nånting vart beslutat och klart", Date = new DateTime(2019, 5, 3, 16, 00, 52), Type = ActivityType.Email, NextStepId = 3, CustomerId = 1, CreatorId = 7, OrganizationId = 1},
                    new Activity{Id = 4, Title = "Intresse", Description = "Samtal som resulterade i nånting som är till intresse för någon av någon anledning", Date = new DateTime(2018, 11, 23, 8, 30, 52), Type = ActivityType.Telephone, NextStepId = 4, CustomerId = 2, CreatorId = 12, OrganizationId = 1},
                    new Activity{Id = 5, Title = "Morgonmöte", Description = "Möte som drog ut på tiden och inget speciellt vart sagt", Date = new DateTime(2019, 1, 4, 8, 45, 52), Type = ActivityType.Meeting, NextStepId = 5, CustomerId = 2, CreatorId = 13, OrganizationId = 1},
                    new Activity{Id = 6, Title = "Beslut", Description = "Det vart beslutat att nånting ska göras åt nånting", Date = new DateTime(2019, 2, 3, 18, 00, 52), Type = ActivityType.Email, NextStepId = 6, CustomerId = 2, CreatorId = 14, OrganizationId = 1},
                };

                foreach (var aktivity in activities)
                {
                    _dataContext.Activities.AddAsync(aktivity).Wait();
                }

                var nextSteps = new List<NextStep>
                {
                    new NextStep{Id = 1, Title = "Lunchmöte", Description = "Möta dem för att stämma av allt", Date =  new DateTime(2019, 5, 1, 13, 30, 52), Type = ActivityType.Email, ActivityId = 1, CreatorId = 7},
                    new NextStep{Id = 2, Title = "Uppföljning", Description = "Kolla så vi har ett klartecken för att dra igång", Date = new DateTime(2019, 5, 3, 16, 00, 52), Type = ActivityType.Meeting, ActivityId = 2, CreatorId = 13},
                    new NextStep{Id = 3, Title = "Uppföljning", Description = "Nånting ska vara klart", Date = new DateTime(2019, 5, 25, 16, 00, 52), Type = ActivityType.Email, ActivityId = 3, CreatorId = 7},
                    new NextStep{Id = 4, Title = "Morgonmöte", Description = "Möta dem för att stämma av allt", Date = new DateTime(2018, 11, 23, 8, 30, 52), Type = ActivityType.Telephone, ActivityId = 4, CreatorId = 12},
                    new NextStep{Id = 5, Title = "Uppföljning", Description = "Kolla vad som riktigt blev sagt för vi förstod ingenting", Date = new DateTime(2019, 1, 4, 8, 45, 52), Type = ActivityType.Meeting, ActivityId = 5, CreatorId = 13},
                    new NextStep{Id = 6, Title = "Uppföljning", Description = "Kolla så det är värkligen det dem vill ha för det var väldigt luddigt", Date = new DateTime(2019, 2, 3, 18, 00, 52), Type = ActivityType.Email, ActivityId = 6, CreatorId = 14},
                };

                foreach (var nextStep in nextSteps)
                {
                    _dataContext.NextStep.AddAsync(nextStep).Wait();
                }

                var actUser = new List<ActivityUser>
                {
                    new ActivityUser{Activity = activities[0], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new ActivityUser{Activity = activities[1], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new ActivityUser{Activity = activities[1], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ActivityUser{Activity = activities[2], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new ActivityUser{Activity = activities[3], User = _dataContext.Users.Single(x => x.Id == 12)},
                    new ActivityUser{Activity = activities[4], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new ActivityUser{Activity = activities[4], User = _dataContext.Users.Single(x => x.Id == 12)},
                    new ActivityUser{Activity = activities[4], User = _dataContext.Users.Single(x => x.Id == 14)},
                    new ActivityUser{Activity = activities[4], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new ActivityUser{Activity = activities[5], User = _dataContext.Users.Single(x => x.Id == 14)},
                };

                foreach (var au in actUser)
                {
                    _dataContext.ActivityUsers.AddAsync(au).Wait();
                }

                var actContacts = new List<ActivityContactperson>
                {
                    new ActivityContactperson{Activity = activities[0], Contactperson = contacts[0]},
                    new ActivityContactperson{Activity = activities[1], Contactperson = contacts[0]},
                    new ActivityContactperson{Activity = activities[1], Contactperson = contacts[1]},
                    new ActivityContactperson{Activity = activities[1], Contactperson = contacts[2]},
                    new ActivityContactperson{Activity = activities[2], Contactperson = contacts[1]},
                    new ActivityContactperson{Activity = activities[3], Contactperson = contacts[3]},
                    new ActivityContactperson{Activity = activities[4], Contactperson = contacts[3]},
                    new ActivityContactperson{Activity = activities[5], Contactperson = contacts[3]},
                };

                foreach (var ac in actContacts)
                {
                    _dataContext.ActivityContactpersons.AddAsync(ac).Wait();
                }

                var nextUser = new List<NextstepUser>
                {
                    new NextstepUser{NextStep = nextSteps[0], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new NextstepUser{NextStep = nextSteps[0], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new NextstepUser{NextStep = nextSteps[1], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new NextstepUser{NextStep = nextSteps[2], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new NextstepUser{NextStep = nextSteps[3], User = _dataContext.Users.Single(x => x.Id == 12)},
                    new NextstepUser{NextStep = nextSteps[3], User = _dataContext.Users.Single(x => x.Id == 13)},
                    new NextstepUser{NextStep = nextSteps[3], User = _dataContext.Users.Single(x => x.Id == 7)},
                    new NextstepUser{NextStep = nextSteps[3], User = _dataContext.Users.Single(x => x.Id == 14)},
                    new NextstepUser{NextStep = nextSteps[4], User = _dataContext.Users.Single(x => x.Id == 14)},
                    new NextstepUser{NextStep = nextSteps[5], User = _dataContext.Users.Single(x => x.Id == 14)},
                };

                foreach (var nu in nextUser)
                {
                    _dataContext.NextstepUsers.AddAsync(nu).Wait();
                }

                var nextContacts = new List<NextstepContactperson>
                {
                    new NextstepContactperson{NextStep = nextSteps[0], Contactperson = contacts[0]},
                    new NextstepContactperson{NextStep = nextSteps[0], Contactperson = contacts[1]},
                    new NextstepContactperson{NextStep = nextSteps[0], Contactperson = contacts[2]},
                    new NextstepContactperson{NextStep = nextSteps[1], Contactperson = contacts[2]},
                    new NextstepContactperson{NextStep = nextSteps[2], Contactperson = contacts[0]},
                    new NextstepContactperson{NextStep = nextSteps[3], Contactperson = contacts[3]},
                    new NextstepContactperson{NextStep = nextSteps[4], Contactperson = contacts[3]},
                    new NextstepContactperson{NextStep = nextSteps[5], Contactperson = contacts[3]},
                };

                foreach (var nc in nextContacts)
                {
                    _dataContext.NextstepContactpersons.AddAsync(nc).Wait();
                }



                //_________________________CRM-SEED__________________________


                _dataContext.SaveChangesAsync().Wait();

            }
        }
    }
}