using System;
using MmoRpg.Units;
using MmoRpg.Strategies;
using MmoRpg.Field;

namespace MmoRpg.NeutralObjects
{
    public abstract class NeutralObject
    {

        public int X { get; private set; } = -1;
        public int Y { get; private set; } = -1;

        public GameField? Field { get; private set; }

        private IInteractionStrategy strategy;

        public Guid Id { get; } = Guid.NewGuid();

        public bool IsReusable { get; }
        public bool IsUsed { get; private set; }



        protected NeutralObject(IInteractionStrategy strategy, bool isReusable)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            this.strategy = strategy;

            IsReusable = isReusable;
            IsUsed = false;
        }

        protected NeutralObject(NeutralObject other)
        {
            strategy = other.strategy;

            IsReusable = other.IsReusable;
            IsUsed = other.IsUsed;
        }

        internal void SetPosition(GameField? field, int x, int y)
        {
            Field = field;
            X = x;
            Y = y;
        }

        internal void SetStrategy(IInteractionStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            this.strategy = strategy;
        }

        internal virtual bool Interact(Unit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (!unit.IsAlive || IsUsed)
            {
                return false;
            }

            if (Field == null || unit.Field != Field)
            {
                return false;
            }

            if (unit.X != X || unit.Y != Y)
            {
                return false;
            }

            if (!strategy.Apply(unit))
            {
                return false;
            }

            if (!IsReusable)
            {
                IsUsed = true;
            }

            return true;
        }

        public abstract string GetObjectType();

        public abstract NeutralObject Clone();
    }
}