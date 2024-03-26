// clone of SDV's RandomBundleData object
using System;
using System.Collections.Generic;
using SeedFinding.Bundles;

namespace SeedFinding.Bundles1_6
{
	public class RandomBundleData
    {
        public string AreaName = "";
        public string Keys = "";
        public List<BundleSetData> BundleSets = new List<BundleSetData>();
        public List<BundleData> Bundles = new List<BundleData>();
    }
}

