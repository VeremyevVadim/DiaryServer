using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDiary.Models
{
    public static class FoodType
    { 
        public enum Types
        {
            Alcohol,
            SoftDrinks,
            Confectionery,
            Cruises,
            DairyProducts,
            MeatProducts,
            Vegetables,
            SeaFood,
            BakeryProducts,
            Fruits
        }

        public static string[] TypesRU = {
            "Алкогольные напитки",
            "Безалкоголные напитки, соки",
            "Кондитерские изделия",
            "Крупы",
            "Молочные продукты",
            "Мясо",
            "Овощи",
            "Морепродукты",
            "Хлебобулочные изделия",
            "Ягоды и фрукты"
        };
    }
}
