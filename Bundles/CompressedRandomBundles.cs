// Class used to translate 64 bit mask to bundle item ids
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SeedFinding.Bundles
{
    public class CompressedRemixBundles
    {
        public ulong State;
        public CompressedRemixBundles(ulong state) { State = state; }

        public bool Satisfies(ulong requirement)
        {
            return (requirement & State) == requirement;
        }

        public bool Contains(ulong requirement)
        {
            return (requirement & State) != 0;
        }

        public Dictionary<string, List<int>> ToDict()
        {
            Dictionary<string, List<int>> bundles = new Dictionary<string, List<int>>();
            // crafts
            {
                var items = new List<int>();
                if ((State & CompressedFlags.CRAFTS_SPRING_FORAGE_WILD_HORSERADISH) != 0) items.Add(16);
                if ((State & CompressedFlags.CRAFTS_SPRING_FORAGE_DAFFODIL) != 0) items.Add(18);
                if ((State & CompressedFlags.CRAFTS_SPRING_FORAGE_LEEK) != 0) items.Add(20);
                if ((State & CompressedFlags.CRAFTS_SPRING_FORAGE_DANDELION) != 0) items.Add(22);
                if ((State & CompressedFlags.CRAFTS_SPRING_FORAGE_SPRING_ONION) != 0) items.Add(399);
                bundles.Add("Spring Foraging", items);
            }
            bundles.Add("Summer Foraging", new List<int>() { 398, 396, 402 });
            bundles.Add("Fall Foraging", new List<int>() { 404, 406, 408, 410 });
            {
                var items = new List<int>();
                if ((State & CompressedFlags.CRAFTS_WINTER_FORAGE_WINTER_ROOT) != 0) items.Add(412);
                if ((State & CompressedFlags.CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT) != 0) items.Add(414);
                if ((State & CompressedFlags.CRAFTS_WINTER_FORAGE_SNOW_YAM) != 0) items.Add(416);
                if ((State & CompressedFlags.CRAFTS_WINTER_FORAGE_CROCUS) != 0) items.Add(418);
                if ((State & CompressedFlags.CRAFTS_WINTER_FORAGE_HOLLY) != 0) items.Add(283);
                bundles.Add("Winter Foraging", items);
            }
            if ((State & CompressedFlags.CRAFTS_STICKY) != 0) bundles.Add("Sticky", new List<int>() { 92 });
            if ((State & CompressedFlags.CRAFTS_CONSTRUCTION) != 0) bundles.Add("Construction", new List<int>() { 388, 388, 2, 709 });
            if ((State & CompressedFlags.CRAFTS_WILD_MEDICINE) != 0) bundles.Add("Wild Medicine", new List<int>() { 422, 259, 157, 304 });
            if ((State & CompressedFlags.CRAFTS_EXOTIC) != 0) bundles.Add("Exotic Foraging", new List<int>() { 88, 90, 78, 420, 422, 724, 725, 726, 257 });
            // pantry
            bundles.Add("Spring Crops", new List<int>() { 24, 188, 190, 192 });
            bundles.Add("Summer Crops", new List<int>() { 256, 260, 258, 254});
            bundles.Add("Fall Crops", new List<int>() { 270, 272, 276, 280});
            if ((State & CompressedFlags.PANTRY_QUALITY) != 0)
            {
                var items = new List<int>();

                if ((State & CompressedFlags.PANTRY_QUALITY_PARSNIP) != 0) items.Add(24);
                if ((State & CompressedFlags.PANTRY_QUALITY_GREEN_BEAN) != 0) items.Add(188);
                if ((State & CompressedFlags.PANTRY_QUALITY_POTATO) != 0) items.Add(192);
                if ((State & CompressedFlags.PANTRY_QUALITY_CAULIFLOWER) != 0) items.Add(190);
                if ((State & CompressedFlags.PANTRY_QUALITY_MELON) != 0) items.Add(254);
                if ((State & CompressedFlags.PANTRY_QUALITY_BLUEBERRY) != 0) items.Add(258);
                if ((State & CompressedFlags.PANTRY_QUALITY_HOT_PEPPER) != 0) items.Add(260);
                if ((State & CompressedFlags.PANTRY_QUALITY_PUMPKIN) != 0) items.Add(276);
                if ((State & CompressedFlags.PANTRY_QUALITY_YAM) != 0) items.Add(280);
                if ((State & CompressedFlags.PANTRY_QUALITY_EGGPLANT) != 0) items.Add(272);
                items.Add(270);
                bundles.Add("Quality Crops", items);
            }
            if ((State & CompressedFlags.PANTRY_RARE) != 0) bundles.Add("Rare Crops", new List<int>() { 454, 417 });
            if ((State & CompressedFlags.PANTRY_ARTISAN) != 0) bundles.Add("Artisan", new List<int>() { 432, 428, 426, 424, 340, 344, 613, 634, 635, 636, 637, 638 });
            if ((State & CompressedFlags.PANTRY_BREWER) != 0) bundles.Add("Brewer's", new List<int>() { 459, 348, 350, 303, 614 });
            if ((State & CompressedFlags.PANTRY_ANIMAL) != 0) bundles.Add("Animal", new List<int>() { 186, 182, 174, 438, 440, 442 });
            if ((State & CompressedFlags.PANTRY_FISH_FARMER) != 0) bundles.Add("Fish Farmer's", new List<int>() { 812, 447, 814 });
            if ((State & CompressedFlags.PANTRY_GARDEN) != 0) bundles.Add("Garden", new List<int>() { 591, 593, 595, 597, 421 });

            // fish
            bundles.Add("River Fish", new List<int>() { 145, 143, 706, 699 });
            bundles.Add("Lake Fish", new List<int>() { 136, 142, 700, 698 });
            bundles.Add("Ocean Fish", new List<int>() { 131, 130, 150, 701 });
            bundles.Add("Night Fishing", new List<int>() { 140, 132, 148 });
            bundles.Add("Crab Pot", new List<int>() { 715, 716, 717, 718, 719, 720, 721, 722, 723, 372 });
            if ((State & CompressedFlags.FISH_MASTER) != 0) bundles.Add("Master Fisher's", new List<int>() { 162, 165, 149, 800 });
            if ((State & CompressedFlags.FISH_QUALITY) != 0) bundles.Add("Quality Fish", new List<int>() { 136, 706, 130, 140 });
            if ((State & CompressedFlags.FISH_SPECIALITY) != 0) bundles.Add("Specialty Fish", new List<int>() { 128, 156, 164, 734 });
            // boiler
            if ((State & CompressedFlags.BOILER_BLACKSMITH) != 0) bundles.Add("Blacksmith's", new List<int>() { 334, 335, 336 });
            if ((State & CompressedFlags.BOILER_GEOLOGIST) != 0) bundles.Add("Geologist's", new List<int>() { 80, 86, 84, 82 });
            if ((State & CompressedFlags.BOILER_ADVENTURER) != 0) {
                var items = new List<int>();

                if ((State & CompressedFlags.BOILER_ADVENTURER_SLIME) != 0) items.Add(766);
                if ((State & CompressedFlags.BOILER_ADVENTURER_BAT_WING) != 0) items.Add(767);
                if ((State & CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE) != 0) items.Add(768);
                if ((State & CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE) != 0) items.Add(769);
                if ((State & CompressedFlags.BOILER_ADVENTURER_BONE_FRAGMENTS) != 0) items.Add(881);
                bundles.Add("Adventurer's", items);
            }
            if ((State & CompressedFlags.BOILER_TREASURE_HUNTER) != 0) bundles.Add("Treasure Hunter's", new List<int>() { 66, 68, 60, 72, 64, 62 });
            if ((State & CompressedFlags.BOILER_ENGINEER) != 0) bundles.Add("Engineer's", new List<int>() { 386, 787, 338 });
            // bulletin
            if ((State & CompressedFlags.BULLETIN_CHEF) != 0) bundles.Add("Chef's", new List<int>() { 724, 259, 430, 376, 228, 194 });
            if ((State & CompressedFlags.BULLETIN_DYE) != 0) {
                var items = new List<int>();
                if ((State & CompressedFlags.BULLETIN_DYE_RED_MUSHROOM) != 0) items.Add(420);
                if ((State & CompressedFlags.BULLETIN_DYE_BEET) != 0) items.Add(284);
                if ((State & CompressedFlags.BULLETIN_DYE_SEA_URCHIN) != 0) items.Add(397);
                if ((State & CompressedFlags.BULLETIN_DYE_AMARANTH) != 0) items.Add(300);
                if ((State & CompressedFlags.BULLETIN_DYE_SUNFLOWER) != 0) items.Add(421);
                if ((State & CompressedFlags.BULLETIN_DYE_STARFRUIT) != 0) items.Add(268);
                if ((State & CompressedFlags.BULLETIN_DYE_DUCK_FEATHER) != 0) items.Add(444);
                if ((State & CompressedFlags.BULLETIN_DYE_CACTUS_FRUIT) != 0) items.Add(90);
                if ((State & CompressedFlags.BULLETIN_DYE_AQUAMARINE) != 0) items.Add(62);
                if ((State & CompressedFlags.BULLETIN_DYE_BLUEBERRY) != 0) items.Add(258);
                if ((State & CompressedFlags.BULLETIN_DYE_RED_CABBAGE) != 0) items.Add(266);
                if ((State & CompressedFlags.BULLETIN_DYE_IRIDIUM_BAR) != 0) items.Add(337);
                bundles.Add("Dye", items);
            }
            if ((State & CompressedFlags.BULLETIN_FIELD_RESEARCH) != 0) bundles.Add("Field Research", new List<int>() { 422, 392, 702, 536 });
            if ((State & CompressedFlags.BULLETIN_FODDER) != 0) bundles.Add("Fodder", new List<int>() { 262, 178, 613});
            if ((State & CompressedFlags.BULLETIN_ENCHANTER) != 0) bundles.Add("Enchanter's", new List<int>() { 725, 348, 446, 637 });
            if ((State & CompressedFlags.BULLETIN_CHILDREN) != 0) bundles.Add("Children's", new List<int>() { 103, 233, 223, 296 });
            if ((State & CompressedFlags.BULLETIN_FORAGER) != 0) bundles.Add("Forager's", new List<int>() { 296, 410, 406 });
            if ((State & CompressedFlags.BULLETIN_HOME_COOK) != 0) bundles.Add("Home Cook's", new List<int>() { -1, -1, 246 });
            //
            return bundles;
        }
    }
}

