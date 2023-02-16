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
        public int CategoryID { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

    }
}
