using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bookshop_Website.Models;

namespace Bookshop_Website.Data
{
    public class BooksDbContext : IdentityDbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }
        public DbSet<Bookshop_Website.Models.Books> Books { get; set; } = default!;
    }
}
