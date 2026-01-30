using UnityEngine;

public enum IngredientType
{
    BreadBottom,
    BreadTop,
    Meat,
    Cheese,
    Tomato,
    Salad
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;
    public bool isCooked = false;
}
