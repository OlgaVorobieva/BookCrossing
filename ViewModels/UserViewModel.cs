using BookCrossingApp.Data;

namespace BookCrossingApp.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public bool isEnabled { get; set; }
    public string? userRole { get; set; }
    public string? ProfileImageUrl { get; set; }
}