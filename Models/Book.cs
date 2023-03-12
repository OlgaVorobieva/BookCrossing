using BookCrossingApp.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace BookCrossingApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public int BCID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public BookCategories CategoryID { get; set; }
        public string? Description { get; set; }
        public BookStatus Status { get; set; }

    }
}
