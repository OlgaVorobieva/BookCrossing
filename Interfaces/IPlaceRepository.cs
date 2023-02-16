using BookCrossingApp.Models;

namespace BookCrossingApp.Interfaces
{
    public interface IPlaceRepository
    {
        bool Add(Place place);
        Task<IEnumerable<Place>> GetAll();
    }
}