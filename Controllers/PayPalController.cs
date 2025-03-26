using Microsoft.AspNetCore.Mvc;
using Bookshop_Website.Models;
using System.Collections.Generic;
using PayPal.Api;
using Newtonsoft.Json;
using Bookshop_Website.Extensions;
using Bookshop_Website.Data;
using Microsoft.AspNetCore.Identity;
using Polly;
using Microsoft.EntityFrameworkCore;


public class PayPalController : Controller
{
    private readonly PayPalService _payPalService;
    private readonly BooksDbContext _context;
    private readonly ILogger<PayPalController> _logger;


    public PayPalController(BooksDbContext context, PayPalService payPalService)
    {
        _payPalService = payPalService;
        _context = context;

    }

    public IActionResult PaymentWithPaypal()
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string cancelUrl = $"{baseUrl}/PayPal/PaymentCancelled";
        string returnUrl = $"{baseUrl}/PayPal/PaymentSuccess";

        var cartItems = GetCartItems();

        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index", "Cart"); 
        }

        var payment = _payPalService.CreatePayment(baseUrl, cancelUrl, returnUrl, cartItems);

        foreach (var link in payment.links)
        {
            if (link.rel.ToLower() == "approval_url")
            {
                return Redirect(link.href);
            }
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> BuyWithPaypal(int bookId, int quantity = 1)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string cancelUrl = $"{baseUrl}/PayPal/PaymentCancelled";
        string returnUrl = $"{baseUrl}/PayPal/PaymentSuccess";

        var books = await GetBookByIdAsync(bookId);

        if (books == null)
        {
            return RedirectToAction("Index", "Books");
        }

        var cartItems = new List<CartItem>
    {
        new CartItem
        {
            BookId = books.BookId,
            Title = books.Title,
            Price = books.Price,
            Quantity = quantity,
            ImageData = books.ImageData,
            ImageMimeType = books.ImageMimeType
        }
    };

        var payment = _payPalService.CreatePayment(baseUrl, cancelUrl, returnUrl, cartItems);

        foreach (var link in payment.links)
        {
            if (link.rel.ToLower() == "approval_url")
            {
                return Redirect(link.href);
            }
        }

        return RedirectToAction("Index", "Home");
    }


    public IActionResult PaymentSuccess(string paymentId, string token, string PayerID)
    {
        var apiContext = _payPalService.GetAPIContext();

        var payment = new Payment() { id = paymentId };
        var paymentExecution = new PaymentExecution() { payer_id = PayerID };

        var executedPayment = payment.Execute(apiContext, paymentExecution);

        if (executedPayment.state.ToLower() == "approved")
        {
            ClearCart();
            return View("PaymentSuccess");
        }

        return RedirectToAction("PaymentFailed");
    }

    public IActionResult PaymentCancelled()
    {
        return View("PaymentCancelled");
    }

    private List<CartItem> GetCartItems()
    {
        return HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
    }

    private async Task<Books?> GetBookByIdAsync(int bookId)
    {

        var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
        return book;
    }

    private void ClearCart()
    {
       
    }
}
