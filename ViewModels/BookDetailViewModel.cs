
using BookCrossingApp.Data.Enum;
using BookCrossingApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCrossingApp.ViewModels
{
    public class BookDetailViewModel
    {
        public int Id { get; set; } 
        public int BCID { get; set; }
        public string? Title { get; set; }        
        public string? Author { get; set; }
        public string? Description { get; set; } 

        public List<BookHistoryViewModel>? bookHistory { get; set; }


    }

    public class BookHistoryViewModel 
    { 
        public int Id { get; set; }
        public string? GiverUserId { get; set; }    
        public string? TakerUserId { get; set; }
        public string? GiverUserName { get; set; }
        public string? TakerUserName { get; set; }
        public PlaceStatus Status { get; set; }
        public DateTime? Date { get; set; }
    }
}
