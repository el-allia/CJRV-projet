using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterToStart : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("sample1");
        }
    }
}