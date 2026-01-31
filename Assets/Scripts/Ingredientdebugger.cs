using UnityEngine;

/// <summary>
/// Attach this to an ingredient prefab to see its measurements
/// </summary>
public class IngredientDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== INGREDIENT DEBUG ===");
        Debug.Log($"Name: {gameObject.name}");
        
        Ingredient ing = GetComponent<Ingredient>();
        if (ing != null)
            Debug.Log($"Type: {ing.type}");
        else
            Debug.LogError("NO INGREDIENT SCRIPT!");

        Collider col = GetComponent<Collider>();
        if (col != null)
            Debug.Log($"Collider size: {col.bounds.size}");
        else
            Debug.LogError("NO COLLIDER!");

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            Debug.Log($"Renderer size: {rend.bounds.size}");
        else
            Debug.LogWarning("No Renderer found");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            Debug.Log($"Rigidbody: isKinematic={rb.isKinematic}, useGravity={rb.useGravity}");
        else
            Debug.LogError("NO RIGIDBODY!");

        Debug.Log($"Tag: {tag}");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Local Position: {transform.localPosition}");
        Debug.Log($"Local Scale: {transform.localScale}");
        Debug.Log("=====================");
    }
}