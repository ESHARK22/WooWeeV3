using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{   
    public GameObject Player;
    public AudioClip dialogueSound;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private CharacterIdentity targetCharacter;

    public GameObject dialoguePanel;
    public GameObject continueButton;

    public GameObject[] optionButtons;

    public Animator animator;

    private Dialogue currentDialogue;
    private DialogueNode currentNode;

    private string[] genders =
    {
        "male",
        "female"
    };

    private string[] expressions =
    {
        "neutral",
        "happy",
        "anger",
        "sad",
        "blush",
        "shock"
    };

    private string[] colors =
    {
        "Red",
        "Blue",
        "Green",
        "Black",
        "White",
        "Brown",
        "Yellow",
        "Purple",
        "Orange",
        "Navy",
        "Pink"
    };

    private string[] hairOptions =
    {
        "flat_top_fade",
        "bangs",
        "bob",
        "curtains",
        "messy1",
        "long",
        null
    };

    private string[] shirtOptions =
    {
        "shortsleeve",
        "longsleeve",
        "overalls"
    };

    private string[] pantsOptions =
    {
        "pants",
        "pantaloons",
        "hose",
        "leggings"
    };

    private string[] shoesOptions =
    {
        "boots/rimmed",
        "boots/basic",
        "shoes/basic",
        "slippers"
    };

    private string[] hatOptions =
    {
        "bandana",
        "hood",
        "leather_cap",
        "tophat",
        "wizard"
    };

    string GetRandomWrongValue(string[] values, string correctValue)
{
    string randomValue;

    do
    {
        randomValue = values[Random.Range(0, values.Length)];
    }
    while (randomValue == correctValue);

    return randomValue;
}

    public void StartDialogue(Dialogue dialogue, CharacterIdentity target)
    {
        AudioSource.PlayClipAtPoint(dialogueSound, Player.transform.position);
        animator.SetBool("IsOpen", true );

        targetCharacter = target;

        currentDialogue = dialogue;
        currentNode = dialogue.startingNode;

        nameText.text = dialogue.characterName;

        DisplayNode(currentNode);
    }

    void DisplayNode(DialogueNode node)
    {
        currentNode = node;

        string sentence = node.sentence;

        if (targetCharacter != null && targetCharacter.truthTeller)
        {
            //sentence = sentence.Replace("{hat}", targetCharacter.GetHatName());
            sentence = sentence.Replace("{hatColor}", targetCharacter.GetHatColor());

            //sentence = sentence.Replace("{shirt}", targetCharacter.GetShirtName());
            sentence = sentence.Replace("{shirtColor}", targetCharacter.GetShirtColor());
 
            //sentence = sentence.Replace("{pants}", targetCharacter.GetPantsName());
            //sentence = sentence.Replace("{pantsColor}", targetCharacter.GetPantsColor());

            //sentence = sentence.Replace("{shoesColor}", targetCharacter.GetShoesColor());

            sentence = sentence.Replace("{hair}", targetCharacter.GetHairStyle());
            sentence = sentence.Replace("{hairColor}", targetCharacter.GetHairColor());

            sentence = sentence.Replace("{gender}", targetCharacter.GetGender());
        }

        if (targetCharacter != null && ! targetCharacter.truthTeller)
        {
            sentence = sentence.Replace("{hatColor}", GetRandomWrongValue(colors, targetCharacter.GetHatColor()));
            sentence = sentence.Replace("{shirtColor}", GetRandomWrongValue(colors, targetCharacter.GetHatColor()));
            sentence = sentence.Replace("{hair}", GetRandomWrongValue(hairOptions, targetCharacter.GetHairStyle()));
            sentence = sentence.Replace("{hairColor}", GetRandomWrongValue(colors, targetCharacter.GetHairColor()));

        }

        dialogueText.text = sentence;


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
    }
}