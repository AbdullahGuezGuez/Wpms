using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _usermanager;
        private readonly IMapper _mapper;

        public CustomerController(DataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _usermanager = userManager;
            _context = context;
            this._mapper = mapper;
        }

        [Authorize(Policy = "Require-R-Customer")]
        [HttpGet("customers/{active}")]
        public async Task<IActionResult> GetAllCustomersForOrganization(bool active)
        {
            var customers = new List<Customer>();
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            if(active)
            {
                customers = await _context.Customers.Where(x => x.OrganizationId == UsersOrgId && x.CustomerStatus != Status.Inactive).ToListAsync();
            }
            else
            {
                customers = await _context.Customers.Where(x => x.OrganizationId == UsersOrgId && x.CustomerStatus == Status.Inactive).ToListAsync();
            }
            
            var customerList = new List<CustomerListDto>();
            foreach (var customer in customers)
            {
                var responsiblecontactperson = "";

                if(await _context.Contactpersons.AnyAsync(x => x.CustomerId == customer.Id && x.Responsible == true))
                {
                    responsiblecontactperson = (await _context.Contactpersons.SingleOrDefaultAsync(x => x.CustomerId == customer.Id && x.Responsible == true)).Name;
                }

                else
                {
                    responsiblecontactperson = "None";
                }

                var customerDto = new CustomerListDto()
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Region = customer.Region,
                    OrganizationNumber = customer.OrganizationNumber,
                    Responsible = responsiblecontactperson,
                    Status = customer.CustomerStatus.ToString()
                };
                customerList.Add(customerDto);
            }

            return Ok(customerList);
        }

        [Authorize(Policy = "Require-R-Customer")]
        [HttpGet("customerwithcustomfields/{id}")]
        public async Task<IActionResult> GetCustomerWithId(int id)
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            var customerSiteDto = new CustomerSiteDto();
            customerSiteDto.Customer = await _context.Customers.SingleOrDefaultAsync(x => x.Id == id && x.OrganizationId == UsersOrgId);
            customerSiteDto.Status = customerSiteDto.Customer.CustomerStatus.ToString();
            customerSiteDto.Customer.CustomFields = await _context.CustomFields.Where(x => x.CustomerId == customerSiteDto.Customer.Id).ToListAsync();

            return Ok(customerSiteDto);
        }

        [Authorize(Policy = "Require-CUD-Customer")]
        [HttpPut("customer")]
        public async Task<IActionResult> EditCustomer([FromBody]Customer customer)
        {

            var customerToChange = await _context.Customers.SingleAsync(x => x.Id == customer.Id);

            if (customerToChange.Name != customer.Name)
            {
                customerToChange.Name = customer.Name;
            }
            if (customerToChange.Region != customer.Region)
            {
                customerToChange.Region = customer.Region;
            }
            if (customerToChange.Address != customer.Address)
            {
                customerToChange.Address = customer.Address;
            }
            if (customerToChange.Telephone != customer.Telephone)
            {
                customerToChange.Telephone = customer.Telephone;
            }
            if (customerToChange.Customermail != customer.Customermail)
            {
                customerToChange.Customermail = customer.Customermail;
            }
            if (customerToChange.CustomerStatus != customer.CustomerStatus)
            {
                customerToChange.CustomerStatus = customer.CustomerStatus;
            }
            if (customerToChange.OrganizationNumber != customer.OrganizationNumber)
            {
                customerToChange.OrganizationNumber = customer.OrganizationNumber;
            }
            if (customerToChange.CustomerDescription != customer.CustomerDescription)
            {
                customerToChange.CustomerDescription = customer.CustomerDescription;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        [Authorize(Policy = "Require-CUD-Customer")]
        [HttpPost("customer")]
        public async Task<IActionResult> CreateCustomer([FromBody]Customer customer)
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            customer.OrganizationId = UsersOrgId;
            customer.FirstContacted = DateTime.Now.Date;

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Ok(customer.Id);
        }

        [Authorize(Policy = "Require-R-Contactperson")]
        [HttpGet("contactpersons/{CustomerId}")]
        public async Task<IActionResult> GetContactPersonsForCustomer(int CustomerId)
        {
            var contactpersons = await _context.Contactpersons.Where(x => x.CustomerId == CustomerId).ToListAsync();
            return Ok(contactpersons);
        }

        [Authorize(Policy = "Require-CUD-Contactperson")]
        [HttpPost("contactperson")]
        public async Task<IActionResult> AddContactperson([FromBody]Contactperson contactperson)
        {
            contactperson.Initials = getInitials(contactperson.Name);
            contactperson.Masked = false;
            await _context.Contactpersons.AddAsync(contactperson);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "Require-CUD-Contactperson")]
        [HttpPut("maskcontactperson")]
        public async Task<IActionResult> MaskContactperson([FromBody]int contactpersonId)
        {
            var contactperson = await _context.Contactpersons.SingleOrDefaultAsync(x => x.Id == contactpersonId);
            var initials = contactperson.Initials.Split(".");
            contactperson.Name = initials[0]+"xxxx "+initials[1]+"xxxx";
            contactperson.Role = "deleted";
            contactperson.Mail = "deleted";
            contactperson.Telephone = "000-0000000";
            contactperson.SecTelephone = "000-0000000";
            contactperson.Masked = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "Require-CUD-Contactperson")]
        [HttpPut("responsiblecontactperson")]
        public async Task<IActionResult> ChangeResponsibleContactperson([FromBody]Contactperson contact)
        {
            if(await _context.Contactpersons.AnyAsync(x => x.Responsible == true && x.CustomerId == contact.CustomerId))
            {
                var prevResponsibleContact = await _context.Contactpersons.FirstOrDefaultAsync(x => x.Responsible == true && x.CustomerId == contact.CustomerId);
                prevResponsibleContact.Responsible = false;
                await _context.SaveChangesAsync(); 
            }
            
            var contactToChange = await _context.Contactpersons.SingleOrDefaultAsync(x => x.Id == contact.Id);
            contactToChange.Responsible = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "Require-CUD-Contactperson")]
        [HttpPut("contactperson")]
        public async Task<IActionResult> EditContactperson([FromBody]Contactperson contact)
        {

            var contactToChange = await _context.Contactpersons.SingleAsync(x => x.Id == contact.Id);

            if (contactToChange.Role != contact.Role)
            {
                contactToChange.Role = contact.Role;
            }
            if (contactToChange.Mail != contact.Mail)
            {
                contactToChange.Mail = contact.Mail;
            }
            if (contactToChange.Name != contact.Name)
            {
                contactToChange.Name = contact.Name;
                contactToChange.Initials = getInitials(contact.Name);
            }
            if (contactToChange.Telephone != contact.Telephone)
            {
                contactToChange.Telephone = contact.Telephone;
            }
            if (contactToChange.SecTelephone != contact.SecTelephone)
            {
                contactToChange.SecTelephone = contact.SecTelephone;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        #region Activities

        [Authorize(Policy = "Require-R-Activity")]
        [HttpGet("activities/{customerId}/{includeArchived}")]
        public async Task<IActionResult> GetAllArchivedActivitiesForCustomer(int customerId, bool includeArchived)
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);

            List<ActivityNextStepDto> activityList = new List<ActivityNextStepDto>();
            var activities = new List<Activity>();

            if (includeArchived)
            {
                activities = await _context.Activities.Where(x => x.Customer.Id == customerId && x.Customer.OrganizationId == UsersOrgId).ToListAsync();
            }
            else if (!includeArchived)
            {
                activities = await _context.Activities.Where(x => x.Customer.Id == customerId && x.Customer.OrganizationId == UsersOrgId && x.Archived == false).ToListAsync();
            }
            else
                Console.WriteLine("Something went wrong");

            foreach (var activity in activities)
            {
                var creatorNameActivity = await _context.Users.SingleOrDefaultAsync(x => x.Id == activity.CreatorId);

                var nextStep = new NextStep();
                var nextstepUsers = new List<User>();
                var nextstepContactpersons = new List<Contactperson>();
                if (activity.NextStepId != null)
                {
                    nextStep = await _context.NextStep.SingleOrDefaultAsync(x => x.Id == activity.NextStepId);

                    var creatorNameNextStep = await _context.Users.SingleOrDefaultAsync(x => x.Id == nextStep.CreatorId);
                    nextstepUsers = await _context.Users
                    .FromSql("SELECT * FROM AspNetUsers JOIN NextstepUsers ON AspNetUsers.Id = NextstepUsers.UserId WHERE NextstepUsers.NextstepId = {0}", nextStep.Id).ToListAsync();
                    nextstepContactpersons = await _context.Contactpersons
                    .FromSql("SELECT * FROM Contactpersons JOIN NextstepContactpersons ON Contactpersons.Id = NextstepContactpersons.ContactpersonId WHERE NextstepContactpersons.NextstepId = {0}", nextStep.Id).ToListAsync();
                }

                var activityUsers = await _context.Users
                .FromSql("SELECT * FROM AspNetUsers JOIN ActivityUsers ON AspNetUsers.Id = ActivityUsers.UserId WHERE ActivityUsers.ActivityId = {0}", activity.Id).ToListAsync();
                var activityContactpersons = await _context.Contactpersons
                .FromSql("SELECT * FROM Contactpersons JOIN ActivityContactpersons ON Contactpersons.Id = ActivityContactpersons.ContactpersonId WHERE ActivityContactpersons.ActivityId = {0}", activity.Id).ToListAsync();

                var activityNextStepDto = new ActivityNextStepDto()
                {
                    Id = activity.Id,
                    NextStepId = nextStep.Id,
                    Description = activity.Description,
                    Title = activity.Title,
                    Date = activity.Date,
                    Type = activity.Type.ToString(),
                    Creator = creatorNameActivity.UserName,
                    BusinessParticipants = activityUsers,
                    CustomerParticipiants = activityContactpersons,
                    Archived = activity.Archived
                };

                if (nextStep.Title != null)
                {
                    activityNextStepDto.NextStepBusinessParticipants = nextstepUsers;
                    activityNextStepDto.NextStepCustomerParticipiants = nextstepContactpersons;
                    activityNextStepDto.NextStepDescription = nextStep.Description;
                    activityNextStepDto.NextStepTitle = nextStep.Title;
                    activityNextStepDto.NextStepDate = nextStep.Date;
                    activityNextStepDto.NextStepType = nextStep.Type.ToString();
                    activityNextStepDto.NextStepCreator = creatorNameActivity.UserName;
                }

                activityList.Add(activityNextStepDto);
            }

            return Ok(activityList);
        }

        [Authorize(Policy = "Require-R-Activity")]
        [HttpGet("allactivities/{includeArchived}")]
        public async Task<IActionResult> GetAllActivities(bool includeArchived)
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);

            List<ActivityNextStepDto> activityList = new List<ActivityNextStepDto>();
            var activities = new List<Activity>();

            if (includeArchived)
            {
                activities = await _context.Activities.Where(x => x.OrganizationId == UsersOrgId).ToListAsync();
            }
            else if (!includeArchived)
            {
                activities = await _context.Activities.Where(x => x.OrganizationId == UsersOrgId && x.Archived == false).ToListAsync();
            }
            else
                Console.WriteLine("Something went wrong");

            foreach (var activity in activities)
            {
                var creatorNameActivity = await _context.Users.SingleOrDefaultAsync(x => x.Id == activity.CreatorId);

                var nextStep = new NextStep();
                var nextstepUsers = new List<User>();
                var nextstepContactpersons = new List<Contactperson>();
                if (activity.NextStepId != null)
                {
                    nextStep = await _context.NextStep.SingleOrDefaultAsync(x => x.Id == activity.NextStepId);

                    var creatorNameNextStep = await _context.Users.SingleOrDefaultAsync(x => x.Id == nextStep.CreatorId);
                    nextstepUsers = await _context.Users
                    .FromSql("SELECT * FROM AspNetUsers JOIN NextstepUsers ON AspNetUsers.Id = NextstepUsers.UserId WHERE NextstepUsers.NextstepId = {0}", nextStep.Id).ToListAsync();
                    nextstepContactpersons = await _context.Contactpersons
                    .FromSql("SELECT * FROM Contactpersons JOIN NextstepContactpersons ON Contactpersons.Id = NextstepContactpersons.ContactpersonId WHERE NextstepContactpersons.NextstepId = {0}", nextStep.Id).ToListAsync();
                }

                var activityUsers = await _context.Users
                .FromSql("SELECT * FROM AspNetUsers JOIN ActivityUsers ON AspNetUsers.Id = ActivityUsers.UserId WHERE ActivityUsers.ActivityId = {0}", activity.Id).ToListAsync();
                var activityContactpersons = await _context.Contactpersons
                .FromSql("SELECT * FROM Contactpersons JOIN ActivityContactpersons ON Contactpersons.Id = ActivityContactpersons.ContactpersonId WHERE ActivityContactpersons.ActivityId = {0}", activity.Id).ToListAsync();

                var activityNextStepDto = new ActivityNextStepDto()
                {
                    Id = activity.Id,
                    NextStepId = nextStep.Id,
                    Description = activity.Description,
                    Title = activity.Title,
                    Date = activity.Date,
                    Type = activity.Type.ToString(),
                    Creator = creatorNameActivity.UserName,
                    BusinessParticipants = activityUsers,
                    CustomerParticipiants = activityContactpersons,
                    Archived = activity.Archived
                };

                if (nextStep.Title != null)
                {
                    activityNextStepDto.NextStepBusinessParticipants = nextstepUsers;
                    activityNextStepDto.NextStepCustomerParticipiants = nextstepContactpersons;
                    activityNextStepDto.NextStepDescription = nextStep.Description;
                    activityNextStepDto.NextStepTitle = nextStep.Title;
                    activityNextStepDto.NextStepDate = nextStep.Date;
                    activityNextStepDto.NextStepType = nextStep.Type.ToString();
                    activityNextStepDto.NextStepCreator = creatorNameActivity.UserName;
                }

                activityList.Add(activityNextStepDto);
            }

            return Ok(activityList);
        }

        [Authorize(Policy = "Require-R-Activity")]
        [HttpGet("alltodos/{includeChecked}")]
        public async Task<IActionResult> GetAllTodos(bool includeChecked)
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);

            List<ActivityNextStepDto> activityList = new List<ActivityNextStepDto>();
            var activities = new List<Activity>();

            if(includeChecked)
            {
                activities = await _context.Activities
                .FromSql("SELECT * FROM Activities JOIN ActivityUsers ON Activities.Id = ActivityUsers.ActivityId WHERE ActivityUsers.UserId = {0} AND Activities.OrganizationId = {1} AND Activities.TodoChecked = true AND Activities.Type == 'ToDo' ORDER BY Date Desc", UsersId, UsersOrgId)
                .ToListAsync();
            }
            if(!includeChecked)
            {
                activities = await _context.Activities
                .FromSql("SELECT * FROM Activities JOIN ActivityUsers ON Activities.Id = ActivityUsers.ActivityId WHERE ActivityUsers.UserId = {0} AND Activities.OrganizationId = {1} AND Activities.TodoChecked = false AND Activities.Type == 'ToDo' ORDER BY Date Desc", UsersId, UsersOrgId)
                .ToListAsync();
            }
                  
            foreach (var activity in activities)
            {
                var creatorNameActivity = await _context.Users.SingleOrDefaultAsync(x => x.Id == activity.CreatorId);

                var nextStep = new NextStep();
                var nextstepUsers = new List<User>();
                var nextstepContactpersons = new List<Contactperson>();
                if (activity.NextStepId != null)
                {
                    nextStep = await _context.NextStep.SingleOrDefaultAsync(x => x.Id == activity.NextStepId);

                    var creatorNameNextStep = await _context.Users.SingleOrDefaultAsync(x => x.Id == nextStep.CreatorId);
                    nextstepUsers = await _context.Users
                    .FromSql("SELECT * FROM AspNetUsers JOIN NextstepUsers ON AspNetUsers.Id = NextstepUsers.UserId WHERE NextstepUsers.NextstepId = {0}", nextStep.Id).ToListAsync();
                    nextstepContactpersons = await _context.Contactpersons
                    .FromSql("SELECT * FROM Contactpersons JOIN NextstepContactpersons ON Contactpersons.Id = NextstepContactpersons.ContactpersonId WHERE NextstepContactpersons.NextstepId = {0}", nextStep.Id).ToListAsync();
                }

                var activityUsers = await _context.Users
                .FromSql("SELECT * FROM AspNetUsers JOIN ActivityUsers ON AspNetUsers.Id = ActivityUsers.UserId WHERE ActivityUsers.ActivityId = {0}", activity.Id).ToListAsync();
                var activityContactpersons = await _context.Contactpersons
                .FromSql("SELECT * FROM Contactpersons JOIN ActivityContactpersons ON Contactpersons.Id = ActivityContactpersons.ContactpersonId WHERE ActivityContactpersons.ActivityId = {0}", activity.Id).ToListAsync();

                var activityNextStepDto = new ActivityNextStepDto()
                {
                    Id = activity.Id,
                    NextStepId = nextStep.Id,
                    Description = activity.Description,
                    Title = activity.Title,
                    Date = activity.Date,
                    Type = activity.Type.ToString(),
                    TodoChecked = activity.TodoChecked,
                    Creator = creatorNameActivity.UserName,
                    BusinessParticipants = activityUsers,
                    CustomerParticipiants = activityContactpersons,
                    Archived = activity.Archived
                };

                if (nextStep.Title != null)
                {
                    activityNextStepDto.NextStepBusinessParticipants = nextstepUsers;
                    activityNextStepDto.NextStepCustomerParticipiants = nextstepContactpersons;
                    activityNextStepDto.NextStepDescription = nextStep.Description;
                    activityNextStepDto.NextStepTitle = nextStep.Title;
                    activityNextStepDto.NextStepDate = nextStep.Date;
                    activityNextStepDto.NextStepType = nextStep.Type.ToString();
                    activityNextStepDto.NextStepCreator = creatorNameActivity.UserName;
                }

                activityList.Add(activityNextStepDto);
            }

            return Ok(activityList);
        }

        [HttpPut("checktodo/{todoId}")]
        public async Task<IActionResult> CheckTodo(int todoId) 
        {
            var todo = await _context.Activities.SingleOrDefaultAsync(x => x.Id == todoId);
            todo.TodoChecked = true;
            await _context.SaveChangesAsync();

            return Ok(todo);
        }

        [HttpPut("unchecktodo/{todoId}")]
        public async Task<IActionResult> UnCheckTodo(int todoId)
        {
            var todo = await _context.Activities.SingleOrDefaultAsync(x => x.Id == todoId);
            todo.TodoChecked = false;
            await _context.SaveChangesAsync();

            return Ok(todo);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("activities")]
        public async Task<IActionResult> CreateNewActivity([FromBody]CreateActivityDto createActivityDto)
        {
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            var activityDate = new DateTime();

            if(createActivityDto.Time != null || createActivityDto.Date != null)
            {
                var splittedTime = createActivityDto.Time.Split(":");
                activityDate = DateTime.Parse(createActivityDto.Date).AddHours(double.Parse(splittedTime[0])).AddMinutes(double.Parse(splittedTime[1]));
            }

            var newActivity = new Activity()
            {
                Title = createActivityDto.Title,
                Description = createActivityDto.Description,
                Date = activityDate,
                Type = createActivityDto.Type,
                CreatorId = UsersId,
                Archived = false,
                OrganizationId = UsersOrgId
            };

            if(createActivityDto.CustomerId != 0 || createActivityDto.CustomerId != null)
            {
                newActivity.CustomerId = createActivityDto.CustomerId;
            }

            await _context.Activities.AddAsync(newActivity);
            await _context.SaveChangesAsync();
            // Check if users or contactpersons are null

            if (createActivityDto.UsersForActivity != null)
            {
                foreach (var user in createActivityDto.UsersForActivity)
                {
                    var activityUser = new ActivityUser()
                    {
                        UserId = user.Id,
                        ActivityId = newActivity.Id
                    };
                    await _context.ActivityUsers.AddAsync(activityUser);
                }
            }

            if (createActivityDto.ContactpersonsForActivity != null)
            {
                foreach (var contactperson in createActivityDto.ContactpersonsForActivity)
                {
                    var activityContactperson = new ActivityContactperson()
                    {
                        ContactpersonId = contactperson.Id,
                        ActivityId = newActivity.Id
                    };
                    await _context.ActivityContactpersons.AddAsync(activityContactperson);
                }
            }


            await _context.SaveChangesAsync();

            return Ok(createActivityDto);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("activitynextstep")]
        public async Task<IActionResult> CreateActivityWithNextStep([FromBody]CreateActivityAndNextStepDto createActivityAndNextStepDto)
        {
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);

            var activityDate = new DateTime();
            if(createActivityAndNextStepDto.Time != null || createActivityAndNextStepDto.Date != null)
            {
                var splittedTime = createActivityAndNextStepDto.Time.Split(":");
                activityDate = DateTime.Parse(createActivityAndNextStepDto.Date).AddHours(double.Parse(splittedTime[0])).AddMinutes(double.Parse(splittedTime[1]));
            }
            var newActivity = new Activity()
            {
                Title = createActivityAndNextStepDto.Title,
                Description = createActivityAndNextStepDto.Description,
                Date = activityDate,
                Type = createActivityAndNextStepDto.Type,
                CreatorId = UsersId,
                Archived = false

            };

            if(createActivityAndNextStepDto.CustomerId != 0)
            {
                newActivity.CustomerId = createActivityAndNextStepDto.CustomerId;
            }

            await _context.Activities.AddAsync(newActivity);
            await _context.SaveChangesAsync();

            var nextStepSplittedTime = createActivityAndNextStepDto.NextstepTime.Split(":");
            var nextStepDate = DateTime.Parse(createActivityAndNextStepDto.NextstepDate)
            .AddHours(double.Parse(nextStepSplittedTime[0]))
            .AddMinutes(double.Parse(nextStepSplittedTime[1]));

            var newNextStep = new NextStep()
            {
                Title = createActivityAndNextStepDto.NextstepTitle,
                Description = createActivityAndNextStepDto.NextstepDescription,
                Date = nextStepDate,
                Type = createActivityAndNextStepDto.NextstepType,
                CreatorId = UsersId,
                ActivityId = newActivity.Id
            };

            await _context.NextStep.AddAsync(newNextStep);
            await _context.SaveChangesAsync();

            newActivity.NextStepId = newNextStep.Id;
            await _context.SaveChangesAsync();

            // Check if users or contactpersons are null

            if (createActivityAndNextStepDto.UsersForActivity != null)
            {
                foreach (var user in createActivityAndNextStepDto.UsersForActivity)
                {
                    var activityUser = new ActivityUser()
                    {
                        UserId = user.Id,
                        ActivityId = newActivity.Id
                    };
                    await _context.ActivityUsers.AddAsync(activityUser);
                }
            }

            if (createActivityAndNextStepDto.ContactpersonsForActivity != null)
            {
                foreach (var contactperson in createActivityAndNextStepDto.ContactpersonsForActivity)
                {
                    var activityContactperson = new ActivityContactperson()
                    {
                        ContactpersonId = contactperson.Id,
                        ActivityId = newActivity.Id
                    };
                    await _context.ActivityContactpersons.AddAsync(activityContactperson);
                }
            }

            if (createActivityAndNextStepDto.NextstepUsersForActivity != null)
            {
                foreach (var user in createActivityAndNextStepDto.NextstepUsersForActivity)
                {
                    var nextStepUser = new NextstepUser()
                    {
                        UserId = user.Id,
                        NextstepId = newNextStep.Id
                    };
                    await _context.NextstepUsers.AddAsync(nextStepUser);
                }
            }

            if (createActivityAndNextStepDto.NextstepContactpersonsForActivity != null)
            {
                foreach (var contactperson in createActivityAndNextStepDto.NextstepContactpersonsForActivity)
                {
                    var nextstepContactperson = new NextstepContactperson()
                    {
                        ContactpersonId = contactperson.Id,
                        NextstepId = newNextStep.Id
                    };
                    await _context.NextstepContactpersons.AddAsync(nextstepContactperson);
                }
            }


            await _context.SaveChangesAsync();

            return Ok(createActivityAndNextStepDto);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("nextstep")]
        public async Task<IActionResult> CreateNewNextStep([FromBody]CreateNextstepDto createNextstepDto)
        {
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);
            var amountOfNextSteps = await _context.NextStep.Where(x => x.ActivityId == createNextstepDto.ActivityId).ToListAsync();
            if (amountOfNextSteps.Count > 0)
            {
                return BadRequest("A nextstep already exists for this activity");
            }

            var activity = await _context.Activities.SingleOrDefaultAsync(x => x.Id == createNextstepDto.ActivityId);

            var splittedTime = createNextstepDto.Time.Split(":");
            var nextStepDate = DateTime.Parse(createNextstepDto.Date).AddHours(double.Parse(splittedTime[0])).AddMinutes(double.Parse(splittedTime[1]));

            int result = 10;
            if (nextStepDate != null)
            {
                result = DateTime.Compare(activity.Date, nextStepDate);
            }

            if (result > 0 && result < 10)
            {
                //aktiviteten är senare än nextstep
                return BadRequest("Nextsteps date is earlier than the activity, please change to a valid date");
            }

            var newNextStep = new NextStep()
            {
                Title = createNextstepDto.Title,
                Description = createNextstepDto.Description,
                Date = nextStepDate,
                Type = createNextstepDto.Type,
                CreatorId = UsersId,
                ActivityId = activity.Id
            };

            await _context.NextStep.AddAsync(newNextStep);
            await _context.SaveChangesAsync();


            activity.NextStepId = newNextStep.Id;
            await _context.SaveChangesAsync();
            return Ok(newNextStep);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("activityuser")]
        public async Task<IActionResult> CreateActivityUser([FromBody]ActivityUser activityUser)
        {
            await _context.ActivityUsers.AddAsync(activityUser);
            await _context.SaveChangesAsync();

            return Ok(activityUser);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("nextstepuser")]
        public async Task<IActionResult> CreateNextstepUser([FromBody]NextstepUser nextstepUser)
        {
            await _context.NextstepUsers.AddAsync(nextstepUser);
            await _context.SaveChangesAsync();

            return Ok(nextstepUser);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("activitycontactperson")]
        public async Task<IActionResult> CreateActivityContactperson([FromBody]ActivityContactperson activityContactperson)
        {
            await _context.ActivityContactpersons.AddAsync(activityContactperson);
            await _context.SaveChangesAsync();

            return Ok(activityContactperson);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPost("nextstepcontactperson")]
        public async Task<IActionResult> CreateNextstepContactperson([FromBody]NextstepContactperson nextstepContactperson)
        {
            await _context.NextstepContactpersons.AddAsync(nextstepContactperson);
            await _context.SaveChangesAsync();

            return Ok(nextstepContactperson);
        }

        [Authorize(Policy = "Require-CU-Activity")]
        [HttpPut("activities")] //! Not Finished
        public async Task<IActionResult> UpdateActivity([FromBody] Activity activity)
        {
            var selectedActivity = await _context.Activities.SingleOrDefaultAsync(x => x.Id == activity.Id);

            return Ok();
        }

        [Authorize(Policy = "Require-D-Activity")]
        [HttpPut("archiveactivity")]
        public async Task<IActionResult> ArchiveActivity([FromBody]int activityId)
        {
            var activity = await _context.Activities.SingleOrDefaultAsync(x => x.Id == activityId);
            activity.Archived = true;
            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        [Authorize(Policy = "Require-D-Activity")]
        [HttpPut("unarchiveactivity")]
        public async Task<IActionResult> UnArchiveActivity([FromBody]int activityId)
        {
            var activity = await _context.Activities.SingleOrDefaultAsync(x => x.Id == activityId);
            activity.Archived = false;
            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        [HttpGet("usernotifications")]
        public async Task<IActionResult> ActivitiesForNotifications()
        {
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);
            var usersActivities = await _context.Activities
            .FromSql("SELECT * FROM Activities JOIN ActivityUsers ON Activities.Id = ActivityUsers.ActivityId WHERE ActivityUsers.UserId = {0} AND Activities.Archived = false AND Activities.TodoChecked = false AND Activities.Type == 'ToDo' ORDER BY Date Asc", UsersId).ToListAsync();
            
            var activities = new List<ActivityNotificationDto>();
            var todaysDate = DateTime.Now;
            foreach(var userActivity in usersActivities)
            {
                var creator = await _context.Users.SingleOrDefaultAsync(x => x.Id == userActivity.CreatorId);
                var dateResult = DateTime.Compare(userActivity.Date, todaysDate);

                

                var activityNotificationDto = new ActivityNotificationDto()
                {
                    Id = userActivity.Id,
                    Description = userActivity.Description,
                    Title = userActivity.Title,
                    Date = userActivity.Date,
                    Type = userActivity.Type.ToString(),
                    Creator = creator.UserName,
                    Archived = userActivity.Archived
                };

                if(dateResult > 0)
                {
                    //Tododatum är tidigare än dagens datum, bra
                    activityNotificationDto.Delayed = false;
                }
                else if(dateResult < 0)
                {
                    //Tododatum är senare än dagens datum, dåligt
                    activityNotificationDto.Delayed = true;
                }
                else if(dateResult == 0)
                {
                    //Same, bra
                    activityNotificationDto.Delayed = false;
                }
                else
                    return BadRequest("Something went wron with the Date comparison");


                activities.Add(activityNotificationDto);
            }
            
            
            var activitiesForNotifications = new ActivitiesForNotificationDto() {
                NumberOfTodos = usersActivities.Count,
                UsersActivities = activities,
            };

            return Ok(activitiesForNotifications);
        }
        #endregion


        #region Custom Fields

        [Authorize(Policy = "Require-CUD-Customer")]
        [HttpPost("customfield")]
        public async Task<IActionResult> AddCustomField([FromBody]CustomField customField)
        {
            await _context.CustomFields.AddAsync(customField);

            await _context.SaveChangesAsync();

            return Ok(customField);
        }

        [Authorize(Policy = "Require-CUD-Customer")]
        [HttpDelete("customfield/{id}")]
        public async Task<IActionResult> DeleteCustomField(int id)
        {
            var customField = await this._context.CustomFields.FirstOrDefaultAsync(x => x.Id == id);
            _context.CustomFields.Remove(customField);
            await _context.SaveChangesAsync();
            return Ok(customField);
        }

        #endregion

        public string getInitials(string name)
        {
            var words = name.Split();
            var initials = "";
            if (words.Length == 2)
            {
                initials = words[0][0].ToString().ToUpper() + "." + words[1][0].ToString().ToUpper();
            }
            else if (words.Length < 2)
            {
                initials = words[0][0].ToString().ToUpper() + "." + words[0][0].ToString().ToUpper();
            }
            else if (words.Length > 2)
            {
                var wordLength = words.Length - 1;
                initials = words[0][0].ToString().ToUpper() + "." + words[wordLength][0].ToString().ToUpper();
            }
            return initials;
        }
    }
}
