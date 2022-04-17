using UnityEngine;

public class InGameCard
{
    public int ManaCost;
    public ElementType Element;

    private Card _baseCard;
    private bool _isUpgraded = false;
    public bool _isToken = false;

    public Card BaseCard
    {
        get { return _baseCard; }
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
        ManaCost = card.ManaCost;
        Element = card.BaseElement;
        if (upgrade) Upgrade();
    }

    public InGameCard(InGameCard other)
    {
        _baseCard = other.BaseCard;
        ManaCost = other.ManaCost;
        Element = other.Element;
        _isUpgraded = other._isUpgraded;
        _isToken = other._isToken;
    }

    public bool Upgrade()
    {
        if (!_isUpgraded)
        {
            _isUpgraded = true;
            // TODO upgrade card
            //ManaCost -= 1;
            return true;
        }
        return false;
    }
}
