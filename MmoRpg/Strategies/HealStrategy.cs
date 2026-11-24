using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class HealStrategy : IInteractionStrategy
    {
        private readonly int amount;

        public HealStrategy(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Количество лечения должно быть положительным");
            }

            this.amount = amount;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive)
            {
                return false;
            }

            if (unit.CurrentHealth == unit.MaxHealth)
            {
                return false;
            }

            unit.TakeHeal(amount);

            return true;
        }
    }
}