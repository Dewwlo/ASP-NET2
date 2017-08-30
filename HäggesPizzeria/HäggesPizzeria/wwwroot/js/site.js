function updateDishIngredient(baseDishId, ingredientId, addIngredient, isOrderedDish) {
    $.ajax({
        url: '/Ingredient/UpdateDishIngredient',
        type: 'POST',
        data: { 'baseDishId': baseDishId, 'ingredientId': ingredientId, 'addIngredient': addIngredient, 'isOrderedDish': isOrderedDish }
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

function createEditCategory(categoryId) {
    $.ajax({
        url: '/Category/CreateEditCategory',
        type: 'POST',
        data: { 'categoryId': categoryId }
    }).done(function (response) {
        $("#create-edit-category").html(response);
    });
}