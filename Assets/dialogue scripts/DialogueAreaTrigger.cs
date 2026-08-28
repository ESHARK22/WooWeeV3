using UnityEngine;

public class DialogueAreaTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private CharacterIdentity targetCharacter;

    private void Awake()
    {
        if (dialogueManager == null)
        {

            dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        if (targetCharacter == null)
        {
            targetCharacter = GetComponentInParent<CharacterIdentity>() 
                              ?? transform.parent?.GetComponentInChildren<CharacterIdentity>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.SetDialogueArea(this);
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
                player.ClearDialogueArea(this);
            }
        }
    }

    public void StartDialogue()
    {
        if (targetCharacter == null)
        {
            Debug.LogError($"DialogueAreaTrigger on '{gameObject.name}' has no Target Character assigned!", this);
            return;
        }

        dialogueManager.StartDialogue(dialogue, targetCharacter);
    }
}