using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Units;

namespace MmoRpg.Factories
{
    public class CavalryFactory : UnitFactory
    {
        private string unitType;

        public CavalryFactory(string unitType)
        {
            if (unitType != "Cataphract" && unitType != "HorseArcher")
            {
                throw new ArgumentException("Неизвестный тип кавалериста");
            }

            this.unitType = unitType;
        }

        public override Unit CreateUnit()
        {
            if (unitType == "Cataphract")
            {
                return new Cataphract();
            }
            else
            {
                return new HorseArcher();
            }
        }
    }
}

