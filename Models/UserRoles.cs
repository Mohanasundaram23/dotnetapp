using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace dotnetapp.Models
{
    public class UserRoles : IdentityRole
    {
        public static string Admin = "Admin";
        public static string User = "User";
    }
}

//Authorize[Role="UserRoles.Admin,UserRoles.User"]
//[Authorize] Controller
//[Authorize (Roles="Admin,User")] Method
//[Authorize (Roles="Admin")] Method
//[Authorize (Roles="User")] Method
