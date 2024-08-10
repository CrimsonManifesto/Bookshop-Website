using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bookshop_Website.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookshop_Website.Data
{
    public class BooksDbContext : IdentityDbContext<IdentityUser>
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }
        public DbSet<Bookshop_Website.Models.Books> Books { get; set; } = default!;
    }
}
