function updateBaseDishIngredient(ingredientId, addIngredient) {
    $.ajax({
        url: '/BaseDish/UpdateBaseDishIngredient',
        type: 'POST',
        data: { 'ingredientId': ingredientId, 'addIngredient': addIngredient }
    }).done(function (response) {
        $("#base-dish-ingredients").html(response);
    });
}