using System.Collections;
using UnityEngine;

public class QuoteText : MonoBehaviour
{
    private const float TYPING_TIME = 0.01f;

    public string dialog;
    private int currentCharacterIndex;

    void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = dialog.Substring(0, currentCharacterIndex);
    }

    public void StartTyping()
    {
        currentCharacterIndex = 0;
        StartCoroutine(BuildText());
    }

    public bool IsTyping()
    {
        return currentCharacterIndex < dialog.Length;
    }

    public void SkipTyping()
    {
        currentCharacterIndex = dialog.Length - 1;
    }

    private IEnumerator BuildText()
    {
        while (IsTyping())
        {
            yield return new WaitForSeconds(TYPING_TIME);
            currentCharacterIndex += 1;
        }
    }
}
