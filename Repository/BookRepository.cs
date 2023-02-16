using BookCrossingApp.Data;
using BookCrossingApp.Interfaces;
using BookCrossingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCrossingApp.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            return await _context.Books.ToListAsync();
        }

        public bool Add(Book book)
        {
            _context.Add(book);
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


        public bool Delete(Book book)
        {
            _context.Remove(book);
            return Save();
        }

        public bool Update(Book book)
        {
            _context.Update(book);
            return Save();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Books.CountAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }


    }
}
