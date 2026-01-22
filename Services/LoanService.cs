using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Data;
using dotnetapp.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace dotnetapp.Services
{
  public class LoanService
  {
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan LoanCacheDuration = TimeSpan.FromMinutes(5);

    public LoanService (ApplicationDbContext context, IMemoryCache cache)
    {
      _context = context;
      _cache = cache;
    }

    public async Task<IEnumerable<Loan>> GetAllLoans ()
    {
      var loans = await _context.Loans.AsNoTracking().ToListAsync();
      return loans;
    }

    public async Task<Loan> GetLoanById (int loanId)
    {
      var cacheKey = $"Loan_{loanId}";
      if (_cache.TryGetValue<Loan>(cacheKey, out var cached))
      {
        return cached;
      }

      var loan = await _context.Loans.AsNoTracking().FirstOrDefaultAsync (l => l.LoanId == loanId);
      if (loan != null)
      {
        _cache.Set(cacheKey, loan, LoanCacheDuration);
      }
      return loan;
    }

    public async Task<bool> AddLoan (Loan loan)
    {
      var extLoan = await _context.Loans.FirstOrDefaultAsync (l => l.LoanType == loan.LoanType);
      if(extLoan == null)
      {
        await _context.Loans.AddAsync (loan);
        await _context.SaveChangesAsync ();
        return true;
      }
      else
      {
        throw new LoanException ("Loan with the same type already exists");
      }
    }

    public async Task<bool> UpdateLoan (int loanId, Loan loan)
    {
      Loan extLoan = await _context.Loans.FirstOrDefaultAsync (l => l.LoanId == loanId);
      if (extLoan != null)
      {
        extLoan.LoanType = loan.LoanType;
        extLoan.Description = loan.Description;
        extLoan.InterestRate = loan.InterestRate;
        extLoan.MaximumAmount = loan.MaximumAmount;
        await _context.SaveChangesAsync ();

        // invalidate cache for this loan
        var cacheKey = $"Loan_{loanId}";
        _cache.Remove(cacheKey);

        return true;
      }
      else
      {
        return false;
      }
    }

    public async Task<bool> DeleteLoan (int loanId)
    {
      var loan = await _context.Loans.FirstOrDefaultAsync (l => l.LoanId == loanId);
      var reference = await _context.LoanApplications.FirstOrDefaultAsync (l => l.LoanId == loanId);
      if (loan != null)
      {
        if(reference == null)
        {
          _context.Loans.Remove (loan);
          await _context.SaveChangesAsync ();

          var cacheKey = $"Loan_{loanId}";
          _cache.Remove(cacheKey);

          return true;
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }
  }
}
