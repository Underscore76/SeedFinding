using System;

namespace StardewValley
{

    /// <summary>Wraps newer .NET features that improve performance, but aren't available on .NET Framework platforms.</summary>
    internal static class LegacyShims
    {
        /// <summary>Get an empty array without allocating a new array each time.</summary>
        /// <typeparam name="T">The array value type.</typeparam>
        public static T[] EmptyArray<T>()
        {
            return Array.Empty<T>();
        }
    }
}