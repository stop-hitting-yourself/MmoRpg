using System;
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
            if (IsAlive)
            {
                return null;
            }

            return new Swordsman();
        }

        protected override void OnDestroyed()
        {
            if (Field != null)
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
