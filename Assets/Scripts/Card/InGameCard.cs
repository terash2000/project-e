using UnityEngine;

public class InGameCard
{
    public int ManaCost;
    public ElementType Element;

    private Card _baseCard;
    private bool _isUpgrade = false;

    public Card BaseCard
    {
        get { return _baseCard; }
    }

    public InGameCard(Card card, bool upgrade = false)
    {
        _baseCard = card;
        ManaCost = card.ManaCost;
        Element = card.BaseElement;
        if (upgrade) Upgrade();
    }

    public bool Upgrade()
    {
        if (!_isUpgrade)
        {
            _isUpgrade = true;
            return true;
        }
        return false;
    }
}
