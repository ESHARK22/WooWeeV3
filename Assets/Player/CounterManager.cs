using TMPro;
using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public GameObject counterParent;

    void Start()
    {
        if (MainMenu.endlessMode)
        {
            counterParent.SetActive(true);
        }
        else
        {
            counterParent.SetActive(false);
        }
    }

    void Update()
    {
        if (MainMenu.endlessMode)
        {
            counterText.text = GameResult.currentLoop.ToString();
        }
    }
}