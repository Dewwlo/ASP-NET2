namespace HaggesPizzeria.Models
{
    public class OrderedDishIngredient
    {
        public int OrderedDishId { get; set; }
        public OrderedDish OrderedDish { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
