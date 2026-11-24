using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public interface IInteractionStrategy
    {
        bool Apply(Unit unit);
    }
}