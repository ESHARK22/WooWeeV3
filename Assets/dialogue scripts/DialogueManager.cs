using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{   
    public GameObject Player;
    public AudioClip dialogueSound;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private CharacterIdentity speakerCharacter;

    public GameObject dialoguePanel;
    public GameObject continueButton;
    public GameObject[] optionButtons;
    public Animator animator;

    private Dialogue currentDialogue;
    private DialogueNode currentNode;

    public void StartDialogue(Dialogue dialogue, CharacterIdentity speaker)
    {
        if (dialogueSound != null && Player != null)
        {
            AudioSource.PlayClipAtPoint(dialogueSound, Player.transform.position);
        }
        
        animator.SetBool("IsOpen", true);

        speakerCharacter = speaker;
        currentDialogue = dialogue;
        currentNode = dialogue.startingNode;

        // Shows the speaker's generated name (or falls back to dialogue asset name)
        nameText.text = speakerCharacter != null 
            ? speakerCharacter.GetCharacterName() 
            : dialogue.characterName;

        DisplayNode(currentNode);
    }

    void DisplayNode(DialogueNode node)
    {
        currentNode = node;
        string sentence = node.sentence;

        if (speakerCharacter != null)
        {
            // The character being talked about
            CharacterIdentity subject = speakerCharacter.talkingAbout != null 
                ? speakerCharacter.talkingAbout 
                : speakerCharacter;

            // Replaces {targetName} with their real random name (e.g. "Arthur", "Alice")
            sentence = sentence.Replace("{targetName}", subject.GetCharacterName());

            if (speakerCharacter.truthTeller)
            {
                // TRUTH
                sentence = sentence.Replace("{hatColor}", subject.GetHatColor());
                sentence = sentence.Replace("{hat}", subject.GetHatName());
                sentence = sentence.Replace("{shirtColor}", subject.GetShirtColor());
                sentence = sentence.Replace("{shirt}", subject.GetShirtName());
                sentence = sentence.Replace("{pantsColor}", subject.GetPantsColor());
                sentence = sentence.Replace("{pants}", subject.GetPantsName());
                sentence = sentence.Replace("{shoesColor}", subject.GetShoesColor());
                sentence = sentence.Replace("{shoes}", subject.GetShoesName());
                sentence = sentence.Replace("{hair}", subject.GetHairStyle());
                sentence = sentence.Replace("{hairColor}", subject.GetHairColor());
                sentence = sentence.Replace("{gender}", subject.GetGender());
            }
            else
            {
                // LIE
                sentence = sentence.Replace("{hatColor}", subject.GetFakeHatColor());
                sentence = sentence.Replace("{hat}", subject.GetFakeHatName());
                sentence = sentence.Replace("{shirtColor}", subject.GetFakeShirtColor());
                sentence = sentence.Replace("{shirt}", subject.GetFakeShirtName());
                sentence = sentence.Replace("{pantsColor}", subject.GetFakePantsColor());
                sentence = sentence.Replace("{pants}", subject.GetFakePantsName());
                sentence = sentence.Replace("{shoesColor}", subject.GetFakeShoesColor());
                sentence = sentence.Replace("{shoes}", subject.GetFakeShoesName());
                sentence = sentence.Replace("{hair}", subject.GetFakeHairStyle());
                sentence = sentence.Replace("{hairColor}", subject.GetFakeHairColor());
                sentence = sentence.Replace("{gender}", subject.GetFakeGender());
            }
        }

        dialogueText.text = sentence;

        foreach (GameObject button in optionButtons)
        {
            button.SetActive(false);
        }

        if (node.options == null || node.options.Length == 0)
        {
            continueButton.SetActive(true);
            return;
        }

        continueButton.SetActive(false);

        for (int i = 0; i < node.options.Length; i++)
        {
            optionButtons[i].SetActive(true);
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.options[i].optionText;

            int choiceIndex = i;
            Button button = optionButtons[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ChooseOption(choiceIndex));
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
        animator.SetBool("IsOpen", false);
    }
}