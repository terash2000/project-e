using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _elementText;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private TextMeshProUGUI _manaText;
    [SerializeField] private GameObject _glowborder;

    private InGameCard _card;

    public InGameCard Card
    {
        get { return _card; }
        set { _card = value; }
    }
    public TextMeshProUGUI ManaText
    {
        get { return _manaText; }
        set { _manaText = value; }
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
        _glowborder.SetActive(true);
    }

    public void Unhighlight()
    {
        _glowborder.SetActive(false);
    }
}
