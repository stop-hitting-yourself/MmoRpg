using System;

namespace MmoRpg.Units
{
    public class Swordsman : Infantry
    {
        public Swordsman()
            : base(125, 20)
        {
        }

        public Swordsman(Swordsman other)
            : base(other)
        {
        }

        public override string GetUnitType()
        {
            return "Swordsman";
        }

        public override Unit Clone()
        {
            return new Swordsman(this);
        }
    }
}
