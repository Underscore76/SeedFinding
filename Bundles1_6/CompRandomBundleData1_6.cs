// Compressed version of RandomBundleData for use in generator
using System;
using System.Collections.Generic;
using System.Linq;
using SeedFinding.Bundles1_5;

namespace SeedFinding.Bundles1_6
{
    public class CompRandomBundleData
    {
        public string AreaName;
        public int[] Keys;
        public List<CompBundleSetData> BundleSets;
        public List<CompBundleData1_6> Bundles;

        public CompRandomBundleData(RandomBundleData randomBundleData)
        {
            AreaName = randomBundleData.AreaName;

            Keys = randomBundleData.Keys.Trim().Split(' ').Select(o => int.Parse(o)).ToArray();
            BundleSets = new List<CompBundleSetData>(
                randomBundleData.BundleSets.Select(o => new CompBundleSetData(o))
                );
            Bundles = new List<CompBundleData1_6>(
                randomBundleData.Bundles.Select(o => new CompBundleData1_6(o))
                );
        }
    }
}

