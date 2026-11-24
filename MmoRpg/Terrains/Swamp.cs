using MmoRpg.Units;

namespace MmoRpg.Terrains
{
    public class Swamp : ITerrain
    {
        public string GetTerrainType()
        {
            return "Swamp";
        }

        public bool CanEnter(Unit unit)
        {
            return true;
        }

        public double GetSpeedMultiplier(Unit unit)
        {
            return 0.5;
        }

        public int GetDefenseBonus(Unit unit)
        {
            return 0;
        }
    }
}