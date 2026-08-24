using UnityEngine;

public class DialogueAreaTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Dialogue dialogue;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player!=null)
            {
                player.SetDialogueArea(this);
            }
            Debug.Log("Player entered dialogue area");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.ClearDialogueArea(this);
            }

            Debug.Log("Player left dialogue area!");
        }
    }

    public void StartDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}
