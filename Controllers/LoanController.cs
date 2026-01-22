using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
 
namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly LoanService _loanService;
        public LoanController (LoanService loanService)
        {
            _loanService=loanService;
        }
        [HttpGet ("/api/loan")]
        [Authorize (Roles="Admin,User")]
        public async Task <ActionResult <IEnumerable <Loan>>> GetAllLoans ()
        {
            try
            {
                IEnumerable <Loan> loans = await _loanService.GetAllLoans();
                if (loans != null)
                {
                    return Ok (loans);
                }
                else
                {
                    throw new LoanException ("");
                }
            }
            catch (LoanException ex)
            {
               return StatusCode (500, ex.Message);
            }
           
        }
        [HttpGet ("/api/loan/{loanId}")]
        [Authorize (Roles="Admin,User")]
        public async Task <ActionResult <Loan>> GetLoanById (int loanId)
        {
            try
            {
                var loan = await _loanService.GetLoanById (loanId);
                if(loan != null)
                {
                    return Ok (loan);
                }
                else
                {
                    return NotFound ("Cannot find any loan");
                }
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }
 
        [HttpPost ("/api/loan")]
       [Authorize (Roles="Admin")]
        public async Task <ActionResult> AddLoan ([FromBody] Loan loan)
        {
           try
           {
                if (ModelState.IsValid)
                {
                    bool result = await _loanService.AddLoan (loan);
                    if (result)
                    {
                        return Ok ("Loan added successfully");
                    }
                    else
                    {
                        throw new LoanException ("Loan with the same type already exists");
                    }
                }
                else
                {
                    return BadRequest ();
                }
           }
           catch (LoanException ex)
           {
              return StatusCode (500, ex.Message);
           }
        }
 
        [HttpPut ("/api/loan/{loanId}")]
       [Authorize (Roles="Admin")]
        public async Task <ActionResult> UpdateLoan (int loanId, [FromBody] Loan loan)
        {
            Console.WriteLine(loan);
            try
            {
                if(ModelState.IsValid)
                {
                    bool result = await _loanService.UpdateLoan (loanId, loan);
                    if (result)
                    {
                        return Ok ("Loan updated successfully");
                    }
                    else
                    {
                        throw new LoanException ("Loan with the same type already exist");
                    }
                }
                else
                {
                    return NotFound ();
                }
            }
            catch(LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }
 
        [HttpDelete ("/api/loan/{loanId}")]
        [Authorize (Roles="Admin")]
        public async Task <ActionResult> DeleteLoan (int loanId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   bool result = await _loanService.DeleteLoan (loanId);
                   if (result)
                   {
                      return Ok ("Loan deleted successfully");
                   }
                   else
                   {
                      throw new LoanException ("Loan cannot be deleted, it is referenced in loanapplication");
                   }
                }
                else
                {
                    return NotFound ();
                }
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }
    }
}