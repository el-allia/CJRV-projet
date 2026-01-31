using UnityEngine;

public class CookBurger : MonoBehaviour
{
    public GameObject cookedMeatPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MeatRaw"))
        {
            Destroy(other.gameObject);
            Instantiate(cookedMeatPrefab, transform.position, Quaternion.identity);
        }
    }
}
