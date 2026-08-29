using UnityEngine;

public class TutorialContinue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void Continue()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}