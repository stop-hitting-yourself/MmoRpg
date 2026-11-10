using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmoRpg.Units;

namespace MmoRpg.Factories
{
    public abstract class UnitFactory
    {
        public abstract Unit CreateUnit();
    }
}