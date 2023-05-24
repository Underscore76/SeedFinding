// Compressed version of BundleSetData
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedFinding.Bundles
{
    public class CompBundleSetData
    {
        public List<CompBundleData> Bundles;
        public CompBundleSetData(BundleSetData bundleSetData)
        {
            Bundles = new List<CompBundleData>(
                bundleSetData.Bundles.Select(o => new CompBundleData(o))
                );
        }
    }
}

