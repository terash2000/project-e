using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image _nameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private Image _elementImage;
    [SerializeField] private TextMeshProUGUI _elementText;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private TextMeshProUGUI _manaText;

    private Color _green = new Color(0f, 1f, 0f);
    private Color _white = new Color(1f, 1f, 1f);
    private InGameCard _card;

    public InGameCard Card
    {
        get { return _card; }
        set { _card = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        render();
    }

    public void render()
    {
        if (_card == null)
        {
            _nameText.text = "";
            _descriptionText.text = "";
            _typeText.text = "";
            _manaText.text = "";
            return;
        }

        _nameText.text = Card.BaseCard.CardName;
        _descriptionText.text = Card.BaseCard.Description;
        _typeText.text = Card.BaseCard.Type.ToString();
        _elementText.text = Card.Element.ToString();
        _artworkImage.sprite = Card.BaseCard.Artwork;
        _manaText.text = Card.ManaCost.ToString();
    }

    public void Highlight()
    {
        GetComponent<Image>().color = _green;
        _nameImage.color = _green;
        _elementImage.color = _green;
    }

    public void Unhighlight()
    {
        GetComponent<Image>().color = _white;
        _nameImage.color = _white;
        _elementImage.color = _white;
    }
}
