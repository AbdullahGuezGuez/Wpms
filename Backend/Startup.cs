using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            }
            );

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>    
                { 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                    GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireSystemAdminRole", policy => policy.RequireRole("SystemAdmin"));
                options.AddPolicy("RequireMemberRole", policy => policy.RequireRole("Member","SystemAdmin","ProjectAdmin","CostumerAdmin"));
                options.AddPolicy("RequireCostumerAdminRole", policy => policy.RequireRole("CostumerAdmin", "SystemAdmin"));
                options.AddPolicy("RequireProjectAdminRole", policy => policy.RequireRole("ProjectAdmin", "SystemAdmin", "CostumerAdmin"));

                //! DEM NEDANFÖR ÄR NYA OCH OVANFÖR SKALL TAS BORT              /\

                // SYSTEMADMIN
                options.AddPolicy("Require-CRUD-Organization", policy => policy.RequireRole("SystemAdmin"));
                options.AddPolicy("Require-U-Admin", policy => policy.RequireRole("SystemAdmin"));

                // CLAIMS
                options.AddPolicy("Require-R-Project", policy => policy.RequireClaim("R-Project"));
                options.AddPolicy("Require-CU-Project", policy => policy.RequireClaim("CU-Project"));
                options.AddPolicy("Require-C-User", policy => policy.RequireClaim("C-User"));
                options.AddPolicy("Require-C-Admin", policy => policy.RequireClaim("C-Admin"));
                options.AddPolicy("Require-D-User", policy => policy.RequireClaim("D-User"));
                options.AddPolicy("Require-U-User", policy => policy.RequireClaim("U-User"));
                options.AddPolicy("Require-CRUD-Role", policy => policy.RequireClaim("CRUD-Role"));
                options.AddPolicy("Require-RU-UserToRole", policy => policy.RequireClaim("RU-UserToRole"));
                options.AddPolicy("Require-R-AssignedProject", policy => policy.RequireClaim("R-AssignedProject"));
                options.AddPolicy("Require-U-AssignedMembers", policy => policy.RequireClaim("U-AssignedMembers"));
                options.AddPolicy("Require-CUD-Customer", policy => policy.RequireClaim("CUD-Customer"));
                options.AddPolicy("Require-R-Customer", policy => policy.RequireClaim("R-Customer"));
                options.AddPolicy("Require-CUD-Contactperson", policy => policy.RequireClaim("CUD-Contactperson"));
                options.AddPolicy("Require-R-Contactperson", policy => policy.RequireClaim("R-Contactperson"));
                options.AddPolicy("Require-CU-Activity", policy => policy.RequireClaim("CU-Activity"));
                options.AddPolicy("Require-R-Activity", policy => policy.RequireClaim("R-Activity"));
                options.AddPolicy("Require-D-Activity", policy => policy.RequireClaim("D-Activity"));

            });

            services.AddMvc(options => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutoMapper();
            services.AddCors();
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IWpmsRepository, WpmsRepository>();
            services.AddTransient<Seed>();
            // services.AddHostedService<TimedHostedService>(); // Timer för arkivering av aktiviteter, lär inte implementeras

        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               //TA BORT KOMMENTAR SENARE !!!!! ÅTERSTÄLL --> app.UseHsts();
            }

            seeder.SeedUsers();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
           // TA BORT KOMMENTAR SENARE !!!!! ÅTERSTÄLL --> app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
