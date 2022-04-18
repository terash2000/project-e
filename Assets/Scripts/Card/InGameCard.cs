using UnityEngine;

public class InGameCard
{
    private Card _baseCard;
    private ElementType _element;
    private int _manaCost;
    private int _damage;
    private bool _isUpgraded = false;
    public bool _isToken = false;

    public Card BaseCard
    {
        get { return _baseCard; }
    }
    public ElementType Element
    {
        get { return _element; }
        set { _element = value; }
    }
    public int ManaCost
    {
        get { return _manaCost; }
    }
    public int Damage
    {
        get { return _damage; }
    }
    public bool IsUpgraded
    {
        get { return _isUpgraded; }
    }
    public bool IsToken
    {
        get { return _isToken; }
        set { _isToken = value; }
    }

    public InGameCard(Card card, bool upgrade = false)
    {
        _baseCard = card;
        _manaCost = card.ManaCost;
        _damage = card.Damage;
        Element = card.BaseElement;
        if (upgrade) Upgrade();
    }

    public InGameCard(InGameCard other)
    {
        _baseCard = other.BaseCard;
        _manaCost = other.ManaCost;
        Element = other.Element;
        _isUpgraded = other._isUpgraded;
        _isToken = other._isToken;
    }

    public bool Upgrade()
    {
        if (!_isUpgraded)
        {
            _isUpgraded = true;
            foreach (Bonus bonus in BaseCard.UpgradeBonus)
            {
                GainBonus(bonus);
            }
            return true;
        }
        return false;
    }

    public void GainBonus(Bonus bonus)
    {
        switch (bonus.type)
        {
            case Bonus.Type.Mana:
                _manaCost += bonus.amount;
                if (_manaCost < 0) _manaCost = 0;
                break;
            case Bonus.Type.Damage:
                _damage += bonus.amount;
                break;
        }
    }

    public bool IsCastable()
    {
        return ManaCost <= PlayerData.Mana;
    }
}
