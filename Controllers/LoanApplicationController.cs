using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnetapp.Services;
using dotnetapp.Models;
using dotnetapp.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class LoanApplicationController : ControllerBase
    {
        private readonly LoanApplicationService _loanApplicationService;
        public LoanApplicationController (LoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpGet ("loan-application")]
        [Authorize (Roles="Admin")]
        public async Task <ActionResult <IEnumerable <LoanApplication>>> GetAllLoanApplications ()
        {
            var applications = await _loanApplicationService.GetAllLoanApplications();
            try
            {
                if (applications != null)
                {
                    return Ok (applications);
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

        [HttpGet ("/api/loan-application/user/{userId}")]
        [Authorize (Roles="User")]
        public async Task <ActionResult <LoanApplication>> GetLoanApplicationByUserId (int userId)
        {
            try
            {
                var application = await _loanApplicationService.GetLoanApplicationsByUserId (userId);
                if (application == null)
                {
                    throw new LoanException ("");
                }
                else
                {
                    return Ok (application);
                }
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpPost ("/api/loan-application")]
        [Authorize (Roles="User")]
        public async Task <ActionResult> AddLoanApplication ([FromBody] LoanApplication loanApplication)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = await _loanApplicationService.AddLoanApplication (loanApplication);
                    if (result)
                    {
                        return Ok ("Loan application added successfully");
                    }
                    else
                    {
                        throw new LoanException ("");
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

        [HttpPut ("/api/loan-application/{loanApplicationId}")]
        [Authorize (Roles="Admin,User")]
        public async Task <ActionResult> UpdateLoanApplication (int loanApplicationId, [FromBody] LoanApplication loanApplication)
        {
            //Console.WriteLine(loanApplicationId);
            try
            {
                 bool result =  await _loanApplicationService.UpdateLoanApplication (loanApplicationId, loanApplication);
                
                if (result)
                {
                    return Ok ("Loan application updated successfully");
                }
                else
                {
                    return NotFound ("Cannot find any loan application");
                }
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpDelete ("/api/loan-application/{loanApplicationId}")]
        [Authorize (Roles="User")]
        public async Task<ActionResult> DeleteLoanApplication (int loanApplicationId)
        {
            try
            {
                bool result = await _loanApplicationService.DeleteLoanApplication (loanApplicationId);
                if (result)
                {
                    return Ok ("Loan application deleted successfully");
                }
                else
                {
                    return NotFound ("Cannot find any loan application");
                }
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }
    }
}   