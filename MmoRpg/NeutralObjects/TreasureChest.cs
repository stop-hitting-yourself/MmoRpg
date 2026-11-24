using MmoRpg.Strategies;

namespace MmoRpg.NeutralObjects
{
    public class TreasureChest : NeutralObject
    {
        public TreasureChest()
            : base(new ResourceStrategy(50), false)
        {
        }

        public TreasureChest(TreasureChest other)
            : base(other)
        {
        }

        public override string GetObjectType()
        {
            return "TreasureChest";
        }

        public override NeutralObject Clone()
        {
            return new TreasureChest(this);
        }
    }
}