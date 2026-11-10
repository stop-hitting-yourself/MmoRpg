using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public class LightArcher : Archer
    {
        public LightArcher()
            : base(5, 30, DamageType.Physical)
        {
        }

        public LightArcher(LightArcher other)
            : base(other)
        {
        }

        public void ChangeDamageType()
        {
            if (UnitAttack.Type == DamageType.Physical)
            {
                UnitAttack.ChangeType(DamageType.Magical);
                DecreaseAttack(10);
            }
            else
            {
                UnitAttack.ChangeType(DamageType.Physical);
                IncreaseAttack(10);
            }
        }

        public override string GetUnitType()
        {
            return "LightArcher";
        }

        public override Unit Clone()
        {
            return new LightArcher(this);
        }
    }
}

