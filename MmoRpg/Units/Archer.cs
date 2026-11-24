using System;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public abstract class Archer : Unit
    {
        protected Archer(int armorValue, int attackValue, DamageType damageType): base(new Health(100),new Armor(armorValue), new Attack(attackValue, damageType), new MagicResistance(5))
        {
        }

        protected Archer(Archer other) : base(other)
        {
        }
    }
}