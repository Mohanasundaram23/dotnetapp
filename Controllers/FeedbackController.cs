using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dotnetapp.Data;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    //[AllowAnonymous]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController (FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet ("/api/feedback")]
        [Authorize (Roles="Admin")]
        public async Task <ActionResult <IEnumerable <Feedback>>> GetAllFeedbacks ()
        {
            IEnumerable <Feedback> feedbacks = await _feedbackService.GetAllFeedbacks ();
            try
            {
                if (feedbacks != null)
                {
                    return Ok (feedbacks);
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

        [HttpGet ("/api/feedback/user/{userId}")]
        [Authorize (Roles="User")]
        public async Task<ActionResult <IEnumerable <Feedback>>> GetFeedbacksByUserId (int userId)
        {
            IEnumerable <Feedback> feedbacks = await _feedbackService.GetFeedbacksByUserId (userId);
            try
            {
                if (feedbacks != null)
                {
                    return Ok (feedbacks);
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

        [HttpPost ("/api/feedback")]
        [Authorize (Roles="User")]
        public async Task <ActionResult> AddFeedback ([FromBody] Feedback feedback)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ru = await _feedbackService.AddFeedback (feedback);
                    if (ru)
                    {
                        return Ok ("Feedback added successfully");
                    }
                    else
                    {
                        return BadRequest ();
                    }
                }
                throw new LoanException ("");
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpDelete ("/api/feedback/{feedbackId}")]
        [Authorize (Roles="User")]
        public async Task <ActionResult> DeleteFeedback (int feedbackId)
        {
            try
            {
                var result = await _feedbackService.DeleteFeedback (feedbackId);
                if (result == true)
                {
                    return Ok ("Feedback deleted successfully");
                }
                else
                {
                    return NotFound ("Cannot find any feedback");
                }    
            }
            catch (LoanException ex)
            {
                return StatusCode (500, ex.Message);
            }
        } 
    }
}