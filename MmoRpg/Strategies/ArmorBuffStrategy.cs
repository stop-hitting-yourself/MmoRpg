using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class ArmorBuffStrategy : IInteractionStrategy
    {
        private readonly int amount;

        public ArmorBuffStrategy(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Бонус брони должен быть положительным");
            }

            this.amount = amount;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive)
            {
                return false;
            }

            unit.IncreaseArmor(amount);

            return true;
        }
    }
}