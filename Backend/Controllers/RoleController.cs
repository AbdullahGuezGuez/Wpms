
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Backend.Dtos;
using System;
using System.Security.Claims;

namespace Backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(DataContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this._context = context;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }


        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet ("getRoles")]
        public async Task<IActionResult> getRoles() {
       
            if(User.Identity.Name.ToUpper() == "SYSTEMADMIN")
             {
                var roles = await _context.Roles.ToListAsync();
                return Ok(roles);
             }
             else
             {
                var orgId = User.FindFirst("Organization").Value;
                var roles = await _context.Roles.Where(x => x.OrganizationId.ToString() == orgId).ToListAsync();
                return Ok(roles);
             }
            
        }

        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet ("claims")]
        public async Task<IActionResult> GetAllClaims()
        {
            var claims = await _context.Claims.ToListAsync();
            return Ok(claims);
        }


        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet ("roleclaims")]
        public async Task<IActionResult> GetAllRoleClaims()
        {
            var orgId = User.FindFirst("Organization").Value;
            var orgRoles = await _context.Roles.Where(x => x.OrganizationId.ToString() == orgId).ToListAsync();
            var rolesWithClaims = new List<RoleWithClaimsDto>();
            var allClaims = await _context.Claims.ToListAsync();

            foreach(var role in orgRoles)
            {
                var roleWithClaim = new RoleWithClaimsDto{Name = role.Name ,Id = role.Id, OrganizationId = role.OrganizationId};
                roleWithClaim.Claims = await _roleManager.GetClaimsAsync(role);
                var claimsDtos = new List<ClaimDto>();

                foreach( var claim in allClaims)
                {
                    var claimDto = new ClaimDto();
                    claimDto.ClaimType = claim.ClaimType;
                    claimDto.ClaimValue = claim.ClaimValue;
                    var found = false;

                    foreach(var roleClaim in roleWithClaim.Claims)
                    {
                        if(claim.ClaimValue == roleClaim.Value)
                        {
                            claimDto.HasClaim = true;
                            claimsDtos.Add(claimDto);
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        claimDto.HasClaim = false;
                        claimsDtos.Add(claimDto);
                    }
                }
                roleWithClaim.ClaimsWithBool = claimsDtos;
                rolesWithClaims.Add(roleWithClaim);
            }
            
            return Ok(rolesWithClaims);
        }


        [HttpGet ("role")]
        public async Task<IActionResult> GetAllRoles()
        {
             if(User.Identity.Name.ToUpper() == "SYSTEMADMIN")
             {
                var roles = await _context.Roles.ToListAsync();
                
                return Ok(roles);
             }
             else
             {
                var orgId = User.FindFirst("Organization").Value;
                var roles = await _context.Roles.Where(x => x.OrganizationId.ToString() == orgId).ToListAsync();
                
                return Ok(roles);
             }
            
        }

        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet ("roleusers/{roleId}")]
        public async Task<IActionResult> GetAllRoleUsers(int roleId)
        {
            var orgId = User.FindFirst("Organization").Value;
            var users = await _context.Users.FromSql("SELECT * FROM ASPNETUSERS WHERE AspNetUsers.Id IN (SELECT AspNetUserRoles.UserId FROM AspNetUserRoles WHERE AspNetUsers.Masked = false AND AspNetUserRoles.RoleId =" + roleId + ")").ToListAsync();
            
            return Ok(users);
        }


        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet("rolewithclaims")]
        public async Task<IActionResult> getAllRolesWithModules() {

            var orgId = User.FindFirst("Organization").Value;
            var roles = await _context.Roles.Where(x => x.OrganizationId.ToString() == orgId).ToListAsync();
            var rolesWithClaims = new List<CustomRoleWithClaimsDto>();
            foreach(var role in roles)
            {
                var CustomRole = new CustomRoleWithClaimsDto(){Name = role.Name, Id = role.Id};
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                CustomRole.Claims = roleClaims.ToList();
                rolesWithClaims.Add(CustomRole);
            }
            return Ok(rolesWithClaims);
        }

        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            if(User.Identity.Name.ToUpper() == "SYSTEMADMIN")
            {

            var users = await _context.Users.Where(x => x.NormalizedUserName != "SYSTEMADMIN" && x.Masked == false).ToListAsync();
            var userList = new List<UserWithRolesDto>();
                
            foreach(var user in users)
                {
                    var userWithRole = new UserWithRolesDto();
                    userWithRole.Username = user.UserName;
                    userWithRole.FullName = user.FullName;
                    userWithRole.Initials = user.Initials;
                    userWithRole.Id = user.Id;
                    userWithRole.Roles = await _context.Roles.FromSql("SELECT * FROM AspNetRoles WHERE AspNetRoles.Id IN (SELECT AspNetUserRoles.RoleId FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = {0})",user.Id).ToListAsync();
                    
                    userList.Add(userWithRole);
                }

                return Ok(userList);
            }
            else
            {
                var orgId = int.Parse(User.FindFirst("Organization").Value);
                var users = await _context.Users.FromSql("SELECT * FROM AspNetUsers WHERE AspNetUsers.Masked = false AND AspNetUsers.Id IN (SELECT OrganizationUser.UserId FROM OrganizationUser WHERE OrganizationUser.OrganizationId = {0})",orgId).ToListAsync();
                var userList = new List<UserWithRolesDto>();
                
                foreach(var user in users)
                {
                    var userWithRole = new UserWithRolesDto();
                    userWithRole.Username = user.UserName;
                    userWithRole.Id = user.Id;
                    
                    userWithRole.FullName = user.FullName;
                    userWithRole.Initials = user.Initials;
                    userWithRole.Roles = await _context.Roles.FromSql("SELECT * FROM AspNetRoles WHERE AspNetRoles.Id IN (SELECT AspNetUserRoles.RoleId FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = {0}) AND AspNetRoles.OrganizationId = {1}",user.Id,orgId).ToListAsync();
                    
                    userList.Add(userWithRole);
                }

                return Ok(userList);
            }
            
            
        }


        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpPut ("roleclaims")]
        public async Task<IActionResult> ChangeClaimsOnRole([FromBody]RoleWithClaimsToUpdateDto role)
        {
            
            var roleToChange = await _roleManager.FindByIdAsync(role.Id.ToString());
            var allClaims = await _context.Claims.ToListAsync();
            var exClaims = await _roleManager.GetClaimsAsync(roleToChange);
           
            foreach(var claim in role.ClaimsWithBool)
            {
                if(claim.HasClaim)
                {
                    var found = false;
                    foreach(var exClaim in exClaims)
                    {
                        if(exClaim.Value == claim.ClaimValue) 
                        {
                            found = true;
                        }
                    }
                    if(!found)
                    {
                        await _roleManager.AddClaimAsync(roleToChange, new Claim(claim.ClaimType, claim.ClaimValue));
                    }
                }
                else
                {
                    var found = false;
                    foreach(var exClaim in exClaims)
                    {
                        if(exClaim.Value == claim.ClaimValue) 
                        {
                            found = true;
                        }  
                    }
                    if(found)
                    {
                        await _roleManager.RemoveClaimAsync(roleToChange, exClaims.SingleOrDefault(x => x.Value == claim.ClaimValue));
                    }
                }
            }
            return Ok(role);
        }

        [Authorize(Policy = "Require-RU-UserToRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = new List<string>();
            if(User.Identity.Name.ToUpper() == "SYSTEMADMIN")
            {
               userRoles = (List<string>)await _userManager.GetRolesAsync(user); 
            }
            else
            {
                var orgId = int.Parse(User.FindFirst("Organization").Value);
                var roles = await _context.Roles.FromSql("SELECT * FROM AspNetRoles WHERE AspNetRoles.Id IN (SELECT AspNetUserRoles.RoleId FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = {0}) AND AspNetRoles.OrganizationId = {1}",user.Id,orgId).ToListAsync();
                foreach(var role in roles)
                {
                    userRoles.Add(role.Name);
                }
            }

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to remove the role");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpPost("role")]
        public async Task<IActionResult> CreateUserRole(CreateRoleDto roleToCreate)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleToCreate.RoleName);
            
            if (!roleExists)
            {
                var role = new Role();
                var orgId = User.FindFirst("Organization").Value;
                role.OrganizationId = int.Parse(orgId);
                role.Name = roleToCreate.RoleName;
                var result = await _roleManager.CreateAsync(role);
            }       
            return Ok();    
        }

        [Authorize(Policy = "Require-CRUD-Role")]
        [HttpDelete("role/{id}")]
        public async Task<IActionResult> deleteRole(int id) 
        {
            var roleToDelete = await this._context.Roles.FirstOrDefaultAsync(x => x.Id == id);

            var usersInRole = await _context.UserRoles.AnyAsync(x => x.RoleId == roleToDelete.Id);

            if(!usersInRole)
            {
                if(roleToDelete.Name != "SystemAdmin")
                {
                    await _roleManager.DeleteAsync(roleToDelete);
                    return Ok(roleToDelete);
                }
            }
            
            return Ok();
        }

    }
}