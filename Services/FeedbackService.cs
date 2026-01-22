using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnetapp.Services
{
    public class FeedbackService
    {
        private readonly ApplicationDbContext _context;
        public FeedbackService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <IEnumerable <Feedback>> GetAllFeedbacks ()
        {
            var feedbacks =  await _context.Feedbacks.Include(u=>u.User).ToListAsync();
            return feedbacks;
        }
        public async Task <IEnumerable <Feedback>> GetFeedbacksByUserId (int userId)
        {
            IEnumerable <Feedback> feedbacks = _context.Feedbacks.Where (p => p.UserId == userId);
            return feedbacks;
        }
        public async Task <bool> AddFeedback (Feedback feedback)
        {
            if(feedback != null)
            {
                _context.Feedbacks.Add (feedback);
                await _context.SaveChangesAsync ();
                return true;
            }    
            return false;
        }
        public async Task <bool> DeleteFeedback (int feedbackId)
        {
            Feedback feedback = _context.Feedbacks.FirstOrDefault (p => p.FeedbackId == feedbackId);
            if(feedback != null)
            {
                _context.Feedbacks.Remove (feedback);
                await _context.SaveChangesAsync ();
                return true;
            }
            return false;
        }  
    }
}