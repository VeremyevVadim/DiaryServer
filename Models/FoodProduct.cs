namespace FoodDiary.Models
{
    public class FoodProduct
    {
        public int Id { get; set; }
        public FoodType.Types Type { get; set; }
        public string Name {get; set;}
        public int Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }

        public FoodProduct() { }

        public FoodProduct(int Id, FoodType.Types Type, string Name, int Calories, double Proteins, double Fats, double Carbohydrates) {
            this.Id = Id;
            this.Type = Type;
            this.Name = Name;
            this.Calories = Calories;
            this.Proteins = Proteins;
            this.Fats = Fats;
            this.Carbohydrates = Carbohydrates;
        }
    }
}
