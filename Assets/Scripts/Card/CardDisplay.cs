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
        _nameText.text = Card.CardName;
        _descriptionText.text = Card.Description;
        _typeText.text = Card.Type.ToString();
        _artworkImage.sprite = Card.Artwork;
        _manaText.text = Card.ManaCost.ToString();
    }
}
