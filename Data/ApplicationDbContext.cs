using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using dotnetapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace dotnetapp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext (DbContextOptions <ApplicationDbContext> options) : base (options) { }
        public DbSet <User> Users { get; set; }
        public DbSet <Loan> Loans { get; set; }
        public DbSet <LoanApplication> LoanApplications { get; set; }
        public DbSet <Feedback> Feedbacks { get; set; }
    }
}