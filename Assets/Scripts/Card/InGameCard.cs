using System.Collections.Generic;
using UnityEngine;

public class InGameCard
{
    private Card _baseCard;
    private ElementType _element;
    private int _additionalManaCost = 0;
    private int _additionalDamage = 0;
    private int _additionalRadius = 0;
    private int _additionalCastRange = 0;
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
        get { return _baseCard.ManaCost + _additionalManaCost; }
    }
    public int Damage
    {
        get { return _baseCard.Damage + _additionalDamage; }
    }
    public int Radius
    {
        get { return _baseCard.Radius + _additionalRadius; }
    }
    public int CastRange
    {
        get { return _baseCard.CastRange + _additionalCastRange; }
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
        _element = card.BaseElement;
        if (upgrade) Upgrade();
    }

    public InGameCard(InGameCard other)
    {
        _baseCard = other._baseCard;
        _element = other._element;
        _additionalManaCost = other._additionalManaCost;
        _additionalManaCost = other._additionalManaCost;
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
                _additionalManaCost += bonus.amount;
                if (ManaCost < 0) _additionalManaCost = -_baseCard.ManaCost;
                break;
            case Bonus.Type.Damage:
                _additionalDamage += bonus.amount;
                if (Damage < 0) _additionalDamage = -_baseCard.Damage;
                break;
            case Bonus.Type.Radius:
                _additionalRadius += bonus.amount;
                if (Radius < 0) _additionalRadius = -_baseCard.Radius;
                break;
            case Bonus.Type.CastRange:
                _additionalCastRange += bonus.amount;
                if (CastRange < 0) _additionalCastRange = -_baseCard.CastRange;
                break;
        }
    }

    public void GainComboBonus(Combo combo)
    {
        Element = combo.Result;
        List<Bonus> bonuses;
        if (BaseCard.Type == CardType.Attack)
        {
            bonuses = combo.AttackCardBonuses;
        }
        else
        {
            bonuses = combo.SkillCardBonuses;
        }

        foreach (Bonus bonus in bonuses)
        {
            GainBonus(bonus);
        }
    }

    public bool IsCastable()
    {
        return ManaCost <= PlayerData.Mana;
    }
}
