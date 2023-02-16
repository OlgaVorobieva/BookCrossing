using BookCrossingApp.Data;
using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCrossingApp.Repository
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Place>> GetAll()
        {
            return await _context.Places.ToListAsync();
        }

        public bool Add(Place place)
        {
            _context.Add(place);
            return Save();
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!BookExists(book.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //}
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
