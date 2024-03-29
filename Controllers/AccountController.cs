﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookCrossingApp.Data;
using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using BookCrossingApp.ViewModels;

namespace BookCrossingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        //private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager)//ApplicationDbContext context
        {
            //_context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                //User is found, check password
                if (!user.IsEnabled) 
                {
                    TempData["Error"] = "Пользователь заблокирован!";
                    return View(loginViewModel);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Неправильные логин или пароль. Пожалуйста попробуйте снова!";
                return View(loginViewModel);
            }
            //User not found
            TempData["Error"] = "Неправильные логин или пароль. Пожалуйста попробуйте снова!";
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "Этот электронный адрес уже используется";
                return View(registerViewModel);
            }

            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.EmailAddress,
                IsEnabled = true,
                Points = 0
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //[HttpGet]
        //[Route("Account/Welcome")]
        //public async Task<IActionResult> Welcome(int page = 0)
        //{
        //    if(page == 0)
        //    {
        //        return View();
        //    }
        //    return View();
            
        //}

      //  [HttpGet]
        //public async Task<IActionResult> GetLocation(string location)
        //{
        //    if(location == null)
        //    {
        //        return Json("Not found");
        //    }
        //    var locationResult = await _locationService.GetLocationSearch(location);
        //    return Json(locationResult);
        //}


    }
}