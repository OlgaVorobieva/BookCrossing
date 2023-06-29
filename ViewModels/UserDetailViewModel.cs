using BookCrossingApp.Data.Enum;
using BookCrossingApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCrossingApp.ViewModels;

public class UserDetailViewModel
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int Points { get; set; }

    public List<PlaceBookedViewModel>? bookedPlaces { get; set; }
}

public class PlaceBookedViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? PictureName { get; set; }

} 