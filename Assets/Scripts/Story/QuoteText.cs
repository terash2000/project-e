using System.Collections;
using UnityEngine;

public class QuoteText : MonoBehaviour
{
    private const float TYPING_TIME = 0.01f;

    private string _dialog;
    private int _currentCharacterIndex;

    public string Dialog
    {
        get { return _dialog; }
        set { _dialog = value; }
    }

    void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = _dialog.Substring(0, _currentCharacterIndex);
    }

    public void StartTyping()
    {
        _currentCharacterIndex = 0;
        StartCoroutine(BuildText());
    }

    public bool IsTyping()
    {
        return _currentCharacterIndex < _dialog.Length;
    }

    public void SkipTyping()
    {
        _currentCharacterIndex = _dialog.Length;
    }

    private IEnumerator BuildText()
    {
        while (IsTyping())
        {
            yield return new WaitForSeconds(TYPING_TIME);
            _currentCharacterIndex += 1;
        }
        _currentCharacterIndex = _dialog.Length;
    }
}
