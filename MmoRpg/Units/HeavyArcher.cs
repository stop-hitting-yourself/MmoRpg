using System;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public class HeavyArcher : Archer
    {
        public HeavyArcher()
            : base(20, 40, DamageType.Physical)
        {
        }

        public HeavyArcher(HeavyArcher other)
            : base(other)
        {
        }

        public void ChangeDamageType()
        {
            if (UnitAttack.Type == DamageType.Physical)
            {
                UnitAttack.ChangeType(DamageType.Magical);
            }
            else
            {
                UnitAttack.ChangeType(DamageType.Physical);
            }
        }

        public void HealSelf()
        {
            TakeHeal(50);
        }

        public override string GetUnitType()
        {
            return "HeavyArcher";
        }

        public override Unit Clone()
        {
            return new HeavyArcher(this);
        }
    }
}

