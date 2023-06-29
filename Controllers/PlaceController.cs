using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using BookCrossingApp.Repository;
using BookCrossingApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookCrossingApp.Controllers
{
    public class PlaceController : Controller
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public PlaceController(IPlaceRepository placeRepository, IBookRepository bookRepository, UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _placeRepository = placeRepository;
            _bookRepository = bookRepository;   
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<JsonResult> GetBooksPlaces()
        {
            var data = await _placeRepository.GetAllActive();
            var result = data.Select(
                p => new AllBooksViewModel { Id = p.Id, Latitude = p.Latitude, Longitude = p.Longitude })
                .ToList();
            return Json(result); 
        }

        public async Task<IActionResult> ConfirmBookReceipt(int? id) 
        {
            var place = await _placeRepository.GetByIdAsync(id.Value);
            if (place == null)
            {
                return NotFound();
            }

            place.Status = Data.Enum.PlaceStatus.Inactive;
            _placeRepository.Update(place);

            return RedirectToAction("Detail", "User");////
        }

        // GET: Place/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await _placeRepository.GetCountAsync() == 0)
            {
                return NotFound();
            }

            var place = await _placeRepository.GetByIdAsync(id.Value);
            if (place == null)
            {
                return NotFound();
            }
            var book = await _bookRepository.GetByIdAsync(place.BookId);
            if (book == null)
            {
                return NotFound();
            }
            var bookPlace = new BookPlaceViewModel()
            {
                Id = id.Value,
                BCID = book.BCID,
                Author = book.Author,
                Title = book.Title,
                Description = book.Description,
                PlaceDescription = place.Description??string.Empty,
                Status = place.Status,
            };
            bookPlace.Longitude = place.Longitude.ToString();
            bookPlace.Latitude = place.Latitude.ToString();
            return View(bookPlace);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TakeConfirmed(int id)
        {

            var place = await _placeRepository.GetByIdAsync(id);
            if (place == null)
            {
                return NotFound();
            }
           
            var book = await _bookRepository.GetByIdAsync(place.BookId);
            if (book == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            // next statements should be one transaction
            
            place.Status = Data.Enum.PlaceStatus.Visited;
            place.TakerUserId = currentUser.Id;
            _placeRepository.Update(place);

            book.Status = Data.Enum.BookStatus.TakenAway;

            _bookRepository.Update(book);

            currentUser.Points -= 3;
            await _userManager.UpdateAsync(currentUser);

            if (place.UserId != null) {
                var creatorUser = await _userRepository.GetUserById(place.UserId);
                creatorUser.Points += 3;
                _userRepository.Update(creatorUser);
            }
            return RedirectToAction("AllBooks","Home");
        }

        // GET: PlaceController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: PlaceController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: PlaceController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: PlaceController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        
    }
}
