using MmoRpg.Units;

namespace MmoRpg.Terrains
{
    public interface ITerrain
    {
        string GetTerrainType();

        bool CanEnter(Unit unit);

        double GetSpeedMultiplier(Unit unit);

        int GetDefenseBonus(Unit unit);
    }
}