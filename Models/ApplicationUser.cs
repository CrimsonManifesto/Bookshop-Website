using Microsoft.AspNetCore.Identity;

namespace Bookshop_Website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePictureUrl { get; set; } = "/images/profiles/default-profile.png";
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public ICollection<Books> Wishlist { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
