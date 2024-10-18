namespace Bookshop_Website.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }

        public ICollection<Books> Books { get; set; }
    }
}
