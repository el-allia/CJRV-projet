using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    [Header("Female Parts")]
    public GameObject head;
    public GameObject[] hairVariants;
    public GameObject[] torsoVariants;
    public GameObject[] legsVariants;

    [Header("Slots")]
    public Transform headSlot;
    public Transform hairSlot;
    public Transform torsoSlot;
    public Transform legsSlot;

    GameObject currentHair;
    GameObject currentTorso;
    GameObject currentLegs;
    GameObject currentHead;

    [ContextMenu("Spawn Random Character")]
    public void SpawnRandomCharacter()
    {
        Clear();

        // HEAD (only one)
        currentHead = Instantiate(head, headSlot);
        ResetLocal(currentHead);

        // HAIR (color baked in mesh)
        currentHair = Instantiate(
            hairVariants[Random.Range(0, hairVariants.Length)],
            hairSlot
        );
        ResetLocal(currentHair);

        // TORSO (shirt color baked)
        currentTorso = Instantiate(
            torsoVariants[Random.Range(0, torsoVariants.Length)],
            torsoSlot
        );
        ResetLocal(currentTorso);

        // LEGS (pants + shoes baked)
        currentLegs = Instantiate(
            legsVariants[Random.Range(0, legsVariants.Length)],
            legsSlot
        );
        ResetLocal(currentLegs);
    }

    void Clear()
    {
        if (currentHead) Destroy(currentHead);
        if (currentHair) Destroy(currentHair);
        if (currentTorso) Destroy(currentTorso);
        if (currentLegs) Destroy(currentLegs);
    }

    void ResetLocal(GameObject obj)
    {
        Transform t = obj.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    void Start()
    {
        SpawnRandomCharacter();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SpawnRandomCharacter();
    }
}
