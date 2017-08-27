function updateDishIngredient(dishName, ingredientId, addIngredient, isOrderedDish) {
    $.ajax({
        url: '/BaseDish/UpdateDishIngredient',
        type: 'POST',
        data: { 'dishName': dishName, 'ingredientId': ingredientId, 'addIngredient': addIngredient, 'isOrderedDish': isOrderedDish }
    }).done(function (response) {
        $("#base-dish-ingredients").html(response);
    });
}