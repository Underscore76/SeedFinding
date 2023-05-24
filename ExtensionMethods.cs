using System;
using System.Collections;
using System.Collections.Generic;

namespace SeedFinding
{
    public static class BitArrayExtensionMethods
    {
        public static BitArray CopySlice(this BitArray source, int offset, int length)
        {
            BitArray ret = new BitArray(length);
            for (int i = 0; i < length; i++)
            {
                ret[i] = source[offset + i];
            }
            return ret;
        }
    }

    public static class RandomExtensionMethods
    {
        public static T GetRandom<T>(this List<T> list, Random random)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            return list[random.Next(list.Count)];
        }

        public static T GetRandom<T>(this T[] list, Random random)
        {
            if (list == null || list.Length == 0)
            {
                return default(T);
            }
            return list[random.Next(list.Length)];
        }
    }
}

