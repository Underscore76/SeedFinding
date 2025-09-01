// Compressed version of BundleSetData
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedFinding.Bundles1_6
{
    public class CompBundleSetData
    {
        public List<CompBundleData1_6> Bundles;
        public CompBundleSetData(BundleSetData bundleSetData)
        {
            Bundles = new List<CompBundleData1_6>(
                bundleSetData.Bundles.Select(o => new CompBundleData1_6(o))
                );
        }
    }
}

