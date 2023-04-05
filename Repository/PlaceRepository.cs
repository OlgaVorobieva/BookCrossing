using BookCrossingApp.Data;
using BookCrossingApp.Data.Enum;
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

        public async Task<Place?> GetByIdAsync(int id)
        {
            return await _context.Places.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Place>> GetAllActive()
        {
            return await _context.Places.Where( x=> x.Status==PlaceStatus.Active).ToListAsync();
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
        public bool Update(Place place)
        {
            _context.Update(place);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Places.CountAsync();
        }

    }
}
