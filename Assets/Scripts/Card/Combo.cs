using System;
using System.Collections.Generic;

[Serializable]
public class Combo
{
    public ElementType Base1;
    public ElementType Base2;
    public ElementType Result;
    public List<Bonus> AttackCardBonuses;
    public List<Bonus> SkillCardBonuses;
}
