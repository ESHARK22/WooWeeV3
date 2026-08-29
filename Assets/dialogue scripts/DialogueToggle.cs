using UnityEngine;

public class DialogueToggle : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private CharacterIdentity targetCharacter;

    public void TriggerDialogue()
    {
        DialogueManager dm = FindAnyObjectByType<DialogueManager>();
        if (dm != null)
        {
            dm.StartDialogue(dialogue, targetCharacter);
        }
    }
}