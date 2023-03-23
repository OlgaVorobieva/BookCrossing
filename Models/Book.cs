using BookCrossingApp.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("User")]
        public string? CreatorUserId { get; set; }

    }
}
