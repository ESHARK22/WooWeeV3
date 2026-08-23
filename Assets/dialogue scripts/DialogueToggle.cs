using UnityEngine;

public class DialogueToggle : MonoBehaviour
{
    
public GameObject DialoguePanel; 
public void ToggleDialogue() {
    DialoguePanel.SetActive(!DialoguePanel.activeSelf);
}

}
