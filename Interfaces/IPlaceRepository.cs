﻿using BookCrossingApp.Models;

namespace BookCrossingApp.Interfaces
{
    public interface IPlaceRepository
    {
        bool Add(Place place);
        Task<IEnumerable<Place>> GetAllActive();
        Task<IEnumerable<Place>> GetAllPlacesByBookId(int bookId);
        Task<Place?> GetByIdAsync(int id);
        Task<int> GetCountAsync();
        bool Update(Place place);

    }
}