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

        _nameText.text = _card.BaseCard.CardName;
        _typeText.text = _card.BaseCard.Type.ToString();
        _elementText.text = _card.Element.ToString();
        _artworkImage.sprite = _card.BaseCard.Artwork;
        _manaText.text = _card.ManaCost.ToString();
        RenderDescriptionText();

        _nameBorder.color = _card.IsUpgraded ? _golden : _white;
        if (_card.IsUpgraded) _nameText.text += "+";
    }

    private void RenderDescriptionText()
    {
        _descriptionText.text = _card.BaseCard.Description;

        // damage
        int damage = _card.GetRealDamage();
        if (_descriptionText.text.IndexOf("_d_") != -1)
        {
            _descriptionText.text = _descriptionText.text.Replace("_d_", damage.ToString());
        }
        else if (damage > 0)
        {
            _descriptionText.text += $" Deal {damage} damage.";
        }

        // block
        if (_descriptionText.text.IndexOf("_bl_") != -1)
        {
            _descriptionText.text = _descriptionText.text.Replace("_bl_", _card.Damage.ToString());
        }

        // status
        Dictionary<Status.Type, int> statuses = new Dictionary<Status.Type, int>();
        foreach (Status status in Card.Statuses)
        {
            if (statuses.ContainsKey(status.type))
            {
                statuses[status.type] += status.value;
            }
            else
            {
                statuses.Add(status.type, status.value);
            }
        }
        if (statuses.ContainsKey(Status.Type.Burn))
        {
            if (_descriptionText.text.IndexOf("_b_") != -1)
            {
                _descriptionText.text = _descriptionText.text.Replace("_b_", statuses[Status.Type.Burn].ToString());
            }
            else
            {
                _descriptionText.text += $" Give {statuses[Status.Type.Burn]} Burn.";
            }
        }
        if (statuses.ContainsKey(Status.Type.Acid))
        {
            if (_descriptionText.text.IndexOf("_a_") != -1)
            {
                _descriptionText.text = _descriptionText.text.Replace("_a_", statuses[Status.Type.Acid].ToString());
            }
            else
            {
                _descriptionText.text += $" Give {statuses[Status.Type.Acid]} Acid.";
            }
        }
        if (statuses.ContainsKey(Status.Type.Stun)
                && _card.BaseCard.Statuses.FindAll(status => status.type == Status.Type.Stun).Count == 0)
        {
            _descriptionText.text += $" Stun.";
        }

        // radius
        if (_descriptionText.text.IndexOf("_rd_") != -1)
        {
            _descriptionText.text = _descriptionText.text.Replace("_rd_", _card.Radius.ToString());
        }
        else if (_descriptionText.text.IndexOf("_di_") != -1)
        {
            _descriptionText.text = _descriptionText.text.Replace("_di_", (_card.Radius * 2 + 1).ToString());
        }
        else
        {
            int additionalRadius = _card.Radius - _card.BaseCard.Radius;
            if (additionalRadius > 0)
            {
                _descriptionText.text += $" Area +{additionalRadius}";
            }
        }

        // range
        if (_descriptionText.text.IndexOf("_rn_") != -1)
        {
            _descriptionText.text =
                _descriptionText.text.Replace("_rn_", _card.CastRange.ToString() + " tile" + (_card.CastRange > 1 ? "s" : ""));
        }
        else
        {
            int additionalRange = _card.CastRange - _card.BaseCard.CastRange;
            if (additionalRange > 0)
            {
                _descriptionText.text += $" Range +{additionalRange}";
            }
        }
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
