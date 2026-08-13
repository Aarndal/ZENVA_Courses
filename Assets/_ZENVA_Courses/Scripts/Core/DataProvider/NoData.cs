using System;

namespace Core
{
    /// <summary>
    /// Marker for generics without data requirement.
    /// </summary>
    public readonly struct NoData : IDataProvider
    {
        public static readonly NoData Instance = default;

        public Guid ID => Guid.Empty;

        public bool Equals(IDataProvider other) => other is NoData;
        public override bool Equals(object obj) => obj is NoData;
        public override int GetHashCode() => 0;
    }
}
