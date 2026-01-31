using UnityEngine;

public class CutBread : MonoBehaviour
{
    public int slices = 0;
    public GameObject bottomSlice;
    public GameObject topSlice;

    public void AddSlice()
    {
        if (slices == 0)
            bottomSlice.SetActive(true);
        else if (slices == 1)
            topSlice.SetActive(true);

        slices++;
    }
}