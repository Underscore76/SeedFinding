// clone of SDV's RandomBundleData object
using System;
using System.Collections.Generic;

namespace SeedFinding.Bundles1_5
{
	public class RandomBundleData
    {
        public string AreaName = "";
        public string Keys = "";
        public List<BundleSetData> BundleSets = new();
        public List<BundleData> Bundles = new();
    }
}

