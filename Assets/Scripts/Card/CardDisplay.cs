using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Image _nameBorder;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _elementText;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private TextMeshProUGUI _manaText;
    [SerializeField] private GameObject _glowborder;

    private Color _golden = new Color(1f, 0.9f, 0f);
    private Color _white = new Color(1f, 1f, 1f);

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
        Render();
    }

    public void Render()
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
        _descriptionText.text = Card.BaseCard.Type == CardType.Attack ? createAttackDescribtion() : Card.BaseCard.Description;
        _typeText.text = Card.BaseCard.Type.ToString();
        _elementText.text = Card.Element.ToString();
        _artworkImage.sprite = Card.BaseCard.Artwork;
        _manaText.text = Card.ManaCost.ToString();

        _nameBorder.color = Card.IsUpgraded ? _golden : _white;
    }

    public void Highlight()
    {
        _glowborder.SetActive(true);
    }

    public void Unhighlight()
    {
        _glowborder.SetActive(false);
    }

    private string createAttackDescribtion()
    {
        string description = "";
        if (Card.Damage != 0 && Card.Effects.Count != 0)
        {
            string statuses = createStatusDescribtion();
            description += $"Deal {Card.Damage} and give {statuses} ";
        }
        else if (Card.Damage != 0)
        {
            description += $"Deal {Card.Damage} ";
        }
        else if (Card.Effects.Count != 0)
        {
            string statuses = createStatusDescribtion();
            description += $"Give {statuses} ";
        }
        description += createAreaDescribtion();
        return description;
    }

    private string createAreaDescribtion()
    {
        switch (Card.BaseCard.TargetShape)
        {
            case AreaShape.Single:
                return "to an enemy";
            case AreaShape.Line:
                return $"to all enemies in {Card.CastRange} straight line";
            case AreaShape.Hexagon:
                return "to all enemies in circle area";
            case AreaShape.Cone:
                return "to all enemies in cone area";
            default:
                return "";
        }
    }

    private string createStatusDescribtion()
    {
        string description = "";
        foreach (Status status in Card.Effects)
        {
            description += $"{status.value} {status.type}";
        }
        return description;
    }
}
