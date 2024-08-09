using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bookshop_Website.Models
{
    public class Books
    {
        public Books(){}

        [Key]
        public int BookId { get; set; }

        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]

        public decimal Price { get; set; }

        public string Publisher { get; set; }

        public string Genre { get; set; }

        public string Language { get; set; }



    }
}
