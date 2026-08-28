using UnityEngine;

public class DialogueToggle : MonoBehaviour
{
    public Dialogue dialogue;
    private CharacterIdentity targetCharacter;

    public void TriggerDialogue ()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, targetCharacter);
    }

}


