using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static bool endlessMode;

    public void MainGame()
    {
        endlessMode = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

    }

        public void EndlessMode()
    {
        endlessMode = true;
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
