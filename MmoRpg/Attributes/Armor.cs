using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MmoRpg.Attributes
{
    public class Armor
    {
        public int Value { get; private set; }

        public Armor(int armorValue)
        {
            Value = armorValue;
        }

        public Armor(Armor other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other", "Нельзя скопировать броню из объекта со значением null");
            }

            Value = other.Value;
        }

        public void Decrease(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Величина уменьшения брони не может быть отрицательной");
            }

            Value -= amount;
        }

        public void Increase(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Величина увеличения брони не может быть отрицательной");
            }

            Value += amount;
        }

        public int ReduceDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentException("Урон не может быть отрицательным");
            }

            double coefficient = 1.0 - (0.06 * Value) / (1.0 + 0.06 * Math.Abs((double)Value));

            return (int)Math.Round(damage * coefficient);
        }
    }
}
