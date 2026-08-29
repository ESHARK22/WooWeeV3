using UnityEngine;
using UnityEngine.SceneManagement;

public class doorSafe : MonoBehaviour
{
    public bool isSafeDoor = false;
    public GameObject interactPrompt; // child prompt object

    private PlayerMovement playerInRange;
    public string doorName; // default to the GameObject's name
    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Door trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                playerInRange = player;
                player.SetDoorArea(this);
                if (interactPrompt != null)
                    interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ClearDoorArea(this);
                playerInRange = null;
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        if (GameResult.currentLoop == 5)
        {
            GameResult.PlayerWon = true;
        }

        if (isSafeDoor && GameResult.currentLoop >= 5 && !MainMenu.endlessMode)
        {
            GameResult.currentLoop++;
            SceneManager.LoadScene("EndScene");
        }
        else if (!isSafeDoor)
        {
            GameResult.PlayerWon = false;
            SceneManager.LoadScene("EndScene");
        }
        else if (isSafeDoor && (GameResult.currentLoop < 5 || MainMenu.endlessMode))
        {
            GameResult.currentLoop++;
            GameResult.PlayerWon = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}