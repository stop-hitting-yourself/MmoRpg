using System;
using MmoRpg.Units;

namespace MmoRpg.Terrains
{
    public class TerrainProxy : ITerrain
    {
        private readonly ITerrain terrain;

        public TerrainProxy(ITerrain terrain)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            this.terrain = terrain;
        }

        public string GetTerrainType()
        {
            return terrain.GetTerrainType();
        }

        public bool CanEnter(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (!unit.IsAlive)
            {
                return false;
            }

            if (!terrain.CanEnter(unit))
            {
                Console.WriteLine($"Юнит {unit.GetUnitType()} не может войти в {GetTerrainType()}");

                return false;
            }

            return true;
        }

        public double GetSpeedMultiplier(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            return terrain.GetSpeedMultiplier(unit);
        }

        public int GetDefenseBonus(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            return terrain.GetDefenseBonus(unit);
        }

        public TerrainProxy Clone()
        {
            return new TerrainProxy(terrain);
        }
    }
}