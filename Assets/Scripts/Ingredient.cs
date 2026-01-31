using UnityEngine;

public enum IngredientType
{
    BottomBread,
    TopBread,
    Meat,
    Cheese,
    Tomato,
    Salad
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;
}
