function updateIngredient(dishId, ingredientId, addIngredient) {
    $.ajax({
        url: '/BaseDish/UpdateBaseDishIngredient',
        type: 'POST',
        dataType: 'JSON',
        data: { 'dishId': dishId, 'ingredientId': ingredientId, 'addIngredient': addIngredient }
    });
}
