using MmoRpg.Units;

namespace MmoRpg.Terrains
{
    public class Plain : ITerrain
    {
        public string GetTerrainType()
        {
            return "Plain";
        }

        public bool CanEnter(Unit unit)
        {
            return true;
        }

        public double GetSpeedMultiplier(Unit unit)
        {
            return 1.0;
        }

        public int GetDefenseBonus(Unit unit)
        {
            return 0;
        }
    }
}