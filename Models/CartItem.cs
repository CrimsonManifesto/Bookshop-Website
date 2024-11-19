using System.ComponentModel.DataAnnotations;

namespace Bookshop_Website.Models

{
    public class CartItem
    {

        public int BookId { get; set; }
        public string? Title { get; set; }
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
