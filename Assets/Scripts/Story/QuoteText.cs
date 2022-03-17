using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuoteText : MonoBehaviour
{
    public string dialog;
    private int currentCharacterIndex;
	
	void Update () {
        GetComponent<TMPro.TextMeshProUGUI>().text = dialog.Substring(0, currentCharacterIndex);
	}

    public void StartTyping(){
        currentCharacterIndex = 0;
        StartCoroutine(BuildText());
    }

    public bool IsTyping() {
        return currentCharacterIndex < dialog.Length;
    }

    IEnumerator BuildText()
    {
        currentCharacterIndex = 0;
        while(IsTyping()){
            currentCharacterIndex += 1;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
