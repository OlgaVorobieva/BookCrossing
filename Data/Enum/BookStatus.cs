using System.ComponentModel.DataAnnotations;

namespace BookCrossingApp.Data.Enum
{
    public enum BookStatus
    {
        [Display(Name = "Созданно")]
        Created,
        [Display(Name = "На карте")]
        OnMap,
        [Display(Name = "Взято")]
        TakenAway
    }
}
