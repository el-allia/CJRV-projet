using UnityEngine;
using UnityEngine.SceneManagement;

public class QuadButton : MonoBehaviour
{
    public string sceneToLoad;
    public bool quitGame;

    void OnMouseDown()
    {
        if (quitGame)
            Application.Quit();
        else
            SceneManager.LoadScene(sceneToLoad);
    }
}