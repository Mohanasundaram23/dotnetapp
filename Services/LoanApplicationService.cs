using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Exceptions;

namespace dotnetapp.Services
{
    public class LoanApplicationService
    {
        private readonly ApplicationDbContext _context;
        public LoanApplicationService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LoanApplication>> GetAllLoanApplications ()
        {
            var loanApplications = await _context.LoanApplications
                .AsNoTracking()
                .Include(c=>c.Loan)
                .Include(d=>d.User)
                .ToListAsync();
            return loanApplications;
        }
        public async Task<IEnumerable<LoanApplication>> GetLoanApplicationsByUserId (int userId)
        {
            var loanApplication = await _context.LoanApplications
                .AsNoTracking()
                .Include(c=>c.Loan)
                .Include(d=>d.User)
                .Where(d => d.UserId == userId)
                .ToListAsync();
            return loanApplication;
        }
        public async Task<bool> AddLoanApplication (LoanApplication loanApplication)
        {
            var application = await _context.LoanApplications.FirstOrDefaultAsync (c => c.UserId == loanApplication.UserId && c.LoanId == loanApplication.LoanId);
            if (application != null)
            {
                return false;
            }
            else
            {
                await _context.LoanApplications.AddAsync (loanApplication);
                await _context.SaveChangesAsync ();
                return true;
            }
        }

        public async Task<bool> UpdateLoanApplication (int loanApplicationId, LoanApplication loanApplication)
        {
            var application = await _context.LoanApplications.FirstOrDefaultAsync (e => e.LoanApplicationId == loanApplicationId);
            if (application == null)
            {
                return false;
            }
            else
            {
                application.LoanStatus = loanApplication.LoanStatus;
                application.Address = loanApplication.Address;
                await _context.SaveChangesAsync ();
                return true;
            }
        }

        public async Task<bool> DeleteLoanApplication (int loanApplicationId)
        {
            var application = await _context.LoanApplications.FirstOrDefaultAsync (c => c.LoanApplicationId == loanApplicationId);
            if (application == null)
            {
                return false;
            }
            else
            {
                _context.LoanApplications.Remove (application);
                await _context.SaveChangesAsync ();
                return true;
            }
        }
    }
}