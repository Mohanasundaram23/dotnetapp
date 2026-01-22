using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using dotnetapp.Data;
using dotnetapp.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace dotnetapp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager <ApplicationUser> userManager;
        private readonly RoleManager <IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        //public static UserRoles UserRoles;
        public AuthService (UserManager <ApplicationUser> userManager, RoleManager <IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }
        public async Task <(int, string)> Registration (User model, string role)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Name = "hhh",
                Email = model.Email,
                UserName = model.Username,
                PhoneNumber = model.MobileNumber
            };
            var exist = await userManager.FindByEmailAsync (model.Email);
            if (exist != null) return (0, "User already exist");
            if (UserRoles.Admin == role || UserRoles.User == role)
            {
                var result = await userManager.CreateAsync (user, model.Password);
                if (result.Succeeded)
                {
                    await _context.Users.AddAsync (model);
                    await _context.SaveChangesAsync ();
                    if (!(await roleManager.RoleExistsAsync (role)))
                    {
                        await roleManager.CreateAsync (new IdentityRole(role));
                    }
                    await userManager.AddToRoleAsync (user, role);
                    return (1, "User created successfully!");
                }
                return (0, "User creation failed! Please check user details and try again.");
            }
            return (0, "User creation failed!`");
        }
        public async Task <(int, string)> Login (LoginModel model)
        {
            var exist = await userManager.FindByEmailAsync (model.Email);
            if (exist == null) return (0, "Invalid email");
            var user = await _context.Users.FirstOrDefaultAsync (u => u.Email == model.Email);
            var result = await userManager.CheckPasswordAsync (exist, model.Password);
            if (result)
            {
                // var claims = new []
                // {
                //     new Claim("role", user.UserRole),
                //     //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                //     new Claim("id",user.UserId)
                // };
                var claims = new List<Claim>
                {
                    new Claim ("role", user.UserRole),//ClaimTypes.Role
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    new Claim ("id", user.UserId.ToString()),
                    new Claim ("username", user.Username)
                };

                
                string token = GenerateToken (claims);
                return (1, token);
            }
            return (0, "Invalid password");
        }
        public async Task <User> GetUserByIdAsync (int userId)
        {
            return await _context.Users.FindAsync (userId);
        }
        private string GenerateToken (IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(_configuration ["Jwt:Key"]));
            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken (_configuration["Jwt:Issuer"],
                _configuration ["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes (30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler ().WriteToken (token);
        }
    }
}