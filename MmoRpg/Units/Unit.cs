using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Attributes;
using MmoRpg.Field;

namespace MmoRpg.Units
{
    public abstract class Unit
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public GameField? Field { get; private set; }

        public Health UnitHealth { get; private set; }
        public Armor UnitArmor { get; private set; }
        public Attack UnitAttack { get; private set; }
        public MagicResistance UnitMagicResistance { get; private set; }

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
        }

        public void SetPosition(GameField? field, int x, int y)
        {
            Field = field;
            X = x;
            Y = y;
        }

        public virtual void AttackUnit(Unit target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (UnitHealth.IsZero() || target.UnitHealth.IsZero())
                return;

            int damage = UnitAttack.CalculateDamage(
                target.UnitArmor, target.UnitMagicResistance);

            target.TakeDamage(damage);
        }

        public virtual bool Move(int newX, int newY)
        {
            if (UnitHealth.IsZero())
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
            UnitHealth.Decrease(damageValue);
        }

        public virtual void TakeHeal(int healValue)
        {
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
    }
}
