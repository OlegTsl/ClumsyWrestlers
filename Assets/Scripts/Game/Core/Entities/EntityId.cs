using System;

namespace Game.Core.Entities
{
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public static readonly EntityId Invalid = new(0u);
        public const uint DynamicRangeStart = 0x80000000u;

        public uint Value { get; }
        public bool IsValid => Value != 0u;

        public EntityId(uint value)
            => Value = value;

        public bool Equals(EntityId other)
            => Value == other.Value;

        public override bool Equals(object obj)
            => obj is EntityId other && Equals(other);

        public override int GetHashCode()
            => (int)Value;

        public override string ToString()
            => Value.ToString();

        public static bool operator ==(EntityId left, EntityId right)
            => left.Equals(right);

        public static bool operator !=(EntityId left, EntityId right)
            => !left.Equals(right);
    }
}
