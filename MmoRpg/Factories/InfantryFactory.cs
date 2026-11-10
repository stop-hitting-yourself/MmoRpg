using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Units;

namespace MmoRpg.Factories
{
    public class InfantryFactory : UnitFactory
    {
        private string unitType;

        public InfantryFactory(string unitType)
        {
            if (unitType != "Swordsman" && unitType != "Spearman")
            {
                throw new ArgumentException("Неизвестный тип пехотинца");
            }

            this.unitType = unitType;
        }

        public override Unit CreateUnit()
        {
            if (unitType == "Swordsman")
            {
                return new Swordsman();
            }
            else
            {
                return new Spearman();
            }
        }
    }
}
