using MmoRpg.Units;

namespace MmoRpg.Terrains
{
    public class Forest : ITerrain
    {
        public string GetTerrainType()
        {
            return "Forest";
        }

        public bool CanEnter(Unit unit)
        {
            return !(unit is Cavalry);
        }

        public double GetSpeedMultiplier(Unit unit)
        {
            return 0.75;
        }

        public int GetDefenseBonus(Unit unit)
        {
            return 10;
        }
    }
}