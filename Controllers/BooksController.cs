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
using Microsoft.IdentityModel.Tokens;
using System.IO;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats.Webp;

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

            var bestSellingBooks = await _context.Books
                .OrderByDescending(b => b.NumberSold)
                .Take(8)
                .ToListAsync();

            var newBooks = await _context.Books
                .OrderByDescending(b => b.BookId)
                .Take(8)
                .ToListAsync();

            var flashSaleBooks = await _context.Books
                .OrderByDescending(b => b.DiscountPercentage)
                .Take(8)
                .ToListAsync();

            ViewBag.BestSellingBooks = bestSellingBooks;
            ViewBag.NewBooks = newBooks;
            ViewBag.FlashSaleBooks = flashSaleBooks;
            ViewBag.AllBooks = await _context.Books.ToListAsync();

            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Search
        public IActionResult Search()
        {
            return View();
        }

        // POST: Books/SearchResults
        public async Task<IActionResult> SearchResults(string SearchPhrase)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [Bind("BookId,Title,Author,Genre,Publisher,Language,DiscountPercentage,OriginalPrice,Description,PublicationDate,NumberOfPages,InStock,AverageRating,NumberSold,StockQuantity")] Books books,
            IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(ms);
                        books.ImageData = ms.ToArray();
                        books.ImageMimeType = imageFile.ContentType;
                    }
                }

                _context.Add(books);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(books);
        }

        // GET: Books/Edit
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

        // POST: Books/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("BookId,Title,Author,Genre,Publisher,Language,DiscountPercentage,OriginalPrice,Description,PublicationDate,NumberOfPages,InStock,AverageRating,NumberSold,StockQuantity")] Books books,
            IFormFile? imageFile)
        {
            if (id != books.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new image file is provided, update the image data.
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(ms);
                            books.ImageData = ms.ToArray();
                            books.ImageMimeType = imageFile.ContentType;
                        }
                    }
                    else
                    {
                        // Retrieve the existing image data if no new file is provided.
                        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == id);
                        if (existingBook != null)
                        {
                            books.ImageData = existingBook.ImageData;
                            books.ImageMimeType = existingBook.ImageMimeType;
                        }
                    }

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

        // GET: Books/Delete
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

        // POST: Books/Delete
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

            return PartialView("~/Views/Shared/TopNavigation/GenresPartial.cshtml");
        }

        // GET: Books/GenresShow
        public async Task<IActionResult> GenresShow(string SearchPhrase)
        {
            ViewData["Category"] = SearchPhrase;
            var books = await _context.Books
                .Where(b => b.Genre.Contains(SearchPhrase))
                .ToListAsync();

            return View("Search", books);
        }

        // GET: Books/LanguagesShow
        public async Task<IActionResult> LanguagesShow(string SearchPhrase)
        {
            ViewData["Category"] = SearchPhrase;
            var books = await _context.Books
                .Where(b => b.Language.Contains(SearchPhrase))
                .ToListAsync();

            return View("Search", books);
        }

        // GET: Books/AllLanguages
        public async Task<IActionResult> AllLanguages()
        {
            var languages = await _context.Books
                .Select(b => b.Language)
                .Distinct()
                .ToListAsync();

            return View(languages);
        }

        [HttpGet]
        public async Task<IActionResult> AdvancedSearchResults(AdvancedSearchModel searchModel)
        {
            var booksQuery = _context.Books.AsQueryable();

            // Filter by Title
            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchModel.Title));
            }

            // Filter by Language
            if (!string.IsNullOrEmpty(searchModel.Language))
            {
                booksQuery = booksQuery.Where(b => b.Language == searchModel.Language);
            }

            // Filter by Price
            if (searchModel.MinPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b =>
                    (b.OriginalPrice * (1 - ((decimal)b.DiscountPercentage / 100))) >= searchModel.MinPrice.Value);
            }
            if (searchModel.MaxPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b =>
                    (b.OriginalPrice * (1 - ((decimal)b.DiscountPercentage / 100))) <= searchModel.MaxPrice.Value);
            }

            // Sort by Discount
            if (!string.IsNullOrEmpty(searchModel.Discount))
            {
                booksQuery = searchModel.Discount.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? booksQuery.OrderBy(b => b.DiscountPercentage)
                    : booksQuery.OrderByDescending(b => b.DiscountPercentage);
            }

            // SortBy + SortOrder
            if (!string.IsNullOrEmpty(searchModel.SortBy))
            {
                switch (searchModel.SortBy.ToLower())
                {
                    case "title":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.Title)
                            : booksQuery.OrderByDescending(b => b.Title);
                        break;
                    case "price":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.OriginalPrice * (1 - ((decimal)b.DiscountPercentage / 100)))
                            : booksQuery.OrderByDescending(b => b.OriginalPrice * (1 - ((decimal)b.DiscountPercentage / 100)));
                        break;
                    case "discount":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.DiscountPercentage)
                            : booksQuery.OrderByDescending(b => b.DiscountPercentage);
                        break;
                    case "date":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.PublicationDate)
                            : booksQuery.OrderByDescending(b => b.PublicationDate);
                        break;
                    case "popular":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.AverageRating)
                            : booksQuery.OrderByDescending(b => b.AverageRating);
                        break;
                    case "bestseller":
                        booksQuery = (searchModel.SortOrder == "asc")
                            ? booksQuery.OrderBy(b => b.NumberSold)
                            : booksQuery.OrderByDescending(b => b.NumberSold);
                        break;
                }
            }

            var booksList = await booksQuery.ToListAsync();
            return View("Search", booksList);
        }

        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.BookId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new
            {
                success = true,
                quantity = cartItem?.Quantity ?? 0,
                totalPrice = cartItem != null ? (cartItem.Price * cartItem.Quantity).ToString("C") : "$0.00",
                cartTotal = cart.Sum(item => item.Price * item.Quantity).ToString("C")
            });
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/AddComment
        [HttpPost]

        // GET: Books/GetImage/5
        [HttpGet]
        public async Task<IActionResult> GetImage(int id, int? width, int? height, string mode = "crop", string format = null)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.ImageData == null || book.ImageMimeType == null)
            {
                return NotFound();
            }

            using var msInput = new MemoryStream(book.ImageData);
            using var image = await Image.LoadAsync(msInput);

            if (width.HasValue || height.HasValue)
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(width ?? image.Width, height ?? image.Height),
                    Mode = mode.Equals("crop", StringComparison.OrdinalIgnoreCase)
                        ? ResizeMode.Crop
                        : ResizeMode.Max
                };
                image.Mutate(x => x.Resize(resizeOptions));
            }

            var msOutput = new MemoryStream();

            if (!string.IsNullOrEmpty(format) && format.Equals("webp", StringComparison.OrdinalIgnoreCase))
            {
                await image.SaveAsWebpAsync(msOutput, new WebpEncoder());
                msOutput.Position = 0;
                return File(msOutput.ToArray(), "image/webp");
            }
            else if (book.ImageMimeType.ToLower().Contains("png"))
            {
                await image.SaveAsPngAsync(msOutput);
                msOutput.Position = 0;
                return File(msOutput.ToArray(), "image/png");
            }
            else
            {
                await image.SaveAsJpegAsync(msOutput);
                msOutput.Position = 0;
                return File(msOutput.ToArray(), "image/jpeg");
            }
        }
    }
}