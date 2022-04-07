using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;
    [SerializeField]
    private TextMeshProUGUI _typeText;
    [SerializeField]
    private TextMeshProUGUI _elementText;
    [SerializeField]
    private Image _artworkImage;

    [SerializeField]
    private TextMeshProUGUI _manaText;

    private Card _card;

    public Card Card
    {
        get { return _card; }
        set { _card = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_card == null)
        {
            _nameText.text = "";
            _descriptionText.text = "";
            _typeText.text = "";
            _manaText.text = "";
            return;
        }

        _nameText.text = Card.CardName;
        _descriptionText.text = Card.Description;
        _typeText.text = Card.Type.ToString();
        _elementText.text = Card.BaseElement.ToString();
        _artworkImage.sprite = Card.Artwork;
        _manaText.text = Card.ManaCost.ToString();
    }
}
