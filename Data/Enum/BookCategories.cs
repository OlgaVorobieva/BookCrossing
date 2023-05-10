using System.ComponentModel.DataAnnotations;

namespace BookCrossingApp.Data.Enum
{
    public enum BookCategories
    {
        [Display(Name = "Не определено")]
        None = 0,
        [Display(Name = "Художественная литература")]
        Fiction = 1,
        [Display(Name = "Поэзия")]
        Poetry = 2,
        [Display(Name = "Приключения")]
        Adventure = 3,
        [Display(Name = "Техническая литература")]
        Science = 4,
        [Display(Name = "Детектив")]
        Detective = 5,
        [Display(Name = "Фантастика")]
        SiFi = 6,
        [Display(Name = "Публицистика")]
        Journalism = 7,
    }
}
