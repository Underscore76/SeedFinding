using System;
using SeedFinding.Utilities;
using System.Drawing;

namespace SeedFinding.NightEvents
{
    public class NightEvent
    {
        public enum Event
        {
            None,
            Fairy,
            Witch,
            Meteor,
            StoneOwl,
            StrangeCapsule,
            WindStorm
        }

        public static Event GetEvent(uint gameID, int daysPlayed, bool pantryComplete = false, bool raccoonsValid = false, bool hasFairyRose = false, bool capsuleValid = false)
        {
            return Game1.Version.Minor switch
            {
                5 => GetEvent1_5((int)gameID, daysPlayed),
                6 => GetEvent1_6(gameID, daysPlayed, pantryComplete, raccoonsValid, hasFairyRose, capsuleValid),
                _ => Event.None,//maybe add more version handling in the future, would need to check code
            };
        }
        public static Event GetEvent1_5(int gameID, int daysPlayed)
        {
            if (daysPlayed == 31)
            {
               return Event.None; 
            } 
            Random random = new(daysPlayed + gameID / 2);
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
        public static Event GetEvent1_6(uint gameID, int daysPlayed, bool pantryComplete, bool raccoonsValid, bool hasFairyRose, bool capsuleValid)
        {
            if (daysPlayed == 31)
            {
                return Event.None;
            }
            Random random = Utility.CreateDaySaveRandom(daysPlayed,gameID);
            for (int i = 0; i < 10; i++)
            {
                random.NextDouble();
            }

            if (pantryComplete && random.NextDouble() < 0.1 && raccoonsValid)
            {
                return Event.WindStorm;
            }

            if (random.NextDouble() < 0.01 + (hasFairyRose ? 0.007 : 0.0 ) && daysPlayed % 28 != 1)// && !Game1.currentSeason.Equals("winter"))
            {
                return Event.Fairy;
            }
            if (random.NextDouble() < 0.01 && daysPlayed > 20)
            {
                return Event.Witch;
            }
            if (random.NextDouble() < 0.01 && daysPlayed > 5)
            {
                return Event.Meteor;
            }
            if (random.NextDouble() < 0.005)
            {
                return Event.StoneOwl;
            }
            if (random.NextDouble() < 0.008 && daysPlayed > 28 * 4 && capsuleValid)// && Game1.year > 1 && !Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"))
            {
                return Event.StrangeCapsule;
            }

            return Event.None;
        }       
    }
}