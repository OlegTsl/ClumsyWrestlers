using System;

namespace Game.Core.Teams
{
    public readonly struct TeamId : IEquatable<TeamId>
    {
        public static readonly TeamId Invalid = new(0u);

        public uint Value { get; }
        public bool IsValid => Value != 0u;

        public TeamId(uint value)
            => Value = value;

        public bool Equals(TeamId other)
            => Value == other.Value;

        public override bool Equals(object obj)
            => obj is TeamId other && Equals(other);

        public override int GetHashCode()
            => (int)Value;

        public override string ToString()
            => Value.ToString();

        public static bool operator ==(TeamId left, TeamId right)
            => left.Equals(right);

        public static bool operator !=(TeamId left, TeamId right)
            => !left.Equals(right);
    }
}
