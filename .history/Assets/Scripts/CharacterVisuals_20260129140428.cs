using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer hairRenderer;
    public Renderer torsoRenderer;
    public Renderer legsRenderer;

    [Header("Materials")]
    public Material[] hairMaterials;   // 2 colors per hair
    public Material[] torsoMaterials;  // 3 shirts
    public Material[] legsMaterials;   // 2 pants (shoes included)

    public void ApplyRandomLook()
    {
        if (hairMaterials.Length > 0)
            hairRenderer.material = hairMaterials[Random.Range(0, hairMaterials.Length)];

        if (torsoMaterials.Length > 0)
            torsoRenderer.material = torsoMaterials[Random.Range(0, torsoMaterials.Length)];

        if (legsMaterials.Length > 0)
            legsRenderer.material = legsMaterials[Random.Range(0, legsMaterials.Length)];
    }
}
