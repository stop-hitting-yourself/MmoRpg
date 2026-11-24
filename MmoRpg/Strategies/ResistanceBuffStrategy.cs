using System;
using MmoRpg.Units;

namespace MmoRpg.Strategies
{
    public class ResistanceBuffStrategy : IInteractionStrategy
    {
        private readonly int amount;

        public ResistanceBuffStrategy(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Бонус сопротивления должен быть положительным");
            }

            this.amount = amount;
        }

        public bool Apply(Unit unit)
        {
            if (!unit.IsAlive)
            {
                return false;
            }

            if (unit.UnitMagicResistance.Value == 100)
            {
                return false;
            }

            unit.IncreaseMagicResistance(amount);

            return true;
        }
    }
}