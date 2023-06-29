using Microsoft.CodeAnalysis.Differencing;
using Microsoft.IdentityModel.Tokens;

namespace BookCrossingApp.Helpers
{
    public class PhotoHelper
    {
        const string UploadsFolder = @"wwwroot\uploads\";

        public static async Task AddPhotoAsync(IFormFile file)
        {
            if (file != null)
            {
                var fileName = GetFileName(file);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), UploadsFolder, fileName);

                using var stream = System.IO.File.Create(filePath);
                await file.CopyToAsync(stream);
            }
        }
        public static void DeletePhoto(string photoImageUrl)
        {
            var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), UploadsFolder, photoImageUrl);
            System.IO.File.Delete(deleteFilePath);
        }

        public static string GetProfileImageFullPath(string? profileImageUrl)
        {
            return profileImageUrl.IsNullOrEmpty() ? "/img/default.jpg" : "/uploads/" + profileImageUrl;
        }

        public static string GetFileName(IFormFile file)
        {
            return file.FileName?.Split('\\')?.LastOrDefault()?.Split('/').LastOrDefault() ?? string.Empty;
        }
    }
}
