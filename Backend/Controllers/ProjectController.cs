using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Backend.Dtos.TrelloDto;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        private readonly IWpmsRepository _wpmsRepository;
        public ProjectController(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IWpmsRepository IWpmsRepository)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _wpmsRepository = IWpmsRepository;
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("projectslist/{activeProjects}")]
        public async Task<IActionResult> GetProjectsList(bool activeProjects)
        {
            var userOrgId = int.Parse(User.FindFirst("Organization").Value);
            var allProjects = await _context.Projects.Where(x => x.OrganizationId == userOrgId && x.Active == activeProjects).ToListAsync();

            var listOfProjectDtos = new List<ProjectListDto>();

            foreach (var project in allProjects)
            {
                var customerName =(await _context.Customers.SingleOrDefaultAsync(x => x.Id == project.CustomerId)).Name;
                var allUsers = await _context.Users
                .FromSql("SELECT * FROM AspNetUsers JOIN ProjectUsers ON ProjectUsers.UserId = AspNetUsers.Id WHERE ProjectUsers.ProjectId = {0}", project.Id)
                .ToListAsync();
                var nameString = "";

                foreach(var user in allUsers)
                {
                    nameString = nameString + " " + user.FullName + ",";
                }

                var timeDifference = ((project.EstimatedTime - (int)project.TotalBudget));
                var projectListDto = new ProjectListDto()
                {
                    Id = project.Id,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    TotalBudget = project.TotalBudget,
                    Priority = project.Priority,
                    UsedTime = project.UsedTime,
                    EstimatedTime = project.EstimatedTime,
                    CustomerName = customerName,
                    AllMembers = nameString.Remove(nameString.Length - 1),
                    
                    EstimatedTimeOver = timeDifference,

                };
                
                if(await _context.TrelloBoards.AnyAsync(x => x.Id == project.TrelloBoardId))
                {
                    var alltrelloBoards = await _context.TrelloBoards.SingleOrDefaultAsync(x => x.Id == project.TrelloBoardId);
                    projectListDto.TrelloUrl = alltrelloBoards.Url;
                }

                listOfProjectDtos.Add(projectListDto);
            }

            return Ok(listOfProjectDtos);
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPost("createprojectusers/{id}")]
        public async Task<IActionResult> CreateProjectUser(int id, [FromBody] List<User> users)
        {
            var userOrgId = int.Parse(User.FindFirst("Organization").Value);
            var boardId = _context.Projects.FirstOrDefault(x => x.Id == id).TrelloBoardId;
            var currentMembers = await _context.ProjectUsers.Where(x => x.ProjectId == id).ToListAsync();
            var projectUser = new ProjectUser
            {
                ProjectId = id
            };

            foreach (var currentMember in currentMembers)
            {
                bool found = false;

                foreach (var user in users)
                {
                    if (user.Id == currentMember.UserId)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    var userToRemove = await _context.Users.FirstOrDefaultAsync(x=> x.Id == currentMember.UserId);
                    await _wpmsRepository.DeleteUserFromTrelloBoard(userOrgId,boardId, userToRemove.TrelloMemberId);
                    _context.ProjectUsers.Remove(currentMember);
                    await _context.SaveChangesAsync();
                }
            }

            foreach (var user in users)
            {    // Hämta hem alla members i projektet och kolla att dem är med i listan, på så sätt skulle man kunna utesluta dem som inte är också

                var isAlreadyMember = await _context.ProjectUsers.AnyAsync(x => x.ProjectId == id && x.UserId == user.Id);

                if (!isAlreadyMember)
                {
                    await _wpmsRepository.AddUserToTrelloBoard(boardId,user.Email,userOrgId);
                    projectUser.UserId = user.Id;

                    await _context.ProjectUsers.AddAsync(projectUser);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(RedirectToRoute(""));
        }

        [Authorize(Policy = "Require-R-AssignedProject")]
        [HttpGet("projectmembersinproject/{id}")]
        public async Task<IActionResult> GetProjectMembers(int id)
        {
            var orgId = int.Parse(User.FindFirst("Organization").Value);

            var projectUsers = await _context.Users.FromSql("SELECT * FROM AspNetUsers WHERE id IN (SELECT UserId FROM ProjectUsers WHERE ProjectId = " + id + ") AND id IN (SELECT UserId From OrganizationUser where OrganizationUser.OrganizationId = " + orgId + ")").ToListAsync();

            return Ok(projectUsers);
        }

        [Authorize(Policy = "Require-R-AssignedProject")]
        [HttpGet("projectmembers/{id}")]
        public async Task<IActionResult> GetAvailableProjectMembers(int id)
        {
            var orgId = int.Parse(User.FindFirst("Organization").Value);

            var projectUsers = await _context.Users.FromSql("SELECT * FROM AspNetUsers WHERE id NOT IN (SELECT UserId FROM ProjectUsers WHERE ProjectId = " + id + ") AND id IN (SELECT UserId From OrganizationUser where OrganizationUser.OrganizationId = " + orgId + ")").ToListAsync();

            return Ok(projectUsers);
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var userOrgId = User.FindFirst("Organization").Value;

            var allProjects = await _context.Projects.Where(x => x.OrganizationId.ToString() == userOrgId).ToListAsync();


            return Ok(allProjects);
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("projectsforcustomer/{customerId}")]
        public async Task<IActionResult> GetAllProjectsForCustomer(int customerId)
        {

            var allCustomerProjects = await _context.Projects.Where(x => x.CustomerId == customerId).ToListAsync();

            return Ok(allCustomerProjects);
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("project/{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(project);
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPost("createproject")]
        public async Task<IActionResult> CreateProject(CreateProjectDto createProjectDto)
        {
            try
            {
                var userName = User.Identity.Name;
                var orgId = int.Parse(User.FindFirst("Organization").Value);
                var user = await _userManager.FindByNameAsync(userName);
                var numProject = _context.Projects.Where(x => x.OrganizationId == orgId).Count();
                Project project = new Project()
                {
                    Name = createProjectDto.Name,
                    CustomerId = createProjectDto.CustomerId,
                    ResponsibleContactpersonId = createProjectDto.ResponsibleContactpersonId,
                    ResponsibleUserId = createProjectDto.ResponsibleUserId,
                    StartDate = createProjectDto.StartDate,
                    EndDate = createProjectDto.EndDate,
                    Priority = (numProject + 1),
                    TotalBudget = createProjectDto.TotalBudget,
                    Description = createProjectDto.Description,
                    OrganizationId = orgId,
                    Active = true,
                    CreatorId = user.Id,
                    EstimatedTime = 0,
                    UsedTime = 0
                };

                if (createProjectDto.IncludeTrello)
                {    
                    var boardResponse = await _wpmsRepository.CreateTrelloBoard(createProjectDto.TrelloBoardName, orgId);
                    var board = _wpmsRepository.ConvertJsonToTrelloBoard(boardResponse);
                    project.TrelloBoardId = board.Id; 
                }

                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();
                
                var addedProject = await _context.Projects.Where(x => x.Name == project.Name && x.OrganizationId == orgId && x.CustomerId == project.CustomerId).SingleOrDefaultAsync();
                
                if(createProjectDto.ResponsibleUserId != 0)
                {
                    var projUser = new ProjectUser{ProjectId = addedProject.Id, UserId = createProjectDto.ResponsibleUserId};
                    await _context.ProjectUsers.AddAsync(projUser);
                }

                
               
                await _context.SaveChangesAsync();
                return Ok(addedProject.Id);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong");
            }

        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("getUserTasks/{projectId}/{listId}")]
        public async Task<IActionResult> GetUserTasks(string projectId, string listId)
        {
            var Users = await _context.Users.FromSql("SELECT * from AspNetUsers where AspNetUsers.Id in (select UserCards.UserId from UserCards where UserCards.TrelloCardId in (SELECT TrelloCards.Id from TrelloCards where TrelloCards.IdList = '" + listId + "'))and AspNetUsers.Id in (SELECT ProjectUsers.UserId from ProjectUsers where ProjectUsers.ProjectId = " + projectId + ")").ToListAsync();


            List<TasksDto> theList = new List<TasksDto>();

            foreach (var user in Users)
            {

                var taskList = await _context.TrelloCards.FromSql("select * from TrelloCards where TrelloCards.Id in (Select UserCards.TrelloCardId from UserCards where UserCards.UserId = " + user.Id + " )and TrelloCards.IdList in (Select TrelloLists.Id from TrelloLists where TrelloLists.Id = '" + listId + "' and TrelloLists.IdBoard in (select Projects.TrelloBoardId from Projects where Projects.Id = " + projectId + "))").Select(x => x.Name).ToListAsync();
                var usersTask = new TasksDto() { CardNames = taskList, UserName = user.UserName };

                theList.Add(usersTask);

            }

            return Ok(theList);
        }


        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("boardlists/{Id}")]
        public async Task<IActionResult> GetProjectLists(int Id)
        {
            var project = await _context.Projects.FirstAsync(x => x.Id == Id);
            var boardList = await _context.TrelloLists.Where(x => x.IdBoard == project.TrelloBoardId).ToListAsync();
            return Ok(boardList);
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("projectvalues/{Id}")]
        public async Task<IActionResult> GetProjectValues(int Id)
        {
            var projectValues = await ProjectValues(Id);

            return Ok(projectValues);
        }


        [Authorize(Policy = "Require-CU-Project")]
        [HttpGet("availableboards")]
        public async Task<IActionResult> GetAvailableBoards()
        {
            var availableBoards = new List<ChangeTrelloBoardDto>();
            var orgId = int.Parse(User.FindFirst("Organization").Value);
            var allProjects = await _context.Projects.Where(x => x.OrganizationId == orgId).ToListAsync();
            var allBoards = await _context.TrelloBoards.Where(x => x.OrganizationId == orgId).ToListAsync();

            foreach (var board in allBoards)
            {
                bool projectGotBoard = false;

                foreach (var project in allProjects)
                {
                    if (project.TrelloBoardId == board.Id)
                    {
                        projectGotBoard = true;
                    }
                    if (projectGotBoard)
                    {
                        break;
                    }
                }

                if (!projectGotBoard)
                {
                    var availableBoard = new ChangeTrelloBoardDto
                    {
                        Name = board.Name,
                        Id = board.Id
                    };

                    availableBoards.Add(availableBoard);
                }
            }

            return Ok(availableBoards);
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPut("changetrelloboard")]
        public async Task<IActionResult> ChangeTrelloBoardConnection([FromBody]ChangeTrelloBoardWithProjectDto idHolder)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id == idHolder.projectId);
            project.TrelloBoardId = idHolder.trelloBoardId;
            project.UsedTime = 0;
            project.EstimatedTime = 0;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPut("removetrelloconnection")]
        public async Task<IActionResult> RemoveTrelloBoardConnection([FromBody]ChangeTrelloBoardWithProjectDto idHolder)
        {
            var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id == idHolder.projectId);
            project.TrelloBoardId = "No Connection";
            project.UsedTime = 0;
            project.EstimatedTime = 0;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPost("createtrelloboard")]
        public async Task<IActionResult> CreateTrelloBoard([FromBody]CreateTrelloBoardDto projIdAndNewName)      //TODO: Koppla TRELLOBOARDID med projektet, hämta hem trelloboardgrejen
        {
            var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id == projIdAndNewName.projectId);
            var orgId = int.Parse(User.FindFirst("Organization").Value);
            var boardResponse = await _wpmsRepository.CreateTrelloBoard(projIdAndNewName.name, orgId);
            var board = _wpmsRepository.ConvertJsonToTrelloBoard(boardResponse);
            project.UsedTime = 0;
            project.EstimatedTime = 0;
            project.TrelloBoardId = board.Id;
            await _context.SaveChangesAsync();
            return Ok(board.Id);
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPut("changepriority")]
        public async Task<IActionResult> ChangePriority([FromBody]ChangePrioDto changePrio)
        {
            //* Ja jag vet att man rent tekniskt kan hämta ut prion genom projektid:et men pallade inte krångla */
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            var allProjects = new List<Project>();
            
            if(changePrio.NewPrio < changePrio.OldPrio) // Högre prioritering
            {
                allProjects = await _context.Projects.Where(x => x.OrganizationId == UsersOrgId && x.Priority >= changePrio.NewPrio && x.Priority < changePrio.OldPrio).ToListAsync();
                foreach(var project in allProjects)
                {
                    project.Priority = project.Priority + 1;
                }
            }

            if(changePrio.NewPrio > changePrio.OldPrio) // Lägre prioritering
            {
                allProjects = await _context.Projects.Where(x => x.OrganizationId == UsersOrgId && x.Priority > changePrio.OldPrio && x.Priority <= changePrio.NewPrio).ToListAsync();
                foreach(var project in allProjects)
                {
                    project.Priority = project.Priority - 1;
                }
            }
            
            else if(changePrio.NewPrio == changePrio.OldPrio)
            {
                return BadRequest("The priorities are the same, no changes made");
            }
            
            var projectTochange = await _context.Projects.SingleOrDefaultAsync(x => x.Id == changePrio.ProjectId);
            projectTochange.Priority = changePrio.NewPrio;
            await _context.SaveChangesAsync();

            return Ok(allProjects);
        }

        [Authorize(Policy = "Require-CU-Project")]
        [HttpPut("changeactiveproject")]
        public async Task<IActionResult> ChangeActiveBoolOnProject([FromBody]int projectId)
        {
            var projectTochange = await _context.Projects.SingleOrDefaultAsync(x => x.Id == projectId);
            projectTochange.Active = !projectTochange.Active;
            if(projectTochange.Active)
            {
                // DEN HAR BLIVIT REACTIVATED, PRIO SKA BLI SÅ MÅNGA SOM ÄR AKTIVa
            }
            if(!projectTochange.Active)
            {
                // DEN HAR BLIVIT ARKIVERAD, PRIO SKA BLI LÄGST
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "Require-R-Project")]
        [HttpGet("dashboardprojects")]
        public async Task<IActionResult> GetDashboardProjects()
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);
            var topProjects = await _context.Projects.Where(x => x.OrganizationId == UsersOrgId && x.Priority <= 3).ToListAsync();
            var dashboardDto = new PMSdashboardDto();
            var projectList = new List<PMSdashboardProjectDto>();
            
            foreach(var project in topProjects)
            {
                var cardList = new List<PMSdashboardCardDto>();
                var dashboardProjectDto = new PMSdashboardProjectDto();
                dashboardProjectDto.Name = project.Name;
                dashboardProjectDto.Id = project.Id;
                dashboardProjectDto.Priority = project.Priority;

                if(await _context.TrelloLists.AnyAsync(x => x.IdBoard == project.TrelloBoardId && x.Name.Contains("Production")))
                {
                    var productionList = await _context.TrelloLists.FirstOrDefaultAsync(x => x.IdBoard == project.TrelloBoardId && x.Name.Contains("Production"));
                    dashboardProjectDto.HasProductionList = true;
                    if(await _context.TrelloCards.AnyAsync(x => x.IdBoard == project.TrelloBoardId && x.IdList == productionList.Id))
                    {
                        var cards = await _context.TrelloCards.Where(x => x.IdBoard == project.TrelloBoardId && x.IdList == productionList.Id).ToListAsync();
                        dashboardProjectDto.HasCards = true;
                        foreach(var card in cards)
                        {
                            
                            var dashboardCardDto = new PMSdashboardCardDto();
                            dashboardCardDto.Content = card.Name;
                            dashboardCardDto.Url = card.Url;

                            if(await _context.CustomFieldItems.AnyAsync(x => x.TrelloCardId == card.Id))
                            {
                                var cardCustomfield = await _context.CustomFieldItems.SingleOrDefaultAsync(x => x.TrelloCardId == card.Id);
                                if(await _context.Values.AnyAsync(x => x.CustomFieldItemId == cardCustomfield.Id))
                                {
                                    var value = await _context.Values.SingleOrDefaultAsync(x => x.CustomFieldItemId == cardCustomfield.Id);
                                    dashboardCardDto.EstimatedTime = value.Number;
                                    dashboardCardDto.HasEstimatedTime = true;
                                }
                            }
                            else
                            {
                                dashboardCardDto.EstimatedTime = 0;
                                dashboardCardDto.HasEstimatedTime = false;
                            }

                            if(await _context.UserCards.AnyAsync(x => x.TrelloCardId == card.Id))
                            {
                                var membership = await _context.UserCards.FirstOrDefaultAsync(x => x.TrelloCardId == card.Id);
                                var user = await _userManager.FindByIdAsync(membership.UserId.ToString());
                                dashboardCardDto.User = user.FullName;
                                dashboardCardDto.HasAssignedUser = true;
                            }
                            else
                            {
                                dashboardCardDto.HasAssignedUser = false;
                            }

                            cardList.Add(dashboardCardDto);
                        } 
                    }
                    else
                    {
                        dashboardProjectDto.HasCards = false;
                    }
                    
                }
                else
                {
                    dashboardProjectDto.HasProductionList = false;
                }
                dashboardProjectDto.Values = await ProjectValues(project.Id);
                dashboardProjectDto.ProductionCards = cardList;
                dashboardDto.NumberOfProjects++;
                projectList.Add(dashboardProjectDto);
            }
            dashboardDto.Projects = projectList;

            return Ok(dashboardDto);
        }

        
        public async Task<ProjectValuesDto> ProjectValues(int Id)
        {
            var projectValues = new ProjectValuesDto();
            var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id == Id);
            projectValues.HasTrello = await _context.TrelloCards.Where(x => x.IdBoard == project.TrelloBoardId).AnyAsync();

            if (projectValues.HasTrello)
            {
                var cards = await _context.TrelloCards.Where(x => x.IdBoard == project.TrelloBoardId).ToListAsync();
                var completedLists = await _context.TrelloLists.Where(x => x.IdBoard == project.TrelloBoardId && x.Name.Contains("Done")).ToListAsync();
                var productionList = await _context.TrelloLists.FirstOrDefaultAsync(x => x.IdBoard == project.TrelloBoardId && x.Name.Contains("Production"));
                var testList = await _context.TrelloLists.FirstOrDefaultAsync(x => x.IdBoard == project.TrelloBoardId && x.Name.Contains("Test"));
                var epicListExists = await _context.TrelloLists.AnyAsync(x => x.IdBoard == project.TrelloBoardId && x.Name == "Backlog");
                var containsEpicsList = await _context.TrelloLists.FirstOrDefaultAsync(x => x.IdBoard == project.TrelloBoardId && x.Name == "Backlog");
                var board = await _context.TrelloBoards.SingleOrDefaultAsync(x => x.Id == project.TrelloBoardId);

                projectValues.TrelloUrl = board.Url;
                projectValues.ProjectId = Id;
                projectValues.TrelloBoardName = board.Name;
                projectValues.EstimatedCount = 0;
                projectValues.CompletedCount = 0;
                projectValues.InTestCount = 0;
                projectValues.InProductionCount= 0;
                int cardsWithTimeCount = 0;
                try
                {
                    foreach (var card in cards)
                    {
                        if (epicListExists)
                        {

                            if (await _context.CustomFieldItems.AnyAsync(x => x.TrelloCardId == card.Id) && card.IdList != containsEpicsList.Id) //TODO: KOLLA SÅ LISTORNA FINNS
                            {
                                projectValues.EstimatedCount++;
                                cardsWithTimeCount++;
                                var customFieldItem = await _context.CustomFieldItems.SingleOrDefaultAsync(x => x.TrelloCardId == card.Id);
                                var value = await _context.Values.SingleOrDefaultAsync(x => x.CustomFieldItemId == customFieldItem.Id);
                                projectValues.EstimatedTime = projectValues.EstimatedTime + value.Number;

                                foreach (var completedList in completedLists)
                                {
                                    if (card.IdList == completedList.Id)
                                    {
                                        projectValues.CompletedCount++;
                                        projectValues.CompletedTime = projectValues.CompletedTime + value.Number;
                                    }
                                }
                                if (card.IdList == productionList.Id)
                                {
                                    projectValues.InProductionCount++;
                                    projectValues.InProductionTime = projectValues.InProductionTime + value.Number;
                                }
                                else if (card.IdList == testList.Id)
                                {
                                    projectValues.InTestCount++;
                                    projectValues.InTestTime = projectValues.InTestTime + value.Number;
                                }
                            }
                        }
                        else
                        {
                            if (await _context.CustomFieldItems.AnyAsync(x => x.TrelloCardId == card.Id))
                            {
                                projectValues.EstimatedCount++;
                                cardsWithTimeCount++;
                                var customFieldItem = await _context.CustomFieldItems.SingleOrDefaultAsync(x => x.TrelloCardId == card.Id);
                                var value = await _context.Values.SingleOrDefaultAsync(x => x.CustomFieldItemId == customFieldItem.Id);
                                projectValues.EstimatedTime = projectValues.EstimatedTime + value.Number;

                                foreach (var completedList in completedLists)
                                {
                                    if (card.IdList == completedList.Id)
                                    {
                                        projectValues.CompletedCount++;
                                        projectValues.CompletedTime = projectValues.CompletedTime + value.Number;
                                    }
                                }
                                if (card.IdList == productionList.Id)
                                {
                                    projectValues.InProductionCount++;
                                    projectValues.InProductionTime = projectValues.InProductionTime + value.Number;
                                }
                                else if (card.IdList == testList.Id)
                                {
                                    projectValues.InTestCount++;
                                    projectValues.InTestTime = projectValues.InTestTime + value.Number;
                                }
                            }

                        }
                    }
                }

                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                }

                if (cardsWithTimeCount < 1)
                {
                    projectValues.HasTrello = false;
                }
                else
                {
                    project.UsedTime = projectValues.CompletedTime;
                    project.EstimatedTime = projectValues.EstimatedTime;
                    await _context.SaveChangesAsync();
                }
            }
            return (projectValues);
        }
        
    }
}