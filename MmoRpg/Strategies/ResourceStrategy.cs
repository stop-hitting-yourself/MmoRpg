using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class ResourceStrategy : IInteractionStrategy
    {
        private readonly int amount;

        public ResourceStrategy(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Количество ресурсов должно быть положительным");
            }

            this.amount = amount;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive || unit.OwnerBase == null)
            {
                return false;
            }

            if (!unit.OwnerBase.IsAlive)
            {
                return false;
            }

            unit.OwnerBase.AddResources(amount);

            return true;
        }
    }
}