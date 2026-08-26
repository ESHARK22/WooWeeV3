using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string characterName;
    public DialogueNode startingNode;
    
}

/* [System.Serializable]
public class DialogueNode
{
    public string id;

    [TextArea(3,10)]
    public string sentence;
    public DialogueOption[] options;
} */


[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
}
