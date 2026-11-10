using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoRpg.Attributes
{
    public class Health
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        public Health(int healthValue)
        {
            if (healthValue < 0)
            {
                throw new ArgumentException("Начальное значение здоровья не может быть отрицательным");
            }

            Value = healthValue;
            MaxValue = healthValue;
        }

        public Health(Health other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other", "Нельзя скопировать здоровье из объекта со значением null");
            }

            Value = other.Value;
            MaxValue = other.MaxValue;
        }

        public void Decrease(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Урон не может быть отрицательным");
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

        public void Increase(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Объём лечения не может быть отрицательным");
            }

            if (amount >= MaxValue - Value)
            {
                Value = MaxValue;
            }
            else
            {
                Value += amount;
            }
        }

        public bool IsZero()
        {
            return Value == 0;
        }
    }
}

