using Microsoft.AspNetCore.Identity;

namespace Bookshop_Website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() {}
        public string FullName { get; set; }
        public string UserImage { get; set; }
    }
}
