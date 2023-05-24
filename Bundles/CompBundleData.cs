// Compressed version of BundleData
using System;
using System.Collections.Generic;

namespace SeedFinding.Bundles
{
    public class CompBundleData
    {
        public string Name;
        public int Index;
        public ulong BaseFlag;
        public List<List<ulong>> Flags;
        public bool HasRandom = false;
        public int Pick = -1;
        public int RequiredItems = -1;
        public string Reward;

        public CompBundleData(BundleData bundleData)
        {
            Name = bundleData.Name;
            Index = bundleData.Index;
            Pick = bundleData.Pick;
            RequiredItems = bundleData.RequiredItems;
            Reward = bundleData.Reward;
            Flags = new List<List<ulong>>();
            switch (Name)
            {
                case "Spring Foraging":
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.CRAFTS_SPRING_FORAGE_WILD_HORSERADISH);
                        flag.Add(CompressedFlags.CRAFTS_SPRING_FORAGE_DAFFODIL);
                        flag.Add(CompressedFlags.CRAFTS_SPRING_FORAGE_LEEK);
                        flag.Add(CompressedFlags.CRAFTS_SPRING_FORAGE_DANDELION);
                        flag.Add(CompressedFlags.CRAFTS_SPRING_FORAGE_SPRING_ONION);
                        Flags.Add(flag);
                    }
                    break;
                case "Winter Foraging":
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.CRAFTS_WINTER_FORAGE_WINTER_ROOT);
                        flag.Add(CompressedFlags.CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT);
                        flag.Add(CompressedFlags.CRAFTS_WINTER_FORAGE_SNOW_YAM);
                        flag.Add(CompressedFlags.CRAFTS_WINTER_FORAGE_CROCUS);
                        flag.Add(CompressedFlags.CRAFTS_WINTER_FORAGE_HOLLY);
                        Flags.Add(flag);
                    }
                    break;
                case "Construction":
                    BaseFlag = CompressedFlags.CRAFTS_CONSTRUCTION;
                    break;
                case "Sticky":
                    BaseFlag = CompressedFlags.CRAFTS_STICKY;
                    break;
                case "Exotic Foraging":
                    BaseFlag = CompressedFlags.CRAFTS_EXOTIC;
                    break;
                case "Wild Medicine":
                    BaseFlag = CompressedFlags.CRAFTS_WILD_MEDICINE;
                    break;
                case "Quality Crops":
                    BaseFlag = CompressedFlags.PANTRY_QUALITY;
                    HasRandom = true;
                    Flags.Add(new List<ulong>() { 0 });
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.PANTRY_QUALITY_PUMPKIN);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_EGGPLANT);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_YAM);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.PANTRY_QUALITY_MELON);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_HOT_PEPPER);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_BLUEBERRY);
                        Flags.Add(flag);
                    }

                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.PANTRY_QUALITY_PARSNIP);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_GREEN_BEAN);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_CAULIFLOWER);
                        flag.Add(CompressedFlags.PANTRY_QUALITY_POTATO);
                        Flags.Add(flag);
                    }
                    break;
                case "Rare Crops":
                    BaseFlag = CompressedFlags.PANTRY_RARE;
                    break;
                case "Animal":
                    BaseFlag = CompressedFlags.PANTRY_ANIMAL;
                    break;
                case "Fish Farmer's":
                    BaseFlag = CompressedFlags.PANTRY_FISH_FARMER;
                    break;
                case "Garden":
                    BaseFlag = CompressedFlags.PANTRY_GARDEN;
                    break;
                case "Artisan":
                    BaseFlag = CompressedFlags.PANTRY_ARTISAN;
                    break;
                case "Brewer's":
                    BaseFlag = CompressedFlags.PANTRY_BREWER;
                    break;

                case "Specialty Fish":
                    BaseFlag = CompressedFlags.FISH_SPECIALITY;
                    break;
                case "Quality Fish":
                    BaseFlag = CompressedFlags.FISH_QUALITY;
                    break;
                case "Master Fisher's":
                    BaseFlag = CompressedFlags.FISH_MASTER;
                    break;

                case "Blacksmith's":
                    BaseFlag = CompressedFlags.BOILER_BLACKSMITH;
                    break;
                case "Geologist's":
                    BaseFlag = CompressedFlags.BOILER_GEOLOGIST;
                    break;
                case "Adventurer's":
                    BaseFlag = CompressedFlags.BOILER_ADVENTURER;
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BOILER_ADVENTURER_SLIME);
                        flag.Add(CompressedFlags.BOILER_ADVENTURER_BAT_WING);
                        flag.Add(CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE);
                        flag.Add(CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE);
                        flag.Add(CompressedFlags.BOILER_ADVENTURER_BONE_FRAGMENTS);
                        Flags.Add(flag);
                    }
                    break;
                case "Treasure Hunter's":
                    BaseFlag = CompressedFlags.BOILER_TREASURE_HUNTER;
                    break;
                case "Engineer's":
                    BaseFlag = CompressedFlags.BOILER_ENGINEER;
                    break;

                case "Chef's":
                    BaseFlag = CompressedFlags.BULLETIN_CHEF;
                    break;
                case "Field Research":
                    BaseFlag = CompressedFlags.BULLETIN_FIELD_RESEARCH;
                    break;
                case "Fodder":
                    BaseFlag = CompressedFlags.BULLETIN_FODDER;
                    break;
                case "Enchanter's":
                    BaseFlag = CompressedFlags.BULLETIN_ENCHANTER;
                    break;
                case "Children's":
                    BaseFlag = CompressedFlags.BULLETIN_CHILDREN;
                    break;
                case "Forager's":
                    BaseFlag = CompressedFlags.BULLETIN_FORAGER;
                    break;
                case "Home Cook's":
                    BaseFlag = CompressedFlags.BULLETIN_HOME_COOK;
                    break;
                case "Dye":
                    BaseFlag = CompressedFlags.BULLETIN_DYE;
                    HasRandom = true;
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_RED_CABBAGE);
                        flag.Add(CompressedFlags.BULLETIN_DYE_IRIDIUM_BAR);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_AQUAMARINE);
                        flag.Add(CompressedFlags.BULLETIN_DYE_BLUEBERRY);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_DUCK_FEATHER);
                        flag.Add(CompressedFlags.BULLETIN_DYE_CACTUS_FRUIT);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_SUNFLOWER);
                        flag.Add(CompressedFlags.BULLETIN_DYE_STARFRUIT);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_SEA_URCHIN);
                        flag.Add(CompressedFlags.BULLETIN_DYE_AMARANTH);
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<ulong>();
                        flag.Add(CompressedFlags.BULLETIN_DYE_RED_MUSHROOM);
                        flag.Add(CompressedFlags.BULLETIN_DYE_BEET);
                        Flags.Add(flag);
                    }
                    break;
            }
        }
    }

}

