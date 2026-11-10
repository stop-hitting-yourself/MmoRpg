using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public class HorseArcher : Cavalry
    {
        public HorseArcher() : base(40, 15)
        {
        }

        public HorseArcher(HorseArcher other) : base(other)
        {
        }

        public HeavyArcher? Transform()
        {
            if (!UnitHealth.IsZero())
            {
                return null;
            }

            return new HeavyArcher();
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
                HeavyArcher? archer = Transform();

                if (archer != null)
                {
                    Field.ReplaceUnit(this, archer);
                }
            }
        }

        public override string GetUnitType()
        {
            return "HorseArcher";
        }

        public override Unit Clone()
        {
            return new HorseArcher(this);
        }
    }
}


