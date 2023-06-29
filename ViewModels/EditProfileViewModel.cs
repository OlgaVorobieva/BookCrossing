namespace BookCrossingApp.ViewModels
{
    public class EditProfileViewModel
    {
        public string? ProfileImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string? Username { get; set; }
    }
}