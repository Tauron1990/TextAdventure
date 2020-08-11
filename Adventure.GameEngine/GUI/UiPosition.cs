using System;

namespace Adventure.GameEngine.GUI
{
    public readonly struct UiPosition : IEquatable<UiPosition>
    {
        public static UiPosition Invalid { get; } = new UiPosition(-1, -1);

        public int Top { get; }

        public int Left { get; }

        public UiPosition(int top, int left)
        {
            Top = top;
            Left = left; }

        public bool Equals(UiPosition other) => Top == other.Top && Left == other.Left;

        public override bool Equals(object obj) => obj is UiPosition other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Top, Left);

        public static bool operator ==(UiPosition left, UiPosition right) => left.Equals(right);

        public static bool operator !=(UiPosition left, UiPosition right) => !left.Equals(right);
    }
}