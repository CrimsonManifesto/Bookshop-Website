using Microsoft.AspNetCore.Identity;

namespace Bookshop_Website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePictureUrl { get; set; } = "/images/profiles/default-profile.png";
    }
}
