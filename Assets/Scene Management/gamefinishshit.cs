using UnityEngine;
using TMPro;

public class gamefinishshit : MonoBehaviour
{
    public TextMeshProUGUI menuText; // Reference to the TextMesh component for displaying "Game Over" text
    void Start()
    {
        if (GameResult.PlayerWon)
        {
            menuText.text = "Congrats, you have over 5 brain cells";
        }
        else
        {
            menuText.text = "You must be some kind of idiot";
        }

    }
    public void RestartGame()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
