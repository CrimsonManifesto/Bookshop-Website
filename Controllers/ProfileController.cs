using Bookshop_Website.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop_Website.Controllers  
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account"); // Make sure user has been login
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only image files (.jpg, .jpeg, .png, .gif) are allowed.";
                    return RedirectToAction("Index");
                }

                if (!profilePicture.ContentType.StartsWith("image/"))
                {
                    TempData["ErrorMessage"] = "Invalid file type.";
                    return RedirectToAction("Index");
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = $"/images/profiles/{fileName}";
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile picture updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                }

                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "No file selected.";
            return RedirectToAction("Index");
        }

    }
}
