using System;
using MmoRpg.Field;
using MmoRpg.NeutralObjects;

namespace MmoRpg.Terrains
{
    public class TerrainMap
    {
        private readonly FieldCell[,] cells;

        public int Width => cells.GetLength(0);
        public int Height => cells.GetLength(1);

        internal TerrainMap(FieldCell[,] cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            this.cells = cells;
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        public ITerrain? GetTerrainAt(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return null;
            }

            return cells[x, y].Terrain;
        }

        public NeutralObject? GetNeutralObjectAt(int x, int y)
        {
            if (!IsInside(x, y))
            {
                return null;
            }

            return cells[x, y].NeutralObject;
        }

        internal bool SetTerrainAt(int x, int y, ITerrain terrain)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            if (!IsInside(x, y))
            {
                return false;
            }

            if (cells[x, y].Unit != null ||
                cells[x, y].Base != null)
            {
                return false;
            }

            if (terrain is TerrainProxy proxy)
            {
                cells[x, y].Terrain = proxy.Clone();
            }
            else
            {
                cells[x, y].Terrain = new TerrainProxy(terrain);
            }

            return true;
        }
    }
}