using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ParallelGOL_v4
{
    internal static class HashHelpers
    {
        public static readonly int RandomSeed = new Random().Next(int.MinValue, int.MaxValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(int h1, int h2)
        {
            // RyuJIT optimizes this to use the ROL instruction
            // Related GitHub pull request: dotnet/coreclr#1830
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }
    }

    public struct Coord : IEnumerable<int>, IEquatable<Coord>
    {
        private readonly int x;
        private readonly int y;
        private readonly int value;
        public int X { get => x; }
        public int Y { get => y; }
        public int Value { get => value; }

        public int this[int index]
        {
            get { return index == 0 ? x : index == 1 ? y : value; }
        }

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
            value = 0;
        }

        public Coord(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }

        public static implicit operator (int, int)(Coord c) => (c.x, c.y);

        public static implicit operator Coord((int X, int Y) c) => new Coord(c.X, c.Y);

        public static implicit operator (int, int, int)(Coord c) => (c.x, c.y, c.value);

        public static implicit operator Coord((int X, int Y, int Z) c) => new Coord(c.X, c.Y, c.Z);

        public void Deconstruct(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }

        public void Deconstruct(out int x, out int y, out int value)
        {
            x = this.x;
            y = this.y;
            value = this.value;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.x + b.x, a.y + b.y, a.value + b.value);
        }

        public override bool Equals(object other) =>
            other is Coord c
                && c.x.Equals(x)
                && c.y.Equals(y)
                && c.value.Equals(value);

        // Implement IEquatable<T> https://stackoverflow.com/a/8952026/7532
        public bool Equals([AllowNull] Coord other) => x == other.x && y == other.y && value == other.value;

        public override int GetHashCode()
        //            => HashHelpers.Combine(HashHelpers.Combine(HashHelpers.Combine(HashHelpers.RandomSeed, x), y), z);
        {
            // based on Jon Skeet - hashcode of an int is just its value
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + x;
                hash = hash * 23 + y;
                hash = hash * 23 + value;
                return hash;
            }
        }

        // => x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();

        public override string ToString() => $"({x},{y},{value})";

        public IEnumerator<int> GetEnumerator()
        {
            yield return x;
            yield return y;
            yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}