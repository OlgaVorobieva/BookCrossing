using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookCrossingApp.ViewModels;
using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using BookCrossingApp.Data;
using BookCrossingApp.Repository;//my
using Microsoft.IdentityModel.Tokens;
using BookCrossingApp.Helpers;

namespace BookCrossingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPlaceRepository _placeRepository;
        private readonly IBookRepository _bookRepository;
        const string UploadsFolder = @"wwwroot\uploads\";


        public UserController(IUserRepository userRepository, UserManager<AppUser> userManager, IPlaceRepository placeRepository, IBookRepository bookRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _placeRepository = placeRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    ProfileImageUrl = GetProfileImageFullPath(user.ProfileImageUrl),
                    isEnabled = user.IsEnabled,
                    userRole = userRoles.FirstOrDefault()
                };
                result.Add(userViewModel);
            }
            return View(result);
        }

        private static string GetProfileImageFullPath(string? profileImageUrl)
        {
            return profileImageUrl.IsNullOrEmpty() ? "/img/default.jpg" : "/uploads/" + profileImageUrl;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            List<PlaceBookedViewModel> bookedPlaces = new List<PlaceBookedViewModel>();
            var userDetail = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfileImageUrl = GetProfileImageFullPath(user.ProfileImageUrl),
                Points = user.Points,
            };

            var items = await _placeRepository.GetAllBookedPlacesByUserId(user.Id);
            if (items == null)
            {
                return View(userDetail);
            }

            foreach (var item in items)
            {
                var bookedPlace = new PlaceBookedViewModel()
                {
                    Id = item.Id,
                    BookId = item.BookId,
                };

                var book = await _bookRepository.GetByIdAsync(item.BookId);

                bookedPlace.Title = book.Title;
                bookedPlace.PictureName = book.PictureName; 

                bookedPlaces.Add(bookedPlace);
            }

            userDetail.bookedPlaces = bookedPlaces;

            return View(userDetail);
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            user.IsEnabled = user.IsEnabled ? false : true;
            _userRepository.Update(user);

            return RedirectToAction("Index", "User");
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.FirstOrDefault() == UserRoles.Admin)
            {
                await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user,UserRoles.User);
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            _userRepository.Update(user);

            return RedirectToAction("Index", "User");
        }

        [HttpGet]
     //   [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            var editMV = new EditProfileViewModel()
            {
                ProfileImageUrl = GetProfileImageFullPath(user.ProfileImageUrl)
            };
            return View(editMV);
        }

        [HttpPost]
 //       [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditProfile", editVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            if (editVM.Image != null) 
            {
                try
                {
                    await PhotoHelper.AddPhotoAsync(editVM.Image);
                    //delete previous
                    if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                    {
                        PhotoHelper.DeletePhoto(user.ProfileImageUrl);
                    }
                    user.ProfileImageUrl = PhotoHelper.GetFileName(editVM.Image);
                    editVM.ProfileImageUrl = PhotoHelper.GetProfileImageFullPath(user.ProfileImageUrl);
                    editVM.Username = user.UserName;

                    await _userManager.UpdateAsync(user);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("Image", "Failed to upload image " + exception.Message);
                    return View("EditProfile", editVM);
                }

                return View(editVM);
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Detail", "User", new { user.Id });
        }
    }
}