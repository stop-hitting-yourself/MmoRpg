using System;
using System.Collections.Generic;
using MmoRpg.Units;
using MmoRpg.Strategies;

namespace MmoRpg.NeutralObjects
{
    public class PowerUpAltar : NeutralObject
    {
        private readonly HashSet<Guid> usedBy;

        public PowerUpAltar()
            : base(new ArmorBuffStrategy(10), true)
        {
            usedBy = new HashSet<Guid>();
        }

        public PowerUpAltar(PowerUpAltar other)
            : base(other)
        {
            usedBy = new HashSet<Guid>(other.usedBy);
        }

        public bool WasUsedBy(Unit unit)
        {
            return usedBy.Contains(unit.Id);
        }

        internal override bool Interact(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (WasUsedBy(unit))
            {
                return false;
            }

            if (!base.Interact(unit))
            {
                return false;
            }

            usedBy.Add(unit.Id);

            return true;
        }

        internal void RemapUsedUnits(
            Dictionary<Guid, Guid> unitIds)
        {
            HashSet<Guid> newUsedBy = new HashSet<Guid>();

            foreach (Guid oldId in usedBy)
            {
                if (unitIds.TryGetValue(oldId, out Guid newId))
                {
                    newUsedBy.Add(newId);
                }
            }

            usedBy.Clear();
            usedBy.UnionWith(newUsedBy);
        }

        public override string GetObjectType()
        {
            return "PowerUpAltar";
        }

        public override NeutralObject Clone()
        {
            return new PowerUpAltar(this);
        }
    }
}