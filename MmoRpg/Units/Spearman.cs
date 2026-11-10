using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoRpg.Units
{
    public class Spearman : Infantry
    {
        public bool ShieldRaised { get; private set; }

        public Spearman()
            : base(150, 15)
        {
            ShieldRaised = false;
        }

        public Spearman(Spearman other)
            : base(other)
        {
            ShieldRaised = other.ShieldRaised;
        }

        public void RaiseShield()
        {
            if (ShieldRaised)
            {
                return;
            }

            IncreaseArmor(20);
            ShieldRaised = true;
        }

        public void LowerShield()
        {
            if (!ShieldRaised)
            {
                return;
            }

            DecreaseArmor(20);
            ShieldRaised = false;
        }

        public override string GetUnitType()
        {
            return "Spearman";
        }

        public override Unit Clone()
        {
            return new Spearman(this);
        }
    }
}
