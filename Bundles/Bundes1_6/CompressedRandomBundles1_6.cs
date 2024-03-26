// Class used to translate 64 bit mask to bundle item ids
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace SeedFinding.Bundles1_6
{
    public class CompressedRemixBundles
    {
        public BigInteger State;
        public CompressedRemixBundles(BigInteger state) { State = state; }

        public bool Satisfies(BigInteger requirement)
        {
            return (requirement & State) == requirement;
        }

        public bool Contains(BigInteger requirement)
        {
            return (requirement & State) != 0;
        }

        public List<string> Curate()
        {
            List<string> result = new List<string>();
            BigInteger flag = new BigInteger();
            FieldInfo[] Flags = typeof(CompressedFlags).GetFields();
            for (int i = 0; i < Flags.Length; i++)
            {
                if (this.Contains((BigInteger)Flags[i].GetValue(flag)))
                {
                    result.Add(Flags[i].Name);
                }
            }
            return result;
        }

        //Removed toDict() as it is not yet updated to 1.6

    }
}

