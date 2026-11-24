using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class DamageStrategy : IInteractionStrategy
    {
        private readonly int damage;

        public DamageStrategy(int damage)
        {
            if (damage <= 0)
            {
                throw new ArgumentException("Урон должен быть положительным");
            }

            this.damage = damage;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive)
            {
                return false;
            }

            unit.TakeDamage(damage);

            return true;
        }
    }
}