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
            Console.WriteLine("CartViewComponent is being called!"); 

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            int cartCount = cart.Sum(item => item.Quantity);

            Console.WriteLine($"Cart count: {cartCount}"); 
            ViewData["Test"] = "Hello from ViewComponent"; 

            return View(cartCount);
        }
    }
}
