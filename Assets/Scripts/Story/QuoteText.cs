using System.Collections;
using UnityEngine;

public class QuoteText : MonoBehaviour
{
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

    private IEnumerator BuildText()
    {
        while (IsTyping())
        {
            yield return new WaitForSeconds(0.01f);
            currentCharacterIndex += 1;
        }
    }
}
