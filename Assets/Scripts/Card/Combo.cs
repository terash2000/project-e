using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Combo : ScriptableObject
{
    public ElementType Base1;
    public ElementType Base2;
    public ElementType Result;
    public List<Bonus> AttackCardBonuses;
    public List<Bonus> SkillCardBonuses;
    public string Equation;
    public string Description;
    public ReactionType Type;

    public enum ReactionType
    {
        Normal,
        Combustion,
        Nuclear
    }
}
