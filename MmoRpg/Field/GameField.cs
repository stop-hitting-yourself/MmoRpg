using MmoRpg.Bases;
using MmoRpg.Factories;
using MmoRpg.NeutralObjects;
using MmoRpg.Strategies;
using MmoRpg.Terrains;
using MmoRpg.Units;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MmoRpg.Field
{
    public class GameField : IEnumerable<Unit>
    {
        private FieldCell[,] cells;
        private int maxUnits;
        private int unitCount;

        public TerrainMap TerrainMap { get; }

        public NeutralObject? GetNeutralObject(int x, int y)
        {
            return TerrainMap.GetNeutralObjectAt(x, y);
        }

        public int TurnNumber { get; private set; } = 1;

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

            cells = new FieldCell[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells[x, y] = new FieldCell();
                }
            }

            TerrainMap = new TerrainMap(cells);
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

            TurnNumber = other.TurnNumber;

            cells = new FieldCell[Width, Height];

            Dictionary<Base, Base> baseCopies =
                new Dictionary<Base, Base>();

            Dictionary<Guid, Guid> unitIds =
                new Dictionary<Guid, Guid>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells[x, y] = new FieldCell();

                    cells[x, y].Terrain =
                        other.cells[x, y].Terrain.Clone();
                }
            }

            TerrainMap = new TerrainMap(cells);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Base? originalBase = other.cells[x, y].Base;

                    if (originalBase != null)
                    {
                        Base copy = new Base(originalBase);

                        if (!PlaceBase(copy, x, y))
                        {
                            throw new InvalidOperationException(
                                "Не удалось скопировать базу");
                        }

                        baseCopies.Add(originalBase, copy);
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Unit? original = other.cells[x, y].Unit;

                    if (original != null)
                    {
                        Unit copy = original.Clone();

                        if (!PlaceUnit(copy, x, y))
                        {
                            throw new InvalidOperationException("Не удалось скопировать юнита");
                        }

                        unitIds.Add(original.Id, copy.Id);

                        if (original.OwnerBase != null)
                        {
                            Base ownerCopy = baseCopies[original.OwnerBase];

                            ownerCopy.RegisterUnit(copy);
                        }
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    NeutralObject? original =
                        other.cells[x, y].NeutralObject;

                    if (original != null)
                    {
                        NeutralObject copy = original.Clone();

                        if (copy is PowerUpAltar altar)
                        {
                            altar.RemapUsedUnits(unitIds);
                        }

                        if (!PlaceNeutralObject(copy, x, y))
                        {
                            throw new InvalidOperationException("Не удалось скопировать нейтральный объект");
                        }
                    }
                }
            }
        }

        public bool PlaceNeutralObject(
            NeutralObject neutralObject, int x, int y)
        {
            if (neutralObject == null)
            {
                throw new ArgumentNullException(nameof(neutralObject));
            }

            if (!IsInside(x, y))
            {
                return false;
            }

            if (neutralObject.Field != null || neutralObject.IsUsed)
            {
                return false;
            }

            if (cells[x, y].NeutralObject != null ||
                cells[x, y].Base != null)
            {
                return false;
            }

            cells[x, y].NeutralObject = neutralObject;

            neutralObject.SetPosition(this, x, y);

            return true;
        }
        public bool InteractWithNeutralObject(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (!unit.IsAlive || unit.Field != this)
            {
                return false;
            }

            int x = unit.X;
            int y = unit.Y;

            if (!IsInside(x, y))
            {
                return false;
            }

            if (cells[x, y].Unit != unit)
            {
                return false;
            }

            NeutralObject? neutralObject = cells[x, y].NeutralObject;

            if (neutralObject == null)
            {
                return false;
            }

            if (neutralObject is PowerUpAltar altar)
            {
                if (!ConfigurePowerUpAltar(altar, unit))
                {
                    return false;
                }
            }

            if (!neutralObject.Interact(unit))
            {
                return false;
            }

            if (neutralObject.IsUsed &&
                cells[x, y].NeutralObject == neutralObject)
            {
                RemoveNeutralObject(x, y);
            }

            return true;
        }

        public bool RemoveNeutralObject(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return false;
            }

            NeutralObject? neutralObject = cells[x, y].NeutralObject;

            if (neutralObject == null)
            {
                return false;
            }

            cells[x, y].NeutralObject = null;

            neutralObject.SetPosition(null, -1, -1);

            return true;
        }

        private bool ConfigurePowerUpAltar(PowerUpAltar altar, Unit unit)
        {
            if (unit is Infantry)
            {
                altar.SetStrategy(new ArmorBuffStrategy(10));
            }
            else if (unit is Archer)
            {
                altar.SetStrategy(new AttackBuffStrategy(15));
            }
            else if (unit is Cavalry)
            {
                altar.SetStrategy(new ResistanceBuffStrategy(15));
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool AreAdjacent(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2) == 1;
        }

        private double GetMovementCost(Unit unit, int x, int y)
        {
            ITerrain terrain = cells[x, y].Terrain;

            double multiplier = terrain.GetSpeedMultiplier(unit);

            if (multiplier <= 0 || !double.IsFinite(multiplier))
            {
                throw new InvalidOperationException("Недопустимый множитель скорости");
            }

            return 1.0 / multiplier;
        }

        public void NextTurn()
        {
            TurnNumber = checked(TurnNumber + 1);

            foreach (Unit unit in this)
            {
                unit.ResetMovementPoints();
            }
        }

        public bool SetTerrain(int x, int y, ITerrain terrain)
        {
            return TerrainMap.SetTerrainAt(x, y, terrain);
        }

        public ITerrain? GetTerrain(int x, int y)
        {
            return TerrainMap.GetTerrainAt(x, y);
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        public bool PlaceUnit(Unit unit, int x, int y)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.Field != null ||
                unit.OwnerBase != null ||
                !unit.IsAlive ||
                unitCount >= maxUnits)
            {
                return false;
            }

            if (!IsInside(x, y) ||
                cells[x, y].Unit != null ||
                cells[x, y].Base != null)
            {
                return false;
            }

            ITerrain terrain = cells[x, y].Terrain;

            if (!terrain.CanEnter(unit))
            {
                return false;
            }

            cells[x, y].Unit = unit;

            unit.SetPosition(this, x, y);

            unitCount++;

            return true;
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

            if (cells[x, y].Unit != null ||
                cells[x, y].Base != null ||
                unitCount >= maxUnits)
            {
                return false;
            }

            Unit unit = factory.CreateUnit();

            if (!PlaceUnit(unit, x, y))
            {
                return false;
            }

            unit.NotifyCreated();

            return true;
        }

        public Unit? GetUnit(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return null;
            }

            return cells[x, y].Unit;
        }


        public bool CanMoveTo(Unit unit, int newX, int newY)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (unit.Field != this || !unit.IsAlive)
            {
                return false;
            }

            if (!IsInside(unit.X, unit.Y))
            {
                return false;
            }

            if (cells[unit.X, unit.Y].Unit != unit)
            {
                return false;
            }

            if (!IsInside(newX, newY))
            {
                return false;
            }

            if (!AreAdjacent(unit.X, unit.Y, newX, newY))
            {
                return false;
            }

            if (cells[newX, newY].Unit != null ||
                cells[newX, newY].Base != null)
            {
                return false;
            }

            ITerrain terrain = cells[newX, newY].Terrain;

            if (!terrain.CanEnter(unit))
            {
                return false;
            }

            double cost = GetMovementCost(unit, newX, newY);

            return unit.HasMovementPoints(cost);
        }

        public bool MoveUnit(Unit unit, int newX, int newY)
        {
            if (!CanMoveTo(unit, newX, newY))
            {
                return false;
            }

            double cost = GetMovementCost(unit, newX, newY);

            if (!unit.SpendMovementPoints(cost))
            {
                return false;
            }

            cells[unit.X, unit.Y].Unit = null;
            cells[newX, newY].Unit = unit;

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

            if (cells[unit.X, unit.Y].Unit != unit)
            {
                return false;
            }

            if (unit.OwnerBase != null)
            {
                unit.OwnerBase.UnregisterUnit(unit);
            }

            cells[unit.X, unit.Y].Unit = null;

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

            if (oldUnit.Field != this ||
                newUnit.Field != null ||
                newUnit.OwnerBase != null ||
                !newUnit.IsAlive)
            {
                return false;
            }

            int x = oldUnit.X;
            int y = oldUnit.Y;

            if (!IsInside(x, y))
            {
                return false;
            }

            if (cells[x, y].Unit != oldUnit)
            {
                return false;
            }

            ITerrain terrain = cells[x, y].Terrain;

            if (!terrain.CanEnter(newUnit))
            {
                return false;
            }

            Base? owner = oldUnit.OwnerBase;

            if (owner != null)
            {
                if (!owner.ReplaceOwnedUnit(oldUnit, newUnit))
                {
                    return false;
                }
            }

            newUnit.SetMovementPoints(oldUnit.MovementPoints);

            cells[x, y].Unit = newUnit;

            oldUnit.SetPosition(null, -1, -1);

            newUnit.SetPosition(this, x, y);

            newUnit.NotifyCreated();

            return true;
        }

        public bool PlaceBase(Base gameBase, int x, int y)
        {
            if (gameBase == null)
            {
                throw new ArgumentNullException(nameof(gameBase));
            }

            if (gameBase.Field != null || !gameBase.IsAlive)
            {
                return false;
            }

            if (!IsInside(x, y))
            {
                return false;
            }

            if (cells[x, y].Unit != null ||  cells[x, y].Base != null || cells[x, y].NeutralObject != null)
            {
                return false;
            }

            cells[x, y].Base = gameBase;

            gameBase.SetPosition(this, x, y);

            return true;
        }

        public Base? GetBase(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return null;
            }

            return cells[x, y].Base;
        }

        public bool RemoveBase(Base gameBase)
        {
            if (gameBase == null)
            {
                throw new ArgumentNullException(nameof(gameBase));
            }

            if (gameBase.Field != this)
            {
                return false;
            }

            int x = gameBase.X;
            int y = gameBase.Y;

            if (!IsInside(x, y) || cells[x, y].Base != gameBase)
            {
                return false;
            }

            gameBase.ClearUnits();

            cells[x, y].Base = null;

            gameBase.SetPosition(null, -1, -1);

            return true;
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Unit? unit = cells[x, y].Unit;

                    if (unit != null)
                    {
                        RemoveUnit(unit);
                    }

                    Base? gameBase = cells[x, y].Base;

                    if (gameBase != null)
                    {
                        RemoveBase(gameBase);
                    }

                    if (cells[x, y].NeutralObject != null)
                    {
                        RemoveNeutralObject(x, y);
                    }
                }
            }

            unitCount = 0;
            TurnNumber = 1;
        }

        public IEnumerator<Unit> GetEnumerator()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Unit? unit = cells[x, y].Unit;

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