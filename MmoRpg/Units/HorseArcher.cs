using System;
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
            if (IsAlive)
            {
                return null;
            }

            return new HeavyArcher();
        }

        protected override void OnDestroyed()
        {
            if (Field != null)
            {
                HeavyArcher? heavyArcher = Transform();

                if (heavyArcher != null)
                {
                    Field.ReplaceUnit(this, heavyArcher);
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


