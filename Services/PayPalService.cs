using Bookshop_Website.Models;
using Microsoft.Extensions.Configuration;
using PayPal.Api;
using PaypalServerSdk.Standard.Models;
using System;
using System.Collections.Generic;

public class PayPalService
{
    private readonly IConfiguration _config;

    public PayPalService(IConfiguration config)
    {
        _config = config;
    }

    public Payment CreatePayment(string baseUrl, string cancelUrl, string returnUrl, List<CartItem> cartItems)
    {
        var apiContext = GetAPIContext();

        var itemList = new ItemList()
        {
            items = cartItems.Select(cartItem => new PayPal.Api.Item()
            {
                name = cartItem.Title,
                currency = "USD",
                price = cartItem.Price.ToString("F2"), 
                quantity = cartItem.Quantity.ToString(),
                sku = cartItem.BookId.ToString() 
            }).ToList()
        };

        var totalAmount = cartItems.Sum(cartItem => cartItem.TotalPrice).ToString("F2");

        var amount = new Amount()
        {
            currency = "USD",
            total = totalAmount
        };

        var transaction = new Transaction()
        {
            amount = amount,
            item_list = itemList,
            description = "Thanh toán đơn hàng tại Kabasa",
            invoice_number = "INV-" + new Random().Next(100000)
        };

        var payer = new PayPal.Api.Payer() { payment_method = "paypal" };

        var redirectUrls = new RedirectUrls()
        {
            cancel_url = cancelUrl,
            return_url = returnUrl
        };

        var payment = new Payment()
        {
            intent = "sale",
            payer = payer,
            transactions = new List<Transaction>() { transaction },
            redirect_urls = redirectUrls
        };

        return payment.Create(apiContext);
    }

    public APIContext GetAPIContext()
    {
        var clientId = _config["PayPal:ClientId"];
        var clientSecret = _config["PayPal:ClientSecret"];

        var config = new Dictionary<string, string>()
        {
            { "mode", _config["PayPal:Mode"] ?? throw new InvalidOperationException("Paypal mode error") }
        };

        var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
        return new APIContext(accessToken) { Config = config };
    }
}
