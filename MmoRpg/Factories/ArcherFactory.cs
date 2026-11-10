using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Units;

namespace MmoRpg.Factories
{
    public class ArcherFactory : UnitFactory
    {
        private string unitType;

        public ArcherFactory(string unitType)
        {
            if (unitType != "LightArcher" && unitType != "HeavyArcher")
            {
                throw new ArgumentException("Неизвестный тип лучника");
            }

            this.unitType = unitType;
        }

        public override Unit CreateUnit()
        {
            if (unitType == "LightArcher")
            {
                return new LightArcher();
            }
            else
            {
                return new HeavyArcher();
            }
        }
    }
}
