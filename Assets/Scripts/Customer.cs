using UnityEngine;
using TMPro;

/// <summary>
/// Represents a restaurant customer with an order and satisfaction level
/// Unity 6 compatible version
/// </summary>
public class Customer : MonoBehaviour
{
    /// <summary>
    /// Available order types
    /// </summary>
    public enum OrderType
    {
        Burger,
        Garantita,
        Coffee
    }

    [Header("Order")]
    [Tooltip("Customer's order type")]
    public OrderType orderType;

    [Header("Satisfaction")]
    [Tooltip("Customer satisfaction (0 to 100)")]
    [Range(0f, 100f)]
    public float satisfaction = 100f;

    [Tooltip("Maximum wait time in seconds")]
    public float maxWaitTime = 30f;

    [Tooltip("Remaining wait time")]
    [SerializeField] private float waitTimer;

    [Header("UI")]
    [Tooltip("Reference to UI text for displaying order")]
    public TextMeshProUGUI orderText;

    [Header("Visual Feedback")]
    [Tooltip("Material to change color (optional)")]
    public Material customerMaterial;

    // Renderer for changing customer color
    private Renderer customerRenderer;
    private MaterialPropertyBlock propertyBlock;

    // Reference to spawner for cleanup
    private CustomerSpawner spawner;

    private void Awake()
    {
        // Get renderer component
        customerRenderer = GetComponentInChildren<Renderer>();
        
        // Initialize property block for efficient material changes
        if (customerRenderer != null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        // Generate random order
        int orderCount = System.Enum.GetValues(typeof(OrderType)).Length;
        orderType = (OrderType)Random.Range(0, orderCount);

        // Initialize timer and satisfaction
        waitTimer = maxWaitTime;
        satisfaction = 100f;

        // Update initial color
        UpdateColor();

        // Display order text
        UpdateOrderText();

        Debug.Log($"New customer spawned - Order: {orderType}, Max Wait: {maxWaitTime}s");
    }

    private void Update()
    {
        // Decrease timer
        waitTimer -= Time.deltaTime;
        waitTimer = Mathf.Max(waitTimer, 0f);

        // Calculate satisfaction based on remaining time
        satisfaction = (waitTimer / maxWaitTime) * 100f;

        // Update visual color
        UpdateColor();

        // Customer leaves if satisfaction reaches zero
        if (satisfaction <= 0f)
        {
            Debug.Log($"Customer left dissatisfied - Order was: {orderType}");
            LeaveRestaurant(false);
        }
    }

    /// <summary>
    /// Updates customer color based on satisfaction level
    /// </summary>
    private void UpdateColor()
    {
        if (customerRenderer == null || propertyBlock == null) return;

        Color targetColor;
        
        if (satisfaction > 66f)
            targetColor = Color.green;
        else if (satisfaction > 33f)
            targetColor = Color.yellow;
        else
            targetColor = Color.red;

        // Use MaterialPropertyBlock for better performance
        customerRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", targetColor);
        customerRenderer.SetPropertyBlock(propertyBlock);
    }

    /// <summary>
    /// Updates the order text display
    /// </summary>
    private void UpdateOrderText()
    {
        if (orderText != null)
        {
            orderText.text = GetOrderText();
        }
    }

    /// <summary>
    /// Called when customer receives food
    /// </summary>
    /// <param name="foodReceived">Type of food received</param>
    /// <returns>True if order is correct</returns>
    public bool ReceiveFood(OrderType foodReceived)
    {
        bool isCorrect = foodReceived == orderType;
        
        if (isCorrect)
        {
            Debug.Log($"Customer received correct order: {orderType}");
            LeaveRestaurant(true);
        }
        else
        {
            Debug.Log($"Wrong order! Expected: {orderType}, Got: {foodReceived}");
        }
        
        return isCorrect;
    }

    /// <summary>
    /// Customer leaves the restaurant
    /// </summary>
    /// <param name="satisfied">Whether customer was satisfied</param>
    private void LeaveRestaurant(bool satisfied)
    {
        // Notify spawner to remove from active list
        if (spawner != null)
        {
            spawner.RemoveCustomer(gameObject);
        }
        else
        {
            // Fallback if spawner reference not set
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Returns the order text with emoji/icon
    /// </summary>
    /// <returns>Formatted order string</returns>
    public string GetOrderText()
    {
        return orderType switch
        {
            OrderType.Burger => "🍔 Burger",
            OrderType.Garantita => "🥙 Garantita",
            OrderType.Coffee => "☕ Coffee",
            _ => ""
        };
    }

    /// <summary>
    /// Set the spawner reference for proper cleanup
    /// </summary>
    public void SetSpawner(CustomerSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    /// <summary>
    /// Get current wait timer (for debugging/UI)
    /// </summary>
    public float GetWaitTimer() => waitTimer;

    /// <summary>
    /// Get current satisfaction (for debugging/UI)
    /// </summary>
    public float GetSatisfaction() => satisfaction;
}
