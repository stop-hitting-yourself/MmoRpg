using System;
using MmoRpg.Attributes;
using MmoRpg.Field;
using System.Collections.Generic;
using MmoRpg.Events;
using MmoRpg.Bases;
using MmoRpg.Terrains;

namespace MmoRpg.Units
{
    public abstract class Unit : IObservable
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private readonly List<IObserver> observers = new List<IObserver>();
        public GameField? Field { get; private set; }
        public Base? OwnerBase { get; private set; }

        public Guid Id { get; } = Guid.NewGuid();

        public double MaxMovementPoints => 4.0;
        public double MovementPoints { get; private set; } = 4.0;

        private Health UnitHealth { get; }
        public int CurrentHealth => UnitHealth.Value;
        public int MaxHealth => UnitHealth.MaxValue;
        public bool IsAlive => !UnitHealth.IsZero();

        public Armor UnitArmor { get; private set; }
        public Attack UnitAttack { get; private set; }
        public MagicResistance UnitMagicResistance { get; private set; }

        public int EffectiveArmor
        {
            get
            {
                int bonus = 0;

                if (Field != null)
                {
                    ITerrain? terrain = Field.GetTerrain(X, Y);

                    if (terrain != null)
                    {
                        bonus = terrain.GetDefenseBonus(this);
                    }
                }

                return (UnitArmor.Value + bonus);
            }
        }
        
        public double SpeedMultiplier
        {
            get
            {
                if (Field != null)
                {
                    ITerrain? terrain = Field.GetTerrain(X, Y);

                    if (terrain != null)
                    {
                        return terrain.GetSpeedMultiplier(this);
                    }
                }

                return 1.0;
            }
        }


        protected Unit(Health unitHealth, Armor unitArmor,
            Attack unitAttack, MagicResistance unitMagicResistance)
        {
            if (unitHealth == null)
                throw new ArgumentNullException(nameof(unitHealth));

            if (unitArmor == null)
                throw new ArgumentNullException(nameof(unitArmor));

            if (unitAttack == null)
                throw new ArgumentNullException(nameof(unitAttack));

            if (unitMagicResistance == null)
                throw new ArgumentNullException(nameof(unitMagicResistance));

            X = -1;
            Y = -1;
            Field = null;

            UnitHealth = new Health(unitHealth);
            UnitArmor = new Armor(unitArmor);
            UnitAttack = new Attack(unitAttack);
            UnitMagicResistance = new MagicResistance(unitMagicResistance);
        }

        protected Unit(Unit other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            X = -1;
            Y = -1;
            Field = null;

            UnitHealth = new Health(other.UnitHealth);
            UnitArmor = new Armor(other.UnitArmor);
            UnitAttack = new Attack(other.UnitAttack);
            UnitMagicResistance = new MagicResistance(other.UnitMagicResistance);
            MovementPoints = other.MovementPoints;
        }

        internal void SetPosition(GameField? field, int x, int y)
        {
            Field = field;
            X = x;
            Y = y;
        }

        public bool HasMovementPoints(double cost)
        {
            if (cost <= 0 || !double.IsFinite(cost))
            {
                throw new ArgumentException("Недопустимая стоимость перемещения");
            }

            return MovementPoints + 0.000001 >= cost;
        }

        internal bool SpendMovementPoints(double cost)
        {
            if (!HasMovementPoints(cost))
            {
                return false;
            }

            MovementPoints = Math.Max(0, MovementPoints - cost);

            return true;
        }

        internal void ResetMovementPoints()
        {
            MovementPoints = MaxMovementPoints;
        }

        internal void SetMovementPoints(double points)
        {
            if (points < 0 || !double.IsFinite(points))
            {
                throw new ArgumentException("Некорректное количество очков перемещения");
            }

            MovementPoints = Math.Min(points, MaxMovementPoints);
        }

        public virtual void AttackUnit(Unit target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!IsAlive || !target.IsAlive)
            {
                return;
            }

            if (target == this)
            {
                return;
            }

            if (Field == null || Field != target.Field)
            {
                return;
            }

            Armor effectiveArmor = new Armor(target.EffectiveArmor);

            int damage = UnitAttack.CalculateDamage(
                effectiveArmor,
                target.UnitMagicResistance);

            target.TakeDamage(damage);
        }

        public virtual bool Move(int newX, int newY)
        {
            if (!IsAlive)
            {
                return false;
            }

            if (Field == null)
            {
                return false;
            }

            return Field.MoveUnit(this, newX, newY);
        }

        public abstract string GetUnitType();

        public abstract Unit Clone();

        public virtual void TakeDamage(int damageValue)
        {
            if (damageValue < 0)
            {
                throw new ArgumentException("Урон не может быть отрицательным");
            }

            if (!IsAlive)
            {
                return;
            }

            int previousHealth = CurrentHealth;

            UnitHealth.Decrease(damageValue);

            int actualDamage = previousHealth - CurrentHealth;

            if (actualDamage > 0)
            {
                Notify(new UnitEvent(UnitEventType.Damaged, Id, actualDamage));
            }

            if (!IsAlive)
            {
                List<IObserver> deathObservers = new List<IObserver>(observers);

                OnDestroyed();

                UnitEvent unitEvent = new UnitEvent(UnitEventType.Destroyed, Id);

                foreach (IObserver observer in deathObservers)
                {
                    NotifyObserver(observer, unitEvent);
                }

                if (Field != null)
                {
                    Field.RemoveUnit(this);
                }

                observers.Clear();
            }
        }

        public virtual void TakeHeal(int healValue)
        {
            if (!IsAlive)
            {
                return;
            }

            UnitHealth.Increase(healValue);
        }

        public virtual void IncreaseArmor(int armorValue)
        {
            UnitArmor.Increase(armorValue);
        }

        public virtual void DecreaseArmor(int armorValue)
        {
            UnitArmor.Decrease(armorValue);
        }

        public virtual void IncreaseAttack(int attackValue)
        {
            UnitAttack.Increase(attackValue);
        }

        public virtual void DecreaseAttack(int attackValue)
        {
            UnitAttack.Decrease(attackValue);
        }

        public virtual void IncreaseMagicResistance(int resistanceValue)
        {
            UnitMagicResistance.Increase(resistanceValue);
        }

        public virtual void DecreaseMagicResistance(int resistanceValue)
        {
            UnitMagicResistance.Decrease(resistanceValue);
        }

        public void Subscribe(IObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            observers.Remove(observer);
        }

        private void NotifyObserver(IObserver observer, UnitEvent unitEvent)
        {
            try
            {
                observer.Update(unitEvent);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка наблюдателя: {exception.Message}");
            }
        }

        private void Notify(UnitEvent unitEvent)
        {
            foreach (IObserver observer in new List<IObserver>(observers))
            {
                NotifyObserver(observer, unitEvent);
            }
        }

        internal void NotifyCreated()
        {
            Notify(new UnitEvent(UnitEventType.Created, Id));
        }

        internal void SetOwnerBase(Base? ownerBase)
        {
            OwnerBase = ownerBase;
        }

        protected virtual void OnDestroyed()
        {
        }
    }
}
