using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Units;
using MmoRpg.Factories;

namespace MmoRpg.Field
{
    public class GameField : IEnumerable<Unit>
    {
        private Unit?[,] units;
        private int maxUnits;
        private int unitCount;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Size => unitCount;
        public bool Empty => unitCount == 0;

        public GameField(int width, int height, int maxUnits)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Ширина и высота игрового поля должны быть положительными");
            }

            if (maxUnits < 0 || maxUnits > (long)width * height)
            {
                throw new ArgumentException("Недопустимое максимальное количество юнитов");
            }

            Width = width;
            Height = height;

            this.maxUnits = maxUnits;
            unitCount = 0;

            units = new Unit?[width, height];
        }

        public GameField(GameField other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Width = other.Width;
            Height = other.Height;

            maxUnits = other.maxUnits;
            unitCount = 0;

            units = new Unit?[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Unit? original = other.units[x, y];

                    if (original != null)
                    {
                        Unit copy = original.Clone();

                        PlaceUnit(copy, x, y);
                    }
                }
            }
        }

        public bool CreateUnit(UnitFactory factory, int x, int y)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!IsInside(x, y))
            {
                return false;
            }

            if (units[x, y] != null || unitCount >= maxUnits)
            {
                return false;
            }

            Unit unit = factory.CreateUnit();

            return PlaceUnit(unit, x, y);
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        public Unit? GetUnit(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return null;
            }

            return units[x, y];
        }

        public bool PlaceUnit(Unit unit, int x, int y)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.Field != null || unitCount >= maxUnits)
            {
                return false;
            }

            if (!IsInside(x, y) || units[x, y] != null)
            {
                return false;
            }

            units[x, y] = unit;
            unit.SetPosition(this, x, y);
            unitCount++;

            return true;
        }

        public bool CanMoveTo(Unit unit, int newX, int newY)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.Field != this)
            {
                return false;
            }

            if (!IsInside(unit.X, unit.Y))
            {
                return false;
            }

            if (units[unit.X, unit.Y] != unit)
            {
                return false;
            }

            if (!IsInside(newX, newY))
            {
                return false;
            }

            if (units[newX, newY] != null)
            {
                return false;
            }

            return true;
        }

        public bool MoveUnit(Unit unit, int newX, int newY)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.UnitHealth.IsZero())
            {
                return false;
            }

            if (!CanMoveTo(unit, newX, newY))
            {
                return false;
            }

            units[unit.X, unit.Y] = null;
            units[newX, newY] = unit;

            unit.SetPosition(this, newX, newY);

            return true;
        }

        public bool RemoveUnit(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.Field != this)
            {
                return false;
            }

            if (!IsInside(unit.X, unit.Y))
            {
                return false;
            }

            if (units[unit.X, unit.Y] != unit)
            {
                return false;
            }

            units[unit.X, unit.Y] = null;
            unit.SetPosition(null, -1, -1);
            unitCount--;

            return true;
        }

        public bool ReplaceUnit(Unit oldUnit, Unit newUnit)
        {
            if (oldUnit == null)
            {
                throw new ArgumentNullException(nameof(oldUnit));
            }

            if (newUnit == null)
            {
                throw new ArgumentNullException(nameof(newUnit));
            }

            if (oldUnit.Field != this || newUnit.Field != null)
            {
                return false;
            }

            int x = oldUnit.X;
            int y = oldUnit.Y;

            if (!IsInside(x, y) || units[x, y] != oldUnit)
            {
                return false;
            }

            units[x, y] = newUnit;

            oldUnit.SetPosition(null, -1, -1);
            newUnit.SetPosition(this, x, y);

            return true;
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (units[x, y] != null)
                    {
                        units[x, y]!.SetPosition(null, -1, -1);
                        units[x, y] = null;
                    }
                }
            }

            unitCount = 0;
        }
        public IEnumerator<Unit> GetEnumerator()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Unit? unit = units[x, y];

                    if (unit != null)
                    {
                        yield return unit;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



    }
}
