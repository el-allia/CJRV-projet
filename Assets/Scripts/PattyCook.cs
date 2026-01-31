using UnityEngine;

public class PattyCook : MonoBehaviour
{
    public Mesh rawMesh;
    public Mesh halfMesh;
    public Mesh cookedMesh;

    public float timeToHalf = 4f;
    public float timeToCooked = 8f;

    private float cookTimer = 0f;
    private bool onGrill = false;
    private bool flipped = false;

    private MeshFilter mf;

    enum CookState { Raw, Half, Cooked }
    CookState state = CookState.Raw;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mf.mesh = rawMesh;
    }

    void Update()
    {
        if (!onGrill) return;

        cookTimer += Time.deltaTime;

        if (cookTimer > timeToHalf && state == CookState.Raw)
        {
            state = CookState.Half;
            mf.mesh = halfMesh;
        }

        if (cookTimer > timeToCooked && state == CookState.Half)
        {
            state = CookState.Cooked;
            mf.mesh = cookedMesh;
        }
    }

    public void Flip()
    {
        flipped = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grill"))
        {
            onGrill = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grill"))
        {
            onGrill = false;
        }
    }

    public bool IsCooked()
    {
        return state == CookState.Cooked;
    }
}
