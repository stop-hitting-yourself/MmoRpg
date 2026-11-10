using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public abstract class Cavalry : Unit
    {
        protected Cavalry(int attackValue, int resistanceValue) : base( new Health(200),new Armor(20), new Attack(attackValue, DamageType.Physical),new MagicResistance(resistanceValue))
        {
        }

        protected Cavalry(Cavalry other) : base(other)
        {
        }
    }
}
