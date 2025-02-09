using Microsoft.AspNetCore.Mvc;
using Bookshop_Website.Extensions;
using System.Collections.Generic;
using System.Linq;
using Bookshop_Website.Models;

namespace Bookshop_Website.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            Console.WriteLine("CartViewComponent is being called!"); // Kiểm tra xem có chạy không

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            int cartCount = cart.Sum(item => item.Quantity);

            Console.WriteLine($"Cart count: {cartCount}"); // In số lượng sản phẩm
            ViewData["Test"] = "Hello from ViewComponent"; // Kiểm tra ViewData

            return View(cartCount);
        }
    }
}
