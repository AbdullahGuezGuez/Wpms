using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _authRepository;
        private readonly DataContext _context;

        public AuthController(IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, IAuthRepository authRepository, IMapper mapper, DataContext context, RoleManager<Role> roleManager)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
            _config = config;
            _authRepository = authRepository;
            _roleManager = roleManager;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("firstlogin")]
        public async Task<IActionResult> FirstLogin(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            var roles = _userManager.GetRolesAsync(user);
            
            if (result.Succeeded && roles.Result.Count > 0)
            {
                var numOrganizations = await _context.Organizations.FromSql
                ("SELECT * FROM Organizations JOIN OrganizationUser ON Organizations.Id = OrganizationUser.OrganizationId WHERE OrganizationUser.UserId =" + user.Id).ToListAsync();

                if (numOrganizations.Count == 0)
                {
                    if (user.UserName.ToUpper() == "SYSTEMADMIN")
                    {
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

                    var userToReturn = _mapper.Map<UserForListDto>(appUser);
                    var token = GenerateJwtToken(appUser);
                    token.Wait();

                    return Ok(new
                    {
                        token = token.Result,
                        user = userToReturn.Username,
                        userRoles = roles.Result

                    });
                    }
                    else
                    {
                        return Unauthorized("This user doesn't seem to belong to a organization?");
                    }
                }
                else if (numOrganizations.Count == 1)
                {
                    //Continue login
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                    var orgId = numOrganizations.ToArray()[0].Id;
                    var userToReturn = _mapper.Map<UserForListDto>(appUser);
                    var token = GenerateJwtToken(appUser, orgId);
                    token.Wait();

                    return Ok(new
                    {
                        token = token.Result,
                        user = userToReturn.Username,
                        userRoles = roles.Result
                    });
                }
                else if (numOrganizations.Count > 1)
                {
                    //User choose organization
                    return Ok("MULTIPLEORGANIZATIONS");
                }
            }


            return Ok();
        }

        [HttpPost("loginorganization/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOrganization(UserForLoginDto userForLoginDto, int id)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            var roles = _userManager.GetRolesAsync(user);

            if (result.Succeeded && roles.Result.Count > 0)
            {
                var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                var userToReturn = _mapper.Map<UserForListDto>(appUser);
                var token = GenerateJwtToken(appUser, id);
                token.Wait();

                return Ok(new
                {
                    token = token.Result,
                    user = userToReturn.Username,
                    userRoles = roles.Result
                });

            }

            return Unauthorized();
        }

        [HttpGet("changeorganizationtoken/{orgId}")]
        public async Task<IActionResult> LoginOrganizationWithToken(int orgId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var roles = _userManager.GetRolesAsync(user);

            if (roles.Result.Count > 0)
            {
                var token = GenerateJwtToken(user, orgId);
                token.Wait();

                return Ok(new
                {
                    token = token.Result,
                    user = user.UserName,
                    userRoles = roles.Result
                });

            }

            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            var roles = _userManager.GetRolesAsync(user);

            if (result.Succeeded && roles.Result.Count > 0)
            {
                var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

                var userToReturn = _mapper.Map<UserForListDto>(appUser);
                var token = GenerateJwtToken(appUser);
                token.Wait();

                return Ok(new
                {
                    token = token.Result,
                    user = userToReturn.Username,
                    userRoles = roles.Result
                });

            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));


            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                var realRole = await _roleManager.FindByNameAsync(role);
                var roleClaims = await _roleManager.GetClaimsAsync(realRole);
                    foreach (var rc in roleClaims)
                    {
                        claims.Add(rc);
                    }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        private async Task<string> GenerateJwtToken(User user, int OrgId) 
        {

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Organization", OrgId.ToString()));
            claims.Add(new Claim("UsersId", user.Id.ToString()));


            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                var roleToCheck = await _roleManager.FindByNameAsync(role);
                
                if(roleToCheck.OrganizationId == OrgId)
                {
                    var realRole = await _roleManager.FindByNameAsync(role);
                    claims.Add(new Claim(ClaimTypes.Role, role));
                    claims.Add(new Claim("Rolename", role));
                    var roleClaims = await _roleManager.GetClaimsAsync(realRole);
                    foreach (var rc in roleClaims)
                    {
                        claims.Add(rc);
                    }
                }
                
                
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        [HttpGet("user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userName = User.Identity.Name;
            var thisUser = await _userManager.FindByNameAsync(userName);
            return Ok(thisUser);
        }

        [HttpGet("loggedinuser")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            var UsersId = int.Parse(User.FindFirst("UsersId").Value);
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == UsersId);
            
            var userDto = _mapper.Map<LoggedInUserDto>(user);

            return Ok(userDto);
        }

        [HttpGet("loggedinuserroles")]
        public async Task<IActionResult> GetLoggedInUsersRoles()
        {
            var usersRoles = User.FindFirst("Rolename").Value;
            var role = new RoleToSendDto();
            if(usersRoles.Contains("CRM-Admin"))
            {
                role.Name = "CRM";
                //return Ok("CRM");
            }
            else if(usersRoles.Contains("PMS-Admin"))
            {
                role.Name = "PMS";
                //return Ok("PMS");
            }
            else
            {
                role.Name = "Member";
                //return Ok("Member");
            }
            return Ok(role);
        }
        
        [HttpGet("issystemadmin")]
        public IActionResult SystemAdminCheck()
        {
            int isSystemAdmin = 0;
            var userName = User.Identity.Name.ToUpper();
            if(userName == "SYSTEMADMIN")
            {
                isSystemAdmin = 1;
                return Ok(isSystemAdmin);
            }
            else
            {
                return Ok(isSystemAdmin);
            }
        }
    }
}