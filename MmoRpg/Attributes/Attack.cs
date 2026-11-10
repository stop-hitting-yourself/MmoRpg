using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoRpg.Attributes
{
    public class Attack
    {
        public int Value { get; private set; }
        public DamageType Type { get; private set; }

        public Attack(int attackValue, DamageType damageType)
        {
            if (attackValue < 0)
            {
                throw new ArgumentException("Начальное значение атаки не может быть отрицательным");
            }

            if (!Enum.IsDefined(damageType))
            {
                throw new ArgumentException("Недопустимый тип урона");
            }

            Value = attackValue;
            Type = damageType;
        }

        public Attack(Attack other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other", "Нельзя скопировать атаку из объекта со значением null");
            }

            Value = other.Value;
            Type = other.Type;
        }
        public void ChangeType(DamageType damageType)
        {
            if (!Enum.IsDefined(damageType))
            {
                throw new ArgumentException("Недопустимый тип урона");
            }
            Type = damageType;
        }
        public void Decrease(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Величина уменьшения атаки не может быть отрицательной");
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
                throw new ArgumentException("Величина увеличения атаки не может быть отрицательной");
            }

            Value += amount;
        }

        public int CalculateDamage(Armor targetArmor, MagicResistance targetResistance)
        {
            if (Type == DamageType.Physical)
            {
                return targetArmor.ReduceDamage(Value);
            }
            else
            {
                return targetResistance.ReduceDamage(Value);
            }
        }
    }
}
