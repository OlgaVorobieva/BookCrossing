using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookCrossingApp.Data;
using BookCrossingApp.Models;
using BookCrossingApp.Interfaces;
using BookCrossingApp.ViewModels;
using System.Globalization;
using BookCrossingApp.Data.Enum;
using Microsoft.AspNetCore.Identity;
using BookCrossingApp.Repository;

namespace BookCrossingApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;

        public BookController(IBookRepository bookRepository, 
            IPlaceRepository placeRepository, 
            UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _bookRepository = bookRepository;
            _placeRepository = placeRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAll();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || await _bookRepository.GetCountAsync() == 0)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();     
            }
            List<BookHistoryViewModel> bookHistory = new List<BookHistoryViewModel>();
            var bookDetail = new BookDetailViewModel
            {
                Id = book.Id,
                BCID = book.BCID,
                Author = book.Author,
                Title = book.Title,
                Description= book.Description,
                bookHistory = bookHistory
            };

            /// history        
            var items = await _placeRepository.GetAllPlacesByBookId(book.Id);
            if (items == null)
            {
                return View(bookDetail);
            }
            

            foreach (var item in items)
            {
                var bookStory = new BookHistoryViewModel()
                {
                    Id = item.Id,
                    GiverUserId = item.UserId,
                    TakerUserId = item.TakerUserId,
                    Status = item.Status,
                    Date = item.Date,
                };

                var giver = await _userRepository.GetUserById(item.UserId);
                bookStory.GiverUserName = giver.UserName;

                if (item.TakerUserId != null)
                {
                    var taker = await _userRepository.GetUserById(item.TakerUserId);
                    bookStory.TakerUserName = taker.UserName;
                }

                bookHistory.Add(bookStory);
            }

            bookDetail.bookHistory = bookHistory;

            return View(bookDetail);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BCID,Title,Author,Description,CategoryID")] Book book)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                book.CreatorUserId = currentUser.Id;
                
                book.PictureName = "default_book.png";
                _bookRepository.Add(book);

                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }


        public async Task<JsonResult> GetByPlaceId(int id)
        {
            var place = await _placeRepository.GetByIdAsync(id);
            if (place != null)
            {
                var result = await _bookRepository.GetByIdAsync(place.BookId);
                return Json(result); 
            }
            return Json(null);
        }

        // GET: Books/PlaceBook/5
        public async Task<IActionResult> PlaceBook(int? id)
        {

            if (id == null || await _bookRepository.GetCountAsync() == 0)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }
            var bookPlace = new BookPlaceViewModel
            {
                Id = book.Id,
                BCID = book.BCID,
                Author = book.Author,
                Title = book.Title,

            };
            return View(bookPlace);
        }

        // POST: Books/Place/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBook(int id, BookPlaceViewModel bookVM)
        {
            if (id != bookVM.Id)
            {
                return NotFound();
            }

            var result = false;
            var currentUser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && currentUser != null)
            {
                var place = new Place
                {
                    BookId = id,
                    Description = bookVM.PlaceDescription,
                    UserId = currentUser.Id,
                    Date = DateTime.Now,
                    Status = PlaceStatus.Active
                };
                var culture = new CultureInfo("en-US");
                place.Longitude = Single.Parse(bookVM.Longitude, culture);
                place.Latitude = Single.Parse(bookVM.Latitude, culture);

                //should be in one transaction
                result = _placeRepository.Add(place);
                result = await _bookRepository.ChangeBookStatus(id, BookStatus.OnMap);

                currentUser.Points += 1;
                _userRepository.Update(currentUser);
            }

            if (!result)
            {
                ModelState.AddModelError("", "Failed to place book");
                return View(bookVM);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || await _bookRepository.GetCountAsync() == 0)
            {
                return NotFound();
            }

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,PictureName,CreatorUserId,BCID,Title,Author,Description,CategoryID")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _bookRepository.Update(book);
               
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return View("Error");
            }

            _bookRepository.Delete(book);
            return RedirectToAction("Index");
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                return View("Error");
            }

            _bookRepository.Delete(book);
            return RedirectToAction(nameof(Index));
        }

    }
}
