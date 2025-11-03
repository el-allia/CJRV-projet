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

    private GameObject chosenHead;
    private GameObject chosenHair;
    private GameObject chosenTorso;
    private GameObject chosenLegs;

    public enum Gender { Male, Female }
    private Gender characterGender;

    private void ClearSlots()
    {
        if (chosenHead)  Destroy(chosenHead);
        if (chosenHair)  Destroy(chosenHair);
        if (chosenTorso) Destroy(chosenTorso);
        if (chosenLegs)  Destroy(chosenLegs);
    }

    public void SpawnRandomCharacter()
    {
        ClearSlots();

        int a = Random.Range(0, 2); // 0 = Male, 1 = Female

        if (a == 0)
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
        int h = Random.Range(0, headMale.Length);
        int hr = Random.Range(0, hairMale.Length);
        int t = Random.Range(0, torsoMale.Length);
        int l = Random.Range(0, legsMale.Length);

        chosenHead  = Instantiate(headMale[h],  headSlot);
        chosenHair  = Instantiate(hairMale[hr], hairSlot);
        chosenTorso = Instantiate(torsoMale[t], torsoSlot);
        chosenLegs  = Instantiate(legsMale[l],  legsSlot);
    }

    private void SpawnFemaleCharacter()
    {
        int h = Random.Range(0, headFemale.Length);
        int hr = Random.Range(0, hairFemale.Length);
        int t = Random.Range(0, torsoFemale.Length);
        int l = Random.Range(0, legsFemale.Length);

        chosenHead  = Instantiate(headFemale[h],  headSlot);
        chosenHair  = Instantiate(hairFemale[hr], hairSlot);
        chosenTorso = Instantiate(torsoFemale[t], torsoSlot);
        chosenLegs  = Instantiate(legsFemale[l],  legsSlot);
    }
}
