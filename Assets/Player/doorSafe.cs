using UnityEngine;
using UnityEngine.SceneManagement;

public class doorSafe : MonoBehaviour
{
    public bool isSafeDoor = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door trigger.");
        }
        if (GameResult.currentLoop == 5)
        {
            GameResult.PlayerWon = true;
        }
        if (other.CompareTag("Player") && isSafeDoor && GameResult.currentLoop >= 5)
        {
            GameResult.currentLoop++;
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
            
        }
        else if (other.CompareTag("Player") && !isSafeDoor)
        {
            GameResult.PlayerWon = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
        }
        else if (other.CompareTag("Player") && isSafeDoor && GameResult.currentLoop < 5)
        {
            GameResult.currentLoop++;
            GameResult.PlayerWon = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

