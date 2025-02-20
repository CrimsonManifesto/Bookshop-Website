using Microsoft.AspNetCore.Mvc;
using Bookshop_Website.Models;
using System.Collections.Generic;
using PayPal.Api;
using Newtonsoft.Json;
using Bookshop_Website.Extensions;

public class PayPalController : Controller
{
    private readonly PayPalService _payPalService;

    public PayPalController(PayPalService payPalService)
    {
        _payPalService = payPalService;
    }

    public IActionResult PaymentWithPaypal()
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string cancelUrl = $"{baseUrl}/PayPal/PaymentCancelled";
        string returnUrl = $"{baseUrl}/PayPal/PaymentSuccess";

        // 🔹 Lấy giỏ hàng từ Session hoặc Database
        var cartItems = GetCartItems();

        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index", "Cart"); // Nếu giỏ hàng rỗng, quay về trang giỏ hàng
        }

        // 🔹 Tạo thanh toán với PayPal
        var payment = _payPalService.CreatePayment(baseUrl, cancelUrl, returnUrl, cartItems);

        // 🔹 Điều hướng người dùng đến PayPal để thanh toán
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
            // 🔹 Xử lý đơn hàng thành công: cập nhật trạng thái, xóa giỏ hàng
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


    private void ClearCart()
    {
       
    }
}
