using BookCrossingApp.Models;

namespace BookCrossingApp.Interfaces
{
    public interface IBookRepository
    {
        bool Add(Book book);
        bool Delete(Book book);
        Task<IEnumerable<Book>> GetAll();
        Task<Book?> GetByIdAsync(int id);
        Task<int> GetCountAsync();
        bool Save();
        bool Update(Book book);
    }
}