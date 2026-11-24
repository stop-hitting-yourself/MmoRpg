using MmoRpg.Units;
using MmoRpg.Bases;
using MmoRpg.Terrains;
using MmoRpg.NeutralObjects;

namespace MmoRpg.Field
{
    public class FieldCell
    {
        public Unit? Unit { get; internal set; }
        public Base? Base { get; internal set; }
        public TerrainProxy Terrain { get; internal set; }
        public NeutralObject? NeutralObject { get; internal set; }

        public FieldCell()
        {
            Unit = null;
            Base = null;
            Terrain = new TerrainProxy(new Plain());
            NeutralObject = null;
        }
    }
}