using UnityEngine;

public class NPCInteractPrompt : MonoBehaviour
{
    [Header("UI Prompt")]
    [Tooltip("Drag the 'E' button GameObject here")]
    [SerializeField] private GameObject interactPrompt;

    private void Awake()
    {
        // Ensure the prompt is hidden at the start
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Show prompt when Player enters the radius
        if (collision.CompareTag("Player"))
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Hide prompt when Player leaves the radius
        if (collision.CompareTag("Player"))
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}