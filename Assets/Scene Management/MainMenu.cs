using UnityEngine;

public class MainMenu : MonoBehaviour
{


    public void MainGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

    }

    public void Tutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TutorialScene");

    }
    public void quitGame()
    {
        Application.Quit();
    }
}
