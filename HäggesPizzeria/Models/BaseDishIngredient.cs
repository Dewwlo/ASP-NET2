namespace HäggesPizzeria.Models
{
    public class BaseDishIngredient
    {
        public int BaseDishId { get; set; }
        public BaseDish BaseDish { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}
