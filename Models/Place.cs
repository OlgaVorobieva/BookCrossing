using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCrossingApp.Models
{
    public class Place
    {
        [Key]
        public int Id { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public int Status { get; set; }
        public string? Description { get; set; }
    }
}
