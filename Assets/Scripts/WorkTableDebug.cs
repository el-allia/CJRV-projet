using UnityEngine;

/// <summary>
/// Attach to WorkTable to see if drops are being detected
/// </summary>
public class WorkTableDebug : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[WORKTABLE] Something collided: {collision.gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[WORKTABLE] Something triggered: {other.gameObject.name}");
    }

    void OnMouseOver()
    {
        Debug.Log("[WORKTABLE] Mouse is over the table");
    }

    // Visual gizmo in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}