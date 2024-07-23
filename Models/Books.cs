using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bookshop_Website.Models
{
    public class Books
    {
        public Books(){}

        [Key]
        public int BookId { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

    }
}
