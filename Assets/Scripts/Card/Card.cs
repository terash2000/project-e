using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string CardName;
    public string Description;
    public CardType Type;
    public ElementType BaseElement;
    public CardRarity Rarity;
    public Sprite Artwork;
    public int ManaCost;
    public bool IsUnlocked;
    public AreaShape AreaShape;
    public AreaShape TargetShape;
    public int CastRange;
    public int Radius;
    public int Damage;
    public List<Status> Statuses;
    public List<Status> GainStatuses;
    public List<CardEffect> Effects;
    public List<Bonus> UpgradeBonus;
    public AudioClip PlaySound;
}
