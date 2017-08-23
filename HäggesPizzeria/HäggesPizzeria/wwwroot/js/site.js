function updateIngredient(dishId, ingredientId, addIngredient) {
    $.ajax({
        url: '../UpdateBaseDishIngredient',
        type: 'POST',
        data: { 'dishId': dishId, 'ingredientId': ingredientId, 'addIngredient': addIngredient }
    }).done(function (response) {
        $("#base-dish-ingredients").html(response);
    });
}
