function updateDishIngredient(dishName, ingredientId, addIngredient, isOrderedDish) {
    $.ajax({
        url: '/Ingredient/UpdateDishIngredient',
        type: 'POST',
        data: { 'dishName': dishName, 'ingredientId': ingredientId, 'addIngredient': addIngredient, 'isOrderedDish': isOrderedDish }
    }).done(function (response) {
        $("#dish-ingredients").html(response);
    });
}

function createEditIngredient(ingredientId) {
    $.ajax({
        url: '/Ingredient/CreateEditIngredient',
        type: 'POST',
        data: { 'ingredientId': ingredientId }
    }).done(function (response) {
        $("#create-edit-ingredient").html(response);
    });
}