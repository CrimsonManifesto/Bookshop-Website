using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookshop_Website.Data;
using Bookshop_Website.Extensions;
using Bookshop_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace Bookshop_Website.Controllers
{
    public class BooksController : Controller
    {
        private readonly BooksDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(BooksDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.ProfilePictureUrl = user?.ProfilePictureUrl;

            return View(await _context.Books.ToListAsync());

        }

        // GET: Books/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }
        // GET: Books/Search
        public async Task<IActionResult> Search()
        {
            return View();
        }
        // Post: Books/SearchResults
        public async Task<IActionResult> SearchResults(String SearchPhrase)
        {
            ViewData["SearchPhrase"] = SearchPhrase;
            return View("Search", await _context.Books.Where(j => j.Title.Contains(SearchPhrase)).ToListAsync());
        }
        // GET: Books/Create
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>
            Create([Bind("BookId,Title,Author,Genre,Publisher,Language,DiscountPercentage,OriginalPrice,ImageUrl,Description, PublicationDate, NumberOfPages, InStock, AverageRating,NumberSold, StockQuantity")]Books books)
        {
            if (ModelState.IsValid)
            {
                _context.Add(books);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(books);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books.FindAsync(id);
            if (books == null)
            {
                return NotFound();
            }
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,Genre,Publisher,Language,DiscountPercentage,OriginalPrice,ImageUrl,Description, PublicationDate, NumberOfPages, InStock, AverageRating,NumberSold, StockQuantity")] Books books)
        {
            if (id != books.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(books);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksExists(books.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(books);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var books = await _context.Books.FindAsync(id);
            if (books != null)
            {
                _context.Books.Remove(books);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        // GET: Books/Genres
        public async Task<IActionResult> Genres()
        {
            var genres = await _context.Books
                .Select(b => b.Genre)
                .Distinct()
                .ToListAsync();

            return PartialView("_GenresPartial"); // Pass genres directly to the partial view
        }
        // POST: Books/GenresShow
        public async Task<IActionResult> GenresShow(string SearchPhrase)
        {
            ViewData["Category"] = SearchPhrase;
            var books = await _context.Books
                .Where(b => b.Genre.Contains(SearchPhrase))
                .ToListAsync();

            return View("Search", books);
        }
        // GET: Books/Genres
        public async Task<IActionResult> Languages()
        {
            var genres = await _context.Books
                .Select(b => b.Language)
                .Distinct()
                .ToListAsync();

            return PartialView("_GenresPartial");
        }
        // POST: Books/GenresShow
        public async Task<IActionResult> LanguagesShow(string SearchPhrase)
        {
            ViewData["Category"] = SearchPhrase;
            var books = await _context.Books
                .Where(b => b.Language.Contains(SearchPhrase))
                .ToListAsync();

            return View("Search", books);
        }
        public IActionResult AddToCart(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.BookId == id);
            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    BookId = book.BookId,
                    ImageUrl = book.ImageUrl,
                    Title = book.Title,
                    Price = book.Price,
                    Quantity = 1
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart != null)
            {
                var cartItem = cart.FirstOrDefault(c => c.BookId == id);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            return RedirectToAction(nameof(ViewCart));
        }



    }
}
