using MmoRpg.Strategies;

namespace MmoRpg.NeutralObjects
{
    public class HealingPotion : NeutralObject
    {
        public HealingPotion()
            : base(new HealStrategy(50), false)
        {
        }

        public HealingPotion(HealingPotion other)
            : base(other)
        {
        }

        public override string GetObjectType()
        {
            return "HealingPotion";
        }

        public override NeutralObject Clone()
        {
            return new HealingPotion(this);
        }
    }
}