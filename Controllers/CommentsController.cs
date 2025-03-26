using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bookshop_Website.Data;
using Bookshop_Website.Models;

namespace Bookshop_Website.Controllers
{
    public class CommentsController : Controller
    {
        private readonly BooksDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(BooksDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comments/AddComment
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                // Optionally, log errors or return error feedback.
                return RedirectToAction("Details", "Books", new { id = comment.BookId });
            }

            comment.CreatedAt = DateTime.Now;
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    comment.UserName = currentUser.UserName;
                    comment.ProfilePictureUrl = currentUser.ProfilePictureUrl;
                }
            }
            else
            {
                comment.UserName = "Anonymous";
                comment.ProfilePictureUrl = "/images/profiles/default-profile.png";
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Books", new { id = comment.BookId });
        }

        // POST: Comments/DeleteComment/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            if (!isAdmin && comment.UserName != currentUser.UserName)
                return Forbid();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Books", new { id = comment.BookId });
        }
    }
}
