using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public abstract class Infantry : Unit
    {
        protected Infantry(int healthValue, int attackValue): base( new Health(healthValue),new Armor(15),new Attack(attackValue, DamageType.Physical), new MagicResistance(10))
        {
        }

        protected Infantry(Infantry other) : base(other)
        {
        }
    }
}