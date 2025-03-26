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
        public async Task<IActionResult> UploadProfilePicture(string croppedImage)
        {
            if (!string.IsNullOrEmpty(croppedImage))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Limit image size
                const int maxSizeInBytes = 2 * 1024 * 1024; // 2MB
                var base64Data = croppedImage.Substring(croppedImage.IndexOf(",") + 1);
                var bytes = Convert.FromBase64String(base64Data);

                // Check image size
                if (bytes.Length > maxSizeInBytes)
                {
                    TempData["ErrorMessage"] = "The image file is too large. Maximum size is 2MB.";
                    return Redirect("/Identity/Account/Manage");
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.ProfilePictureUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }


                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + ".webp";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the cropped image to the file system
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);

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

                return Redirect("/Identity/Account/Manage");
            }

            TempData["ErrorMessage"] = "No image selected.";
            return Redirect("/Identity/Account/Manage");
        }

    }
}
