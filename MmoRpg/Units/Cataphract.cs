using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public class Cataphract : Cavalry
    {
        public Cataphract() : base(30, 15)
        {
        }

        public Cataphract(Cataphract other) : base(other)
        {
        }

        public Swordsman? Transform()
        {
            if (!UnitHealth.IsZero())
            {
                return null;
            }

            return new Swordsman();
        }

        public override void TakeDamage(int damageValue)
        {
            if (UnitHealth.IsZero())
            {
                return;
            }

            base.TakeDamage(damageValue);

            if (UnitHealth.IsZero() && Field != null)
            {
                Swordsman? swordsman = Transform();

                if (swordsman != null)
                {
                    Field.ReplaceUnit(this, swordsman);
                }
            }
        }

        public override string GetUnitType()
        {
            return "Cataphract";
        }

        public override Unit Clone()
        {
            return new Cataphract(this);
        }
    }
}
