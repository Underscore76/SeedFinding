// Compressed version of BundleSetData
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedFinding.Bundles1_5
{
    public class CompBundleSetData
    {
        public List<CompBundleData1_5> Bundles;
        public CompBundleSetData(BundleSetData bundleSetData)
        {
            Bundles = new List<CompBundleData1_5>(
                bundleSetData.Bundles.Select(o => new CompBundleData1_5(o))
                );
        }
    }
}

