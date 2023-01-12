using System.Collections.Generic;
using System;
using System.Linq;

namespace AdaptiveFEM.Models.MathElements
{
    public sealed class MathSet<T> : HashSet<T>, IEquatable<HashSet<T>>
    {
        public override int GetHashCode() => this.Select(elt => elt.GetHashCode()).Sum().GetHashCode();

        public bool Equals(HashSet<T>? other) => SetEquals(other);

        public static bool operator ==(MathSet<T> a, MathSet<T> b) =>
            ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);

        public static bool operator !=(MathSet<T> a, MathSet<T> b) => !(a == b);
    }
}
