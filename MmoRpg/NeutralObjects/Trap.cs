using MmoRpg.Strategies;

namespace MmoRpg.NeutralObjects
{
    public class Trap : NeutralObject
    {
        public Trap()
            : base(new DamageStrategy(30), false)
        {
        }

        public Trap(Trap other)
            : base(other)
        {
        }

        public override string GetObjectType()
        {
            return "Trap";
        }

        public override NeutralObject Clone()
        {
            return new Trap(this);
        }
    }
}