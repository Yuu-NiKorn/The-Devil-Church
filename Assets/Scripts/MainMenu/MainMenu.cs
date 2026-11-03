using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("SceneChris");
    }

    public void Quit()
    {
        Application.Quit();
    }
}