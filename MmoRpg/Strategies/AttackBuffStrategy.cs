using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class AttackBuffStrategy : IInteractionStrategy
    {
        private readonly int amount;

        public AttackBuffStrategy(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Бонус атаки должен быть положительным");
            }

            this.amount = amount;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive)
            {
                return false;
            }

            unit.IncreaseAttack(amount);

            return true;
        }
    }
}