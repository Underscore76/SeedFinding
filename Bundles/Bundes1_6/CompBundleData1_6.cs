// Compressed version of BundleData
using System;
using System.Collections.Generic;
using System.Numerics;
using SeedFinding.Bundles;

namespace SeedFinding.Bundles1_6
{
    public class CompBundleData
    {
        public string Name;
        public int Index;
        public BigInteger BaseFlag;
        public List<List<BigInteger>> Flags;
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
            Flags = new List<List<BigInteger>>();
            switch (Name)
            {
                case "Spring Foraging":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.CRAFTS_SPRING_FORAGE_WILD_HORSERADISH,
                            CompressedFlags.CRAFTS_SPRING_FORAGE_DAFFODIL,
                            CompressedFlags.CRAFTS_SPRING_FORAGE_LEEK,
                            CompressedFlags.CRAFTS_SPRING_FORAGE_DANDELION,
                            CompressedFlags.CRAFTS_SPRING_FORAGE_SPRING_ONION
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Winter Foraging":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.CRAFTS_WINTER_FORAGE_WINTER_ROOT,
                            CompressedFlags.CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT,
                            CompressedFlags.CRAFTS_WINTER_FORAGE_SNOW_YAM,
                            CompressedFlags.CRAFTS_WINTER_FORAGE_CROCUS,
                            CompressedFlags.CRAFTS_WINTER_FORAGE_HOLLY
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Construction":
                    BaseFlag = CompressedFlags.CRAFTS_CONSTRUCTION;
                    break;
                case "Sticky":
                    BaseFlag = CompressedFlags.CRAFTS_STICKY;
                    break;
                case "Forest":
                    BaseFlag = CompressedFlags.CRAFTS_FOREST;
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.CRAFTS_FOREST_MOSS,
                            CompressedFlags.CRAFTS_FOREST_FIBER,
                            CompressedFlags.CRAFTS_FOREST_ACORN,
                            CompressedFlags.CRAFTS_FOREST_MAPLE_SEED
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Exotic Foraging":
                    BaseFlag = CompressedFlags.CRAFTS_EXOTIC;
                    break;
                case "Wild Medicine":
                    BaseFlag = CompressedFlags.CRAFTS_WILD_MEDICINE;
                    break;

                case "Spring Crops":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_SPRING_CROPS_PARSNIP,
                            CompressedFlags.PANTRY_SPRING_CROPS_GREEN_BEAN,
                            CompressedFlags.PANTRY_SPRING_CROPS_CAULIFLOWER,
                            CompressedFlags.PANTRY_SPRING_CROPS_POTATO,
                            CompressedFlags.PANTRY_SPRING_CROPS_KALE,
                            CompressedFlags.PANTRY_SPRING_CROPS_CARROT
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Summer Crops":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_SUMMER_CROPS_TOMATO,
                            CompressedFlags.PANTRY_SUMMER_CROPS_HOT_PEPPER,
                            CompressedFlags.PANTRY_SUMMER_CROPS_BLUEBERRY,
                            CompressedFlags.PANTRY_SUMMER_CROPS_MELON,
                            CompressedFlags.PANTRY_SUMMER_CROPS_SUMMER_SQUASH
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Fall Crops":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_FALL_CROPS_CORN,
                            CompressedFlags.PANTRY_FALL_CROPS_EGGPLANT,
                            CompressedFlags.PANTRY_FALL_CROPS_PUMPKIN,
                            CompressedFlags.PANTRY_FALL_CROPS_YAM,
                            CompressedFlags.PANTRY_FALL_CROPS_BROCCOLI
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Quality Crops":
                    BaseFlag = CompressedFlags.PANTRY_QUALITY;
                    HasRandom = true;
                    Flags.Add(new List<BigInteger>() { 0 });
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_QUALITY_PUMPKIN,
                            CompressedFlags.PANTRY_QUALITY_EGGPLANT,
                            CompressedFlags.PANTRY_QUALITY_YAM
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_QUALITY_MELON,
                            CompressedFlags.PANTRY_QUALITY_HOT_PEPPER,
                            CompressedFlags.PANTRY_QUALITY_BLUEBERRY
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.PANTRY_QUALITY_PARSNIP,
                            CompressedFlags.PANTRY_QUALITY_GREEN_BEAN,
                            CompressedFlags.PANTRY_QUALITY_CAULIFLOWER,
                            CompressedFlags.PANTRY_QUALITY_POTATO
                        };
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
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BOILER_ADVENTURER_SLIME,
                            CompressedFlags.BOILER_ADVENTURER_BAT_WING,
                            CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE,
                            CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE,
                            CompressedFlags.BOILER_ADVENTURER_BONE_FRAGMENTS
                        };
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
                case "Dye":
                    BaseFlag = CompressedFlags.BULLETIN_DYE;
                    HasRandom = true;
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_RED_CABBAGE,
                            CompressedFlags.BULLETIN_DYE_IRIDIUM_BAR
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_AQUAMARINE,
                            CompressedFlags.BULLETIN_DYE_BLUEBERRY
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_DUCK_FEATHER,
                            CompressedFlags.BULLETIN_DYE_CACTUS_FRUIT
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_SUNFLOWER,
                            CompressedFlags.BULLETIN_DYE_STARFRUIT
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_SEA_URCHIN,
                            CompressedFlags.BULLETIN_DYE_AMARANTH
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags.BULLETIN_DYE_RED_MUSHROOM,
                            CompressedFlags.BULLETIN_DYE_BEET
                        };
                        Flags.Add(flag);
                    }
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
                case "Helper's":
                    BaseFlag = CompressedFlags.BULLETIN_HELPERS;
                    break;
                case "Spirit's Eve":
                    BaseFlag = CompressedFlags.BULLETIN_SPIRITS_EVE;
                    break;
                case "Winter Star":
                    BaseFlag = CompressedFlags.BULLETIN_WINTER_STAR;
                    break;

            }
        }
    }

}

