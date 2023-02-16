using Microsoft.AspNetCore.Identity;

namespace BookCrossingApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? ProfileImageUrl { get; set; }
    }
}
