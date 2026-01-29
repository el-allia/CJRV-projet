using UnityEngine;

/// <summary>
/// ScriptableObject for food data
/// Unity 6 compatible version
/// </summary>
[CreateAssetMenu(fileName = "NewFood", menuName = "Restaurant Game/Food Data")]
public class FoodData : ScriptableObject
{
    /// <summary>
    /// Types of food available in the game
    /// </summary>
    public enum FoodType 
    { 
        Burger, 
        Garantita, 
        Coffee 
    }
    
    [Header("Food Information")]
    [Tooltip("Type of food")]
    public FoodType foodType;
    
    [Tooltip("Display name of the food")]
    public string foodName;
    
    [Header("Gameplay Parameters")]
    [Tooltip("Cooking time in seconds")]
    [Range(1f, 60f)]
    public float cookingTime = 10f;
    
    [Tooltip("Time window after cooking completes before food burns")]
    [Range(1f, 30f)]
    public float burnDelay = 5f;
    
    [Header("Scoring")]
    [Tooltip("Points awarded for perfect timing")]
    public int perfectScore = 50;
    
    [Tooltip("Points awarded for good timing")]
    public int goodScore = 30;
    
    [Tooltip("Points awarded for late delivery")]
    public int lateScore = 10;

    [Header("Visual")]
    [Tooltip("Icon or sprite for this food (optional)")]
    public Sprite foodIcon;

    [Tooltip("Prefab of cooked food (optional)")]
    public GameObject cookedFoodPrefab;

    /// <summary>
    /// Validates the food data
    /// </summary>
    private void OnValidate()
    {
        // Ensure cooking time is positive
        if (cookingTime <= 0f)
        {
            cookingTime = 1f;
        }

        // Ensure burn delay is positive
        if (burnDelay <= 0f)
        {
            burnDelay = 1f;
        }

        // Ensure scores are logical
        if (perfectScore < goodScore)
        {
            perfectScore = goodScore + 10;
        }

        if (goodScore < lateScore)
        {
            goodScore = lateScore + 10;
        }
    }

    /// <summary>
    /// Get display text for this food type
    /// </summary>
    public string GetDisplayText()
    {
        return foodType switch
        {
            FoodType.Burger => "🍔 Burger",
            FoodType.Garantita => "🥙 Garantita",
            FoodType.Coffee => "☕ Coffee",
            _ => foodName
        };
    }

    /// <summary>
    /// Calculate score based on timing
    /// </summary>
    /// <param name="timingQuality">0-1 value representing timing quality</param>
    public int CalculateScore(float timingQuality)
    {
        timingQuality = Mathf.Clamp01(timingQuality);

        if (timingQuality >= 0.9f)
            return perfectScore;
        else if (timingQuality >= 0.6f)
            return goodScore;
        else
            return lateScore;
    }
}
