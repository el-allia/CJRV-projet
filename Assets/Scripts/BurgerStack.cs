using UnityEngine;
using System.Collections.Generic;

public class BurgerStack : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform stackPoint;
    public float heightPerIngredient = 0.08f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    [Header("Stack Data")]
    private List<IngredientType> stack = new List<IngredientType>();
    private List<GameObject> ingredients = new List<GameObject>();
    private List<float> ingredientHeights = new List<float>(); // Store actual heights!

    public bool HasBottomBread => stack.Contains(IngredientType.BottomBread);
    public bool HasTopBread => stack.Contains(IngredientType.TopBread);

    public bool CanAdd(IngredientType type)
    {
        if (type == IngredientType.BottomBread)
        {
            bool canAdd = stack.Count == 0;
            if (showDebugLogs) Debug.Log($"BottomBread check: stack.Count={stack.Count}, canAdd={canAdd}");
            return canAdd;
        }

        if (!HasBottomBread)
        {
            if (showDebugLogs) Debug.Log($"Cannot add {type}: no bottom bread yet!");
            return false;
        }

        if (HasTopBread)
        {
            if (showDebugLogs) Debug.Log($"Cannot add {type}: burger already has top bread!");
            return false;
        }

        return true;
    }

    public void AddIngredient(GameObject ingredient)
    {
        if (ingredient == null)
        {
            Debug.LogError("Trying to add null ingredient!");
            return;
        }

        Ingredient ing = ingredient.GetComponent<Ingredient>();
        if (ing == null)
        {
            Debug.LogWarning("Ingredient script missing on " + ingredient.name);
            Destroy(ingredient);
            return;
        }

        if (showDebugLogs) Debug.Log($"Attempting to add {ing.type} to burger...");

        if (!CanAdd(ing.type))
        {
            Debug.LogWarning($"Cannot add {ing.type}! Current stack: {string.Join(", ", stack)}");
            Destroy(ingredient);
            return;
        }

        // CRITICAL: Get size BEFORE parenting and save original scale
        Vector3 originalScale = ingredient.transform.localScale;
        float ingredientHeight = heightPerIngredient;
        
        Renderer rend = ingredient.GetComponent<Renderer>();
        if (rend != null)
        {
            ingredientHeight = rend.bounds.size.y;
            if (showDebugLogs) Debug.Log($"{ing.type} height: {ingredientHeight:F3}, scale: {originalScale}");
        }
        else
        {
            Collider col = ingredient.GetComponent<Collider>();
            if (col != null)
            {
                ingredientHeight = col.bounds.size.y;
                if (showDebugLogs) Debug.Log($"{ing.type} height: {ingredientHeight:F3}, scale: {originalScale}");
            }
        }

        // Parent it
        ingredient.transform.SetParent(stackPoint);
        ingredient.transform.localRotation = Quaternion.identity;
        ingredient.transform.localScale = originalScale; // Keep original scale

        // Calculate Y position using STORED heights
        float yPosition = 0f;

        if (stack.Count == 0)
        {
            // First ingredient
            yPosition = ingredientHeight / 2f;
            if (showDebugLogs) Debug.Log($"First ingredient: Y = {yPosition:F3}");
        }
        else
        {
            // Stack on top - use stored heights!
            float totalStackHeight = 0f;
            
            for (int i = 0; i < ingredientHeights.Count; i++)
            {
                totalStackHeight += ingredientHeights[i];
            }
            
            yPosition = totalStackHeight + (ingredientHeight / 2f);
            if (showDebugLogs) Debug.Log($"Stacking: totalHeight={totalStackHeight:F3}, newY={yPosition:F3}");
        }

        // Set position
        ingredient.transform.localPosition = new Vector3(0f, yPosition, 0f);

        // Disable physics
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Store the height for future stacking calculations
        ingredientHeights.Add(ingredientHeight);
        
        // Add to lists
        stack.Add(ing.type);
        ingredients.Add(ingredient);

        if (showDebugLogs) Debug.Log($"✓ Added {ing.type} at Y={yPosition:F3}. Stack: {string.Join(", ", stack)}");
    }

    public bool IsBurgerValid()
    {
        return HasBottomBread && HasTopBread;
    }

    public void ClearBurger()
    {
        foreach (GameObject obj in ingredients)
        {
            if (obj != null)
                Destroy(obj);
        }

        ingredients.Clear();
        stack.Clear();
        ingredientHeights.Clear(); // Clear heights too!
        
        if (showDebugLogs) Debug.Log("Burger cleared!");
    }
}