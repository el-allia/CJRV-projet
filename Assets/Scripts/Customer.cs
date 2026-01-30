using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    public enum OrderType { Burger, Garantita, Coffee }

    [Header("Order")]
    public OrderType orderType;

    [Header("Satisfaction")]
    [Range(0f, 100f)] public float satisfaction = 100f;
    public float maxWaitTime = 30f;

    [SerializeField] private float waitTimer;

    [Header("UI")]
    public TextMeshProUGUI orderText;

    private bool hasLeft = false;

    private Renderer customerRenderer;
    private MaterialPropertyBlock propertyBlock;

    private CustomerSpawner spawner;

    private void Awake()
    {
        customerRenderer = GetComponentInChildren<Renderer>();
        if (customerRenderer != null)
            propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        int orderCount = System.Enum.GetValues(typeof(OrderType)).Length;
        orderType = (OrderType)Random.Range(0, orderCount);

        waitTimer = maxWaitTime;
        satisfaction = 100f;

        UpdateColor();
        UpdateOrderText();
    }

    private void Update()
    {
        if (hasLeft) return;

        waitTimer -= Time.deltaTime;
        waitTimer = Mathf.Max(waitTimer, 0f);

        satisfaction = (waitTimer / maxWaitTime) * 100f;

        UpdateColor();

        if (satisfaction <= 0f)
        {
            LeaveRestaurant();
        }
    }

    private void UpdateColor()
    {
        if (customerRenderer == null || propertyBlock == null) return;

        Color targetColor =
            (satisfaction > 66f) ? Color.green :
            (satisfaction > 33f) ? Color.yellow :
            Color.red;

        customerRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", targetColor);
        customerRenderer.SetPropertyBlock(propertyBlock);
    }

    private void UpdateOrderText()
    {
        if (orderText != null)
            orderText.text = GetOrderText();
    }

    public bool ReceiveFood(OrderType foodReceived)
    {
        if (hasLeft) return false;

        bool correct = (foodReceived == orderType);
        if (correct)
            LeaveRestaurant();

        return correct;
    }

    private void LeaveRestaurant()
    {
        if (hasLeft) return;
        hasLeft = true;

        // remove from spawner count
        if (spawner != null)
            spawner.RemoveCustomer(this);

        // walk to exit then destroy
        ClientQueueMember qm = GetComponent<ClientQueueMember>();
        if (qm != null)
            qm.GoToExit();
        else
            Destroy(gameObject);
    }

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

    public void SetSpawner(CustomerSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }
}
