using Microsoft.AspNetCore.Mvc;
using Bookshop_Website.Models;  
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Bookshop_Website.Data;

namespace Bookshop_Website.Controllers
{
    public class SupportController : Controller
    {
        private readonly BooksDbContext _context;

        public SupportController(BooksDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(string content, string feedbackType, string userEmail, string userPhone, string userName)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(feedbackType))
            {
                return BadRequest("The feedback content or type cannot be empty.");
            }

            var feedback = new Feedback
            {
                FeedbackType = feedbackType,
                UserEmail = userEmail,
                UserPhone = userPhone,
                UserName = userName,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            // Save feedback in database
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
