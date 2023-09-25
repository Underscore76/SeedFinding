using System;

namespace SeedFinding
{
    public class NightEvents
    {
        public enum Event
        {
            None,
            Fairy,
            Witch,
            Meteor,
            StoneOwl,
            StrangeCapsule
        }

        public static Event GetEvent(int gameID, int day)
        {
            return GetEvent(day + gameID / 2);
        }

        public static Event GetEvent(int seed)
        {
            Random random = new Random(seed);
            if (random.NextDouble() < 0.01)// && !Game1.currentSeason.Equals("winter"))
            {
                return Event.Fairy;
            }
            if (random.NextDouble() < 0.01)
            {
                return Event.Witch;
            }
            if (random.NextDouble() < 0.01)
            {
                return Event.Meteor;
            }
            if (random.NextDouble() < 0.005)
            {
                return Event.StoneOwl;
            }
            if (random.NextDouble() < 0.008)// && Game1.year > 1 && !Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"))
            {
                return Event.StrangeCapsule;
            }

            return Event.None;
        }
    }
}