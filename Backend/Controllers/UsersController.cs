using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using Backend.Models;
using System.Linq;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        

        public UsersController(IMapper mapper,DataContext context, UserManager<User> usermanager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = usermanager;
        }


        [HttpGet("allusersinorganization")]
        public async Task<IActionResult> GetUsersInOrganization() 
        {
            var UsersOrgId = int.Parse(User.FindFirst("Organization").Value);

            var users = await _context.Users
            .FromSql("SELECT * FROM AspNetUsers JOIN OrganizationUser ON OrganizationUser.UserId == AspNetUsers.Id WHERE AspNetUsers.Masked = false AND OrganizationUser.OrganizationId = {0}", UsersOrgId).ToListAsync();

            return Ok(users);
        }  

        
        
    }
}