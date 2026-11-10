using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoRpg.Attributes
{
    public class MagicResistance
    {
        public int Value { get; private set; }

        public MagicResistance(int resistanceValue)
        {
            if (resistanceValue < 0 || resistanceValue > 100)
            {
                throw new ArgumentException("Магическое сопротивление должно быть от 0 до 100 включительно");
            }

            Value = resistanceValue;
        }

        public MagicResistance(MagicResistance other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other", "Нельзя скопировать магическое сопротивление из объекта со значением null");
            }

            Value = other.Value;
        }

        public void Increase(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Величина увеличения магического сопротивления не может быть отрицательной");
            }

            if (amount >= 100 - Value)
            {
                Value = 100;
            }
            else
            {
                Value += amount;
            }
        }

        public void Decrease(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Величина уменьшения магического сопротивления не может быть отрицательной");
            }

            if (amount >= Value)
            {
                Value = 0;
            }
            else
            {
                Value -= amount;
            }
        }

        public int ReduceDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentException("Урон не может быть отрицательным");
            }

            double coefficient = 1.0 - Value / 100.0;

            return (int)Math.Round(damage * coefficient);
        }
    }
}
