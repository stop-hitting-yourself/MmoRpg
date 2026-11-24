using System;
using MmoRpg.Attributes;

namespace MmoRpg.Units
{
    public class LightArcher : Archer
    {

        private int damagePenalty;
        public LightArcher(): base(5, 30, DamageType.Physical)
        {
        }

        public LightArcher(LightArcher other): base(other)
        {
            damagePenalty = other.damagePenalty;
        }

        public void ChangeDamageType()
        {
            if (!IsAlive)
            {
                return;
            }

            if (UnitAttack.Type == DamageType.Physical)
            {
                damagePenalty = Math.Min(10, UnitAttack.Value);

                DecreaseAttack(damagePenalty);

                UnitAttack.ChangeType(DamageType.Magical);
            }
            else
            {
                IncreaseAttack(damagePenalty);

                UnitAttack.ChangeType(DamageType.Physical);

                damagePenalty = 0;
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

