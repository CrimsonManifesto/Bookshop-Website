﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bookshop_Website.Models
{
    public class Books
    {
        public Books(){}

        [Key]
        public int BookId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageMimeType { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]

        [Precision(18, 3)]
        [Display(Name = "Original Price")]
        public decimal OriginalPrice { get; set; }

        [Precision(18, 3)]
        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercentage { get; set; }

        public decimal Price => OriginalPrice * (1 - DiscountPercentage / 100);
        public required string Publisher { get; set; }
        public required string Genre { get; set; }
        public required string Language { get; set; }
        public required string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publication Date")]
        public DateTime PublicationDate { get; set; }

        [Display(Name = "Number of Pages")]
        public int NumberOfPages { get; set; }

        [Display(Name = "In Stock")]
        public bool InStock { get; set; }

        [Range(0, 5)]
        [Display(Name = "Average Rating")]
        public float AverageRating { get; set; }

        [Display(Name = "Number Sold")]
        public int NumberSold { get; set; }

        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
