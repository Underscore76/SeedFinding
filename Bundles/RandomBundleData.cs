// clone of SDV's RandomBundleData object
using System;
using System.Collections.Generic;

namespace SeedFinding.Bundles
{
	public class RandomBundleData
    {
        public string AreaName = "";
        public string Keys = "";
        public List<BundleSetData> BundleSets = new List<BundleSetData>();
        public List<BundleData> Bundles = new List<BundleData>();
    }
}

