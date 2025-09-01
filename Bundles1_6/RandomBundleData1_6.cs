// clone of SDV's RandomBundleData object
using System;
using System.Collections.Generic;
using SeedFinding.Bundles1_5;

namespace SeedFinding.Bundles1_6
{
	public class RandomBundleData
    {
        public string AreaName = "";
        public string Keys = "";
        public List<BundleSetData> BundleSets = new();
        public List<BundleData1_6> Bundles = new();
    }
}

