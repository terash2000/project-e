using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldChecker : MonoBehaviour
{
    public int GoldNeeded;
    [SerializeField] private TextMeshProUGUI _text;

    private Color _originalTextColor;
    private Color _red = new Color(1f, 0f, 0f);

    void Start()
    {
        _originalTextColor = _text.color;
    }

    void Update()
    {
        bool clickable = PlayerData.Gold >= GoldNeeded;
        GetComponent<Button>().interactable = clickable;
        _text.color = clickable ? _originalTextColor : _red;
    }
}
