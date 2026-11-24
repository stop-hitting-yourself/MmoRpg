using System;

namespace MmoRpg.Events
{
    public class UnitEvent
    {
        public UnitEventType Type { get; }
        public Guid UnitId { get; }
        public int Damage { get; }
        public DateTime Timestamp { get; }

        public UnitEvent(UnitEventType type, Guid unitId, int damage = 0)
        {
            Type = type;
            UnitId = unitId;
            Damage = damage;
            Timestamp = DateTime.Now;
        }
    }
}