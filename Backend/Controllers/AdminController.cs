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
    public class AdminController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public AdminController(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        [Authorize(Policy = "Require-C-Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            
            var orgId = userForRegisterDto.OrganizationId;
            var userName = User.Identity.Name;
            if (userName != "SystemAdmin")
            {   
                orgId = int.Parse(User.FindFirst("Organization").Value);
            }

            if(await _context.Users.AnyAsync(x => x.UserName.ToUpper() == userForRegisterDto.Username.ToUpper()))
            {
                return BadRequest("Username taken");
            }
            else
            {
                var userToCreate = _mapper.Map<User>(userForRegisterDto);
                var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

                var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
                var createdUser = await _userManager.FindByNameAsync(userToCreate.UserName);
                createdUser.Initials = getInitials(createdUser.FullName);
                createdUser.Masked = false;
                var result2 = await _userManager.AddToRoleAsync(createdUser, userForRegisterDto.Roles);
                var organizationUser = new OrganizationUser { OrganizationId = orgId, UserId = createdUser.Id };
                await _context.OrganizationUser.AddAsync(organizationUser);
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    return Ok("Success");
                }

                return BadRequest("Something went wrong"); 
            }
        }

        [Authorize(Policy = "Require-C-Admin")]
        [HttpPut("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]int id)
        {
            var user = await _context.Users.SingleAsync(x => x.Id == id);

            if(User.Identity.Name.ToUpper() == "SYSTEMADMIN")
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, "password");
                return Ok();
            }

            return BadRequest("You cant change someone elses password");
        }

        
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordValues)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            bool correctPassword = await _userManager.CheckPasswordAsync(user, changePasswordValues.oldPassword);
            if(correctPassword)
            {
                await _userManager.ChangePasswordAsync(user, changePasswordValues.oldPassword, changePasswordValues.newPassword);
                return Ok("Success");
            }
            else
            {
                return BadRequest("Wrong Current password");
            }
        }

        [Authorize(Policy = "Require-C-Admin")]
        [HttpPut("maskuser")]
        public async Task<IActionResult> MaskUser([FromBody]int id)
        {
            Random nr = new Random();
            var user = await _userManager.FindByIdAsync(id.ToString());
            var initials = user.Initials.Split(".");
            user.Masked = true;
            user.FullName = initials[0]+"xxxx "+initials[1]+"xxxx";
            user.UserName = "RemovedUser" + nr.Next(99999);
            user.NormalizedUserName = user.UserName.ToUpper();
            user.PhoneNumber = "0000000";
            user.PasswordHash = "AL3Pg3s3j52kjb2kd2s1";
            user.Email = "deleted";
            user.NormalizedEmail = "DELETED";
            await _context.SaveChangesAsync();
            return Ok();
        }

        #region Organization

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpGet("organization")]
        public async Task<IActionResult> GetallOrganizations()
        {
            var organizations = await _context.Organizations.ToListAsync();
            return Ok(organizations);
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpGet("organization/{id}")]
        public async Task<IActionResult> GetOrganization(int id)
        {
            var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(organization);
        }

        [AllowAnonymous]
        [HttpGet("organizationbyuser/{name}")]
        public async Task<IActionResult> GetAllUsersOrganizationsWithName(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            var usersOrganizations = await _context.Organizations.FromSql
            ("SELECT * FROM Organizations JOIN OrganizationUser ON Organizations.Id = OrganizationUser.OrganizationId WHERE OrganizationUser.UserId = {0}", user.Id.ToString()).ToListAsync();
            return Ok(usersOrganizations);
        }

        [AllowAnonymous]
        [HttpGet("organizationbyuserid/{userId}")]
        public async Task<IActionResult> GetAllUsersOrganizationsWithId(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var usersOrganizations = _context.Organizations.FromSql("SELECT * FROM Organizations JOIN OrganizationUser ON Organizations.Id = OrganizationUser.OrganizationId WHERE OrganizationUser.UserId = " + user.Id.ToString()).ToList();
            return Ok(usersOrganizations);
        }

        
        [HttpGet("organizationbyusertoken")]
        public async Task<IActionResult> GetAllUsersOrganizationsWithToken()
        {
            var username = User.Identity.Name;
            var org = User.FindFirst("Organization").Value;

            var usersOrganizations = await _context.Organizations.FromSql
            ("SELECT * FROM Organizations WHERE Organizations.Id IN (SELECT OrganizationId FROM OrganizationUser WHERE OrganizationUser.UserId = (SELECT AspNetUsers.Id FROM AspNetUsers WHERE NormalizedUserName = '" + username.ToUpper() + "')) AND Organizations.Id != " + org).ToListAsync();
            return Ok(usersOrganizations);
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpPost("organization")]
        public async Task<IActionResult> CreateOrganization([FromBody]Organization newOrganization)
        {
            await this._context.Organizations.AddAsync(newOrganization);
            _context.SaveChanges();

            var organization = await _context.Organizations.SingleOrDefaultAsync(x => x.Name == newOrganization.Name);

            var roles = new List<Role>
                    {
                        new Role{Name = "Member"},
                        new Role{Name = "PMS-Admin"},
                        new Role{Name = "CRM-Admin"},
                    };

            foreach (var role in roles)
            {
                if (role.Name == "Member")
                {
                    role.Name = organization.Name + "-Member";
                    role.NormalizedName = organization.Name + "-Member";
                }
                if (role.Name == "PMS-Admin")
                {
                    role.Name = organization.Name + "-PMS-Admin";
                    role.NormalizedName = organization.Name + "-PMS-Admin";
                }
                if (role.Name == "CRM-Admin")
                {
                    role.Name = organization.Name + "-CRM-Admin";
                    role.NormalizedName = organization.Name + "-CRM-Admin";
                }

                role.OrganizationId = organization.Id;
                _roleManager.CreateAsync(role).Wait();

                //! ALLAS CLAIMS
                await _roleManager.AddClaimAsync(role, new Claim("R-Project", "See All Projects"));
                await _roleManager.AddClaimAsync(role, new Claim("R-AssignedProject", "See assigned Users in projects"));
                await _roleManager.AddClaimAsync(role, new Claim("R-Customer", "See All Customers"));
                await _roleManager.AddClaimAsync(role, new Claim("R-Contactperson", "See All Contactpersons"));
                await _roleManager.AddClaimAsync(role, new Claim("R-Activity", "See All Activities"));
                await _roleManager.AddClaimAsync(role, new Claim("CU-Activity", "Create/Update Activities"));

                if (role.Name.Contains("PMS"))
                {
                    //! BARA PMS-ADMIN CLAIMS
                    await _roleManager.AddClaimAsync(role, new Claim("CU-Project", "Create/Update Projects"));
                    await _roleManager.AddClaimAsync(role, new Claim("C-User", "Create Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("C-Admin", "Create Admins"));
                    await _roleManager.AddClaimAsync(role, new Claim("D-User", "Delete Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("U-User", "Update Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("U-AssingedMembers", "Assign and Remove Users from projects"));
                    await _roleManager.AddClaimAsync(role, new Claim("CRUD-Role", "Create/Update/Delete Roles"));
                    await _roleManager.AddClaimAsync(role, new Claim("RU-UserToRole", "Change Roles on Users"));
                }
                if (role.Name.Contains("CRM"))
                {
                    //! BARA CRM-ADMIN CLAIMS
                    await _roleManager.AddClaimAsync(role, new Claim("CU-Project", "Create/Update Projects"));
                    await _roleManager.AddClaimAsync(role, new Claim("C-User", "Create Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("C-Admin", "Create Admins"));
                    await _roleManager.AddClaimAsync(role, new Claim("D-User", "Delete Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("U-User", "Update Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("CRUD-Role", "Create/Update/Delete Roles"));
                    await _roleManager.AddClaimAsync(role, new Claim("RU-UserToRole", "Change Roles on Users"));
                    await _roleManager.AddClaimAsync(role, new Claim("CUD-Customer", "Create/Update/Delete Customers"));
                    await _roleManager.AddClaimAsync(role, new Claim("CUD-Contactperson", "Create/Update/Delete Contactpersons"));
                    await _roleManager.AddClaimAsync(role, new Claim("U-AssingedMembers", "Assign and Remove Users from projects"));
                    await _roleManager.AddClaimAsync(role, new Claim("D-Activity", "Delete Activities"));
                }
            }

            await _context.SaveChangesAsync();

            return Ok(organization);
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpPut("organization/{id}")]
        public async Task<IActionResult> UpdateOrganization([FromBody]Organization organization, int id)
        {
            var organizationToUpdate = await this._context.Organizations.FirstOrDefaultAsync(x => x.Id == id);

            if (organization.Name != null)
            {
                organizationToUpdate.Name = organization.Name;
            }
            if (organization.Trellokey != null)
            {
                organizationToUpdate.Trellokey = organization.Trellokey;
            }
            if (organization.Trellotoken != null)
            {
                organizationToUpdate.Trellotoken = organization.Trellotoken;
            }
            if (organization.TrelloTeamName != null)
            {
                organizationToUpdate.TrelloTeamName = organization.TrelloTeamName;
            }

            _context.SaveChanges();

            return Ok(organizationToUpdate);
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpDelete("organization/{id}")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {

            var organizationToDelete = await this._context.Organizations.FirstOrDefaultAsync(x => x.Id == id);
            _context.Organizations.Remove(organizationToDelete);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpDelete("organizationuserdelete")]              //TODO: MULTIPLEORGANIZATION
        public async Task<IActionResult> DeleteOrganizationUser(OrganizationUser organizationUser)
        {
            _context.OrganizationUser.Remove(organizationUser);
            await _context.SaveChangesAsync();

            return Ok(organizationUser);
        }

        [Authorize(Policy = "Require-CRUD-Organization")]
        [HttpPost("organizationuser")]
        public async Task<IActionResult> CreateOrganizationUser(OrganizationUser organizationUser)
        {
            await _context.OrganizationUser.AddAsync(organizationUser);
            await _context.SaveChangesAsync();
            return Ok(organizationUser);
        }

#endregion

        [HttpPut("usersorganizations")]
        public async Task<IActionResult> UpdateUsersOrganizations([FromBody]int userId)
        {


            return Ok();
        }

        public static string getInitials(string name)
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