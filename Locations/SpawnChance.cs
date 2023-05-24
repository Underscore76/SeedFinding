using System;

namespace SeedFinding.Locations
{
    public struct SpawnChance
    {
        public int Id;
        public double P;
        public SpawnChance(int id, double p) { Id = id; P = p; }
    }
}

