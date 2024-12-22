using Microsoft.AspNetCore.Mvc;
using Bookshop_Website.Models;  // Đảm bảo import đúng namespace chứa model Feedback
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
        public async Task<IActionResult> SubmitFeedback(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("The feedback content cannot be empty.");
            }

            var feedback = new Feedback
            {
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
