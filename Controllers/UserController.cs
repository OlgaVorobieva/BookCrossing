using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookCrossingApp.ViewModels;
using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using BookCrossingApp.Data;

namespace BookCrossingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;


        public UserController(IUserRepository userRepository, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
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
                    ProfileImageUrl = user.ProfileImageUrl ?? "/img/default.jpg",
                    isEnabled = user.IsEnabled,
                    userRole = userRoles.FirstOrDefault()
                };
                result.Add(userViewModel);
            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl ?? "/img/default.jpg",
                Points = user.Points,
            };
            return View(userDetailViewModel);
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
                ProfileImageUrl = user.ProfileImageUrl,
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

            if (editVM.Image != null) // only update profile image
            {
                //var photoResult = await _photoService.AddPhotoAsync(editVM.Image);

                //if (photoResult.Error != null)
                //{
                //    ModelState.AddModelError("Image", "Failed to upload image");
                //    return View("EditProfile", editVM);
                //}

                //if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                //{
                //    _ = _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                //}

                //user.ProfileImageUrl = photoResult.Url.ToString();
                //editVM.ProfileImageUrl = user.ProfileImageUrl;

                //await _userManager.UpdateAsync(user);

                return View(editVM);
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Detail", "User", new { user.Id });
        }
    }
}