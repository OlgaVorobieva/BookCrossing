using BookCrossingApp.Data.Enum;
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
        public string? UserId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [ForeignKey("User")]
        public string? TakerUserId { get; set; }
        public PlaceStatus Status { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
    }
}
