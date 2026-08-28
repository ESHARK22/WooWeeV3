using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{   
    public GameObject Player;
    public AudioClip dialogueSound;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public GameObject dialoguePanel;
    public GameObject continueButton;

    public GameObject[] optionButtons;

    public Animator animator;

    private Dialogue currentDialogue;
    private DialogueNode currentNode;

    public void StartDialogue(Dialogue dialogue)
    {
        AudioSource.PlayClipAtPoint(dialogueSound, Player.transform.position);
        animator.SetBool("IsOpen", true );
        dialoguePanel.SetActive(true);

        currentDialogue = dialogue;
        currentNode = dialogue.startingNode;

        nameText.text = dialogue.characterName;

        DisplayNode(currentNode);
    }

    void DisplayNode(DialogueNode node)
    {
        currentNode = node;

        dialogueText.text = node.sentence;

        // Hide all option buttons first
        foreach (GameObject button in optionButtons)
        {
            button.SetActive(false);
        }

        // No choices means this is the end of this branch
        if (node.options == null || node.options.Length == 0)
        {
            continueButton.SetActive(true);
            return;
        }

        continueButton.SetActive(false);

        for (int i = 0; i < node.options.Length; i++)
        {
            optionButtons[i].SetActive(true);

            optionButtons[i]
                .GetComponentInChildren<TextMeshProUGUI>()
                .text = node.options[i].optionText;

            int choiceIndex = i;

            Button button = optionButtons[i].GetComponent<Button>();

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                () => ChooseOption(choiceIndex)
            );
        }
    }

    void ChooseOption(int index)
    {
        DialogueNode nextNode = currentNode.options[index].nextNode;

        if (nextNode != null)
        {
            DisplayNode(nextNode);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false );
        dialoguePanel.SetActive(false);
    }
}