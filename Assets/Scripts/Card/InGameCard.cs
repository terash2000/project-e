using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCard
{
    private Card _baseCard;
    private ElementType _element;

    private int _additionalManaCost = 0;
    private int _additionalDamage = 0;
    private int _additionalRadius = 0;
    private int _additionalCastRange = 0;
    private List<Status> _additionalStatuses = new List<Status>();
    private List<Status> _additionalGainStatuses = new List<Status>();

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
    public List<Status> Statuses
    {
        get { return _baseCard.Statuses.Concat(_additionalStatuses).ToList(); }
    }
    public List<Status> GainStatuses
    {
        get { return _baseCard.GainStatuses.Concat(_additionalGainStatuses).ToList(); }
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
        _additionalDamage = other._additionalDamage;
        _additionalRadius = other._additionalRadius;
        _additionalCastRange = other._additionalCastRange;
        _additionalStatuses = new List<Status>(other._additionalStatuses);
        _additionalStatuses = new List<Status>(other._additionalGainStatuses);


        _isUpgraded = other._isUpgraded;
        _isToken = other._isToken;
    }

    public int GetRealDamage()
    {
        int realDamage = Damage;

        if (SceneManager.GetActiveScene().name == "CombatScene")
        {
            Dictionary<Status.Type, int> statuses = PlayerManager.Instance.Player.StatusDict;
            if (statuses.ContainsKey(Status.Type.Strong))
                realDamage = (int)(realDamage * Status.STRONG_DAMAGE_MULTIPLIER);
            else if (statuses.ContainsKey(Status.Type.Weak))
                realDamage = (int)(realDamage * Status.WEAK_DAMAGE_MULTIPLIER);
        }

        return realDamage;
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

    public List<InGameCard> GainBonus(Bonus bonus)
    {
        List<InGameCard> createdCards = new List<InGameCard>();
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

            case Bonus.Type.AddStatus:
                _additionalStatuses.Add(new Status(bonus.statusType, bonus.amount));
                break;

            case Bonus.Type.CreateCard:
                for (int i = 0; i < bonus.amount; i++)
                {
                    InGameCard createdCard = new InGameCard(bonus.card);
                    createdCard._isToken = true;
                    createdCards.Add(createdCard);
                }
                break;

            default:
                break;
        }
        return createdCards;
    }

    public List<InGameCard> GainComboBonus(Combo combo)
    {
        Element = combo.Result;
        List<Bonus> bonuses;
        List<InGameCard> createdCards = new List<InGameCard>();

        if (BaseCard.Type == CardType.Attack)
        {
            bonuses = combo.AttackCardBonuses;
        }
        else bonuses = combo.SkillCardBonuses;

        foreach (Bonus bonus in bonuses)
        {
            createdCards.AddRange(GainBonus(bonus));
        }

        return createdCards;
    }

    public bool IsCastable()
    {
        return ManaCost <= PlayerData.Mana;
    }
}
