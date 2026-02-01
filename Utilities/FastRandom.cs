using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace SeedFinding
{
    public class FastRandom : Random
    {
        public int myseed = 0;
        public int counter = 0;

        Random fallback = null;

        public static (uint,uint)[] offsets = buildOffsets(1000);
        public static int maxDepth = offsets.Length;

        //Use this to create a random; avoids creating a base Random object 
        public static FastRandom createFR(int seed)
        {
            FastRandom r = RuntimeHelpers.GetUninitializedObject(typeof(FastRandom)) as FastRandom;
            r.myseed = seed;
            return r;
        }

        //Specifically lacks a constructor that isn't createFR because *we don't want to initialize the base class*. Of course, an empty one will be generated but...
        public static (uint,uint)[] buildOffsets(int amount)
        {
            var offsets = new (uint,uint)[amount];
            Random seed0 = new Random(0);
            Random seed1 = new Random(1);
            for (int i = 0; i < amount; i++)
            {
                uint s0 = (uint)seed0.Next();
                uint s1 = (uint)seed1.Next();
                uint answer = (uint)(s1 - s0);
                if (answer > 0x7FFFFFFF)
                {
                    answer -= 0x80000001;
                }
                offsets[i] = (answer,s0);
            }
            return offsets;
        }

        // Fallback if the random function goes through more random calls than exist in the cache
        private void initializeRandom()
        {
            // Console.Out.WriteLine("fellback :("); //Useful for testing, if this occurs you should likely avoid using fastrandom in this usecase.
            fallback = new Random(myseed);
            for (int i = 0; i < counter; i++)
            {
                fallback.Next();
            }
        }
        public static (uint, uint) getOffsets(int callNumber) //0 is first call, returns (a,b) in aX+b
        {
            Random seed0 = new Random(0);
            Random seed1 = new Random(1);
            for (int i = 0; i < callNumber; i++)
            {
                seed0.Next();
                seed1.Next();
            }
            uint s0 = (uint)seed0.Next();
            uint s1 = (uint)seed1.Next();
            uint answer = (uint)(s1 - s0);
            if (answer > 0x7FFFFFFF)
            {
                answer -= 0x80000001;
            }
            return ((uint)answer, s0);
        }

        public static int fastForward(int seed, uint a, uint b)
        {
            long prod = ((seed == int.MinValue) ? int.MaxValue : Math.Abs(seed)) * (long) a;
            prod += (long)b;
            prod %= 0x7FFFFFFF; 
            return (int) prod;
        }
        public static int fastForward(int seed, int depth)
        {
            (uint,uint) offsetPair = offsets[depth];
            return fastForward(seed,offsetPair.Item1,offsetPair.Item2);
        }

        private int InternalSample()
        {
            if (fallback!=null || counter >= maxDepth)
            {
                if (fallback == null)
                {
                    initializeRandom();
                }
                counter++;
                return fallback.Next();
            }
            (uint,uint) offsetPair = offsets[counter];
            counter++;
            return fastForward(myseed,offsetPair.Item1,offsetPair.Item2);
        }


        public static void testFF(int depth,int starting)
        {
            uint a;
            uint b;
            int amountWrong = 0;
            int maxoff = 0;
            long amountOff = 0;
            int min = starting;
            int count = 10000;
            (a,b)  = getOffsets(depth);
            for (int seed = min; seed < min + count; seed++)
            {
                Random random = new Random(seed);
                for (int i = 0; i < depth - 1; i++)
                {
                    random.Next();
                }
                int x = random.Next();
                int ff = fastForward(seed, a, b);
                if (x != ff)
                {
                    amountWrong++;
                    uint distance = (uint) Math.Abs(ff - x);
                    if (distance > 0x80000000)
                    {
                        distance = 0xFFFFFFFF - distance;
                    }
                    if (distance > 0x40000000)
                    {
                        distance = 0x80000000 - distance;
                    }
                    amountOff += distance;
                    maxoff = Math.Max(maxoff, (int) distance);
                    // Console.WriteLine(depth + "," + seed + "," + ff + "," + x + "," + a + "," + b);
                    // Console.WriteLine();
                    // Console.WriteLine();
                }
                else
                {
                   // Console.WriteLine();
                }
            }
            if (maxoff != 0)
            {
                Console.WriteLine("Tested "+depth+" starting from "+min+" and was wrong "+amountWrong+"/"+count + " with an average incorrectness of " + amountOff/(amountWrong)+ " and a max wrongness of "+maxoff);
            }
        }



        //Below is all of the Random functions basically just copy-pasted to work with the new InteranlSample made above.

        protected override double Sample()
        {
            return InternalSample() * 4.656612875245797E-10;
        }

        public override int Next()
        {
            return InternalSample();
        }

        public override int Next(int maxValue)
        {
            return (int) (Sample() * (double) maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            return (int)(Sample() * (double)(maxValue-minValue)) + minValue;
        }

         public override long NextInt64()
        {
            ulong num;
            do
            {
                num = NextUInt64() >> 1;
            }
            while (num == long.MaxValue);
            return (long)num;
        }

        public override long NextInt64(long maxValue)
        {
            return NextInt64(0L, maxValue);
        }

        public override long NextInt64(long minValue, long maxValue)
        {
            ulong num = (ulong)(maxValue - minValue);
            if (num > 1)
            {
                int num2 = BitOperations.Log2(num);
                ulong num3;
                do
                {
                    num3 = NextUInt64() >> 64 - num2;
                }
                while (num3 >= num);
                return (long)num3 + minValue;
            }
            return minValue;
        }

        private ulong NextUInt64()
        {
            return (uint)Next(4194304) | ((ulong)(uint)Next(4194304) << 22) | ((ulong)(uint)Next(1048576) << 44);
        }
        public override double NextDouble()
        {
            return Sample();
        }

        public override float NextSingle()
        {
            return (float)Sample();
        }

        public override void NextBytes(byte[] buffer)
        {
            NextBytes(buffer);
        }

        public override void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Next();
            }
        }
    }
}