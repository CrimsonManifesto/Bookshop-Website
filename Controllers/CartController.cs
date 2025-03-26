using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Bookshop_Website.Data;
using Bookshop_Website.Models;
using Bookshop_Website.Extensions;

namespace Bookshop_Website.Controllers
{
    public class CartController : Controller
    {
        private readonly BooksDbContext _context;

        public CartController(BooksDbContext context)
        {
            _context = context;
        }

        // GET: Cart/AddToCart/5
        public IActionResult AddToCart(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
                return NotFound();

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.BookId == id);
            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    BookId = book.BookId,
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
            return RedirectToAction("Index", "Books");
        }

        // GET: Cart/ViewCart
        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // GET: Cart/RemoveFromCart/5
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
            return RedirectToAction("ViewCart");
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

        [HttpPost]
        public IActionResult DecreaseQuantity(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.BookId == id);
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new
            {
                success = true,
                quantity = cartItem?.Quantity ?? 1,
                totalPrice = cartItem != null ? (cartItem.Price * cartItem.Quantity).ToString("C") : "$0.00",
                cartTotal = cart.Sum(item => item.Price * item.Quantity).ToString("C")
            });
        }

        // GET: Cart/GetCartCount
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            int cartCount = cart.Sum(item => item.Quantity);
            return Json(new { count = cartCount });
        }
    }
}
