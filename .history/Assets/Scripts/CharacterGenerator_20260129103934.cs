using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    [Header("Male Parts")]
    public GameObject[] headMale;
    public GameObject[] hairMale;
    public GameObject[] torsoMale;
    public GameObject[] legsMale;

    [Header("Female Parts")]
    public GameObject[] headFemale;
    public GameObject[] hairFemale;
    public GameObject[] torsoFemale;
    public GameObject[] legsFemale;

    [Header("Slots")]
    public Transform headSlot;
    public Transform hairSlot;
    public Transform torsoSlot;
    public Transform legsSlot;

    private GameObject chosenHead, chosenHair, chosenTorso, chosenLegs;

    public enum Gender { Male, Female }
    private Gender characterGender;

    private void ClearSlots()
    {
        if (chosenHead)  Destroy(chosenHead);
        if (chosenHair)  Destroy(chosenHair);
        if (chosenTorso) Destroy(chosenTorso);
        if (chosenLegs)  Destroy(chosenLegs);
    }

    [ContextMenu("Spawn Random Character")]
    public void SpawnRandomCharacter()
    {
        // safety: don’t try to spawn if arrays are empty
        bool maleReady = headMale.Length > 0 && hairMale.Length > 0 && torsoMale.Length > 0 && legsMale.Length > 0;
        bool femaleReady = headFemale.Length > 0 && hairFemale.Length > 0 && torsoFemale.Length > 0 && legsFemale.Length > 0;

        if (!maleReady && !femaleReady)
        {
            Debug.LogWarning("Assign your prefabs to the arrays in the Inspector first.");
            return;
        }

        ClearSlots();

        // pick gender that actually has data
        int coin = Random.Range(0, 2); // 0 or 1
        if ((!maleReady && coin == 0) || (!femaleReady && coin == 1))
            coin = maleReady ? 0 : 1;

        if (coin == 0)
        {
            characterGender = Gender.Male;
            SpawnMaleCharacter();
        }
        else
        {
            characterGender = Gender.Female;
            SpawnFemaleCharacter();
        }

        Debug.Log("Spawned: " + characterGender);
    }

    private void SpawnMaleCharacter()
    {
        chosenHead  = Instantiate(headMale[Random.Range(0, headMale.Length)],   headSlot);
        chosenHair  = Instantiate(hairMale[Random.Range(0, hairMale.Length)],   hairSlot);
        chosenTorso = Instantiate(torsoMale[Random.Range(0, torsoMale.Length)], torsoSlot);
        chosenLegs  = Instantiate(legsMale[Random.Range(0, legsMale.Length)],   legsSlot);
        ResetLocal(chosenHead, chosenHair, chosenTorso, chosenLegs);
    }

    private void SpawnFemaleCharacter()
    {
        chosenHead  = Instantiate(headFemale[Random.Range(0, headFemale.Length)],   headSlot);
        chosenHair  = Instantiate(hairFemale[Random.Range(0, hairFemale.Length)],   hairSlot);
        chosenTorso = Instantiate(torsoFemale[Random.Range(0, torsoFemale.Length)], torsoSlot);
        chosenLegs  = Instantiate(legsFemale[Random.Range(0, legsFemale.Length)],   legsSlot);
        ResetLocal(chosenHead, chosenHair, chosenTorso, chosenLegs);
    }

    private void ResetLocal(params GameObject[] parts)
    {
        // ensure each spawned part sits exactly on the slot (no offsets)
        foreach (var p in parts)
        {
            var t = p.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale    = Vector3.one;
        }
    }

    private void Start()
    {
        // auto-spawn once on Play. If you want manual only, delete this line.
        SpawnRandomCharacter();
    }
    private void Update()
{
    if (Input.GetKeyDown(KeyCode.R))
    {
        SpawnRandomCharacter();
    }
}

}
