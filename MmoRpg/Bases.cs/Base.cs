using System;
using System.Collections.Generic;
using MmoRpg.Attributes;
using MmoRpg.Units;
using MmoRpg.Factories;
using MmoRpg.Field;
using MmoRpg.Events;

namespace MmoRpg.Bases
{
    public class Base : IObserver
    {
        private readonly List<Unit> units = new List<Unit>();

        private Health BaseHealth { get; }
        private Armor BaseArmor { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public int X { get; private set; }
        public int Y { get; private set; }

        public GameField? Field { get; private set; }

        public int Resources { get; private set; }
        public int MaxUnits { get; }

        public int UnitCount => units.Count;

        public int CurrentHealth => BaseHealth.Value;
        public int MaxHealth => BaseHealth.MaxValue;
        public int ArmorValue => BaseArmor.Value;

        public bool IsAlive => !BaseHealth.IsZero();

        public Base(int health, int armor, int resources, int maxUnits)
        {
            if (health <= 0)
            {
                throw new ArgumentException("Здоровье базы должно быть положительным");
            }

            if (armor < 0)
            {
                throw new ArgumentException("Броня базы не может быть отрицательной");
            }

            if (resources < 0)
            {
                throw new ArgumentException("Количество ресурсов не может быть отрицательным");
            }

            if (maxUnits < 0)
            {
                throw new ArgumentException("Лимит юнитов не может быть отрицательным");
            }

            BaseHealth = new Health(health);
            BaseArmor = new Armor(armor);

            Resources = resources;
            MaxUnits = maxUnits;

            X = -1;
            Y = -1;
            Field = null;
        }

        public Base(Base other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            BaseHealth = new Health(other.BaseHealth);
            BaseArmor = new Armor(other.BaseArmor);

            Resources = other.Resources;
            MaxUnits = other.MaxUnits;

            X = -1;
            Y = -1;
            Field = null;
        }

        internal void SetPosition(GameField? field, int x, int y)
        {
            Field = field;
            X = x;
            Y = y;
        }

        public bool CreateUnit(UnitFactory factory, int x, int y, int cost)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (cost < 0)
            {
                throw new ArgumentException("Стоимость юнита не может быть отрицательной");
            }

            if (!IsAlive || Field == null)
            {
                return false;
            }

            if (UnitCount >= MaxUnits || Resources < cost)
            {
                return false;
            }

            Unit unit = factory.CreateUnit();

            if (!Field.PlaceUnit(unit, x, y))
            {
                return false;
            }

            if (!RegisterUnit(unit))
            {
                Field.RemoveUnit(unit);

                return false;
            }

            Resources -= cost;

            unit.NotifyCreated();

            return true;
        }

        internal bool UnregisterUnit(Unit unit)
        {
            if (!units.Remove(unit))
            {
                return false;
            }

            unit.Unsubscribe(this);
            unit.SetOwnerBase(null);

            return true;
        }

        internal bool ReplaceOwnedUnit(Unit oldUnit, Unit newUnit)
        {
            int index = units.IndexOf(oldUnit);

            if (index == -1 ||
                oldUnit.OwnerBase != this ||
                newUnit.OwnerBase != null ||
                !newUnit.IsAlive ||
                Field == null ||
                oldUnit.Field != Field)
            {
                return false;
            }

            oldUnit.Unsubscribe(this);
            oldUnit.SetOwnerBase(null);

            units[index] = newUnit;

            newUnit.SetOwnerBase(this);
            newUnit.Subscribe(this);

            return true;
        }

        public bool ContainsUnit(Unit unit)
        {
            return units.Contains(unit);
        }

        public IReadOnlyList<Unit> GetUnits()
        {
            return units.AsReadOnly();
        }

        public void AddResources(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Количество ресурсов не может быть отрицательным");
            }

            Resources = checked(Resources + amount);
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentException("Урон не может быть отрицательным");
            }

            if (!IsAlive)
            {
                return;
            }

            int actualDamage = BaseArmor.ReduceDamage(damage);

            BaseHealth.Decrease(actualDamage);

            if (!IsAlive && Field != null)
            {
                Field.RemoveBase(this);
            }
        }

        public void TakeHeal(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Лечение не может быть отрицательным");
            }

            if (!IsAlive)
            {
                return;
            }

            BaseHealth.Increase(amount);
        }

        public void Update(UnitEvent unitEvent)
        {
            Unit? unit = units.Find(u => u.Id == unitEvent.UnitId);

            if (unitEvent.Type == UnitEventType.Destroyed)
            {
                if (unit != null && !unit.IsAlive)
                {
                    UnregisterUnit(unit);
                }

                Console.WriteLine($"Юнит {unitEvent.UnitId} уничтожен");

                return;
            }

            if (unit == null)
            {
                return;
            }

            switch (unitEvent.Type)
            {
                case UnitEventType.Created:
                    Console.WriteLine($"Создан юнит {unit.GetUnitType()}");
                    break;

                case UnitEventType.Damaged:
                    Console.WriteLine($"Юнит {unit.GetUnitType()} получил {unitEvent.Damage} урона");
                    break;
            }
        }

        internal bool RegisterUnit(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (!IsAlive || !unit.IsAlive ||
                Field == null || unit.Field != Field ||
                unit.OwnerBase != null ||
                UnitCount >= MaxUnits)
            {
                return false;
            }

            units.Add(unit);

            unit.SetOwnerBase(this);
            unit.Subscribe(this);

            return true;
        }

        internal void ClearUnits()
        {
            foreach (Unit unit in units)
            {
                unit.Unsubscribe(this);
                unit.SetOwnerBase(null);
            }

            units.Clear();
        }
    }
}