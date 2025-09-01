// Compressed version of BundleData
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SeedFinding.Bundles1_5
{
    public class CompBundleData1_5
    {
        public string Name;
        public int Index;
        public BigInteger BaseFlag;
        public List<List<BigInteger>> Flags;
        public bool HasRandom = false;
        public int Pick = -1;
        public int RequiredItems = -1;
        public string Reward;

        public CompBundleData1_5(BundleData bundleData)
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
                            CompressedFlags1_5.CRAFTS_SPRING_FORAGE_WILD_HORSERADISH,
                            CompressedFlags1_5.CRAFTS_SPRING_FORAGE_DAFFODIL,
                            CompressedFlags1_5.CRAFTS_SPRING_FORAGE_LEEK,
                            CompressedFlags1_5.CRAFTS_SPRING_FORAGE_DANDELION,
                            CompressedFlags1_5.CRAFTS_SPRING_FORAGE_SPRING_ONION
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Winter Foraging":
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.CRAFTS_WINTER_FORAGE_WINTER_ROOT,
                            CompressedFlags1_5.CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT,
                            CompressedFlags1_5.CRAFTS_WINTER_FORAGE_SNOW_YAM,
                            CompressedFlags1_5.CRAFTS_WINTER_FORAGE_CROCUS,
                            CompressedFlags1_5.CRAFTS_WINTER_FORAGE_HOLLY
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Construction":
                    BaseFlag = CompressedFlags1_5.CRAFTS_CONSTRUCTION;
                    break;
                case "Sticky":
                    BaseFlag = CompressedFlags1_5.CRAFTS_STICKY;
                    break;
                case "Exotic Foraging":
                    BaseFlag = CompressedFlags1_5.CRAFTS_EXOTIC;
                    break;
                case "Wild Medicine":
                    BaseFlag = CompressedFlags1_5.CRAFTS_WILD_MEDICINE;
                    break;
                case "Quality Crops":
                    BaseFlag = CompressedFlags1_5.PANTRY_QUALITY;
                    HasRandom = true;
                    Flags.Add(new List<BigInteger>() { 0 });
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.PANTRY_QUALITY_PUMPKIN,
                            CompressedFlags1_5.PANTRY_QUALITY_EGGPLANT,
                            CompressedFlags1_5.PANTRY_QUALITY_YAM
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.PANTRY_QUALITY_MELON,
                            CompressedFlags1_5.PANTRY_QUALITY_HOT_PEPPER,
                            CompressedFlags1_5.PANTRY_QUALITY_BLUEBERRY
                        };
                        Flags.Add(flag);
                    }

                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.PANTRY_QUALITY_PARSNIP,
                            CompressedFlags1_5.PANTRY_QUALITY_GREEN_BEAN,
                            CompressedFlags1_5.PANTRY_QUALITY_CAULIFLOWER,
                            CompressedFlags1_5.PANTRY_QUALITY_POTATO
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Rare Crops":
                    BaseFlag = CompressedFlags1_5.PANTRY_RARE;
                    break;
                case "Animal":
                    BaseFlag = CompressedFlags1_5.PANTRY_ANIMAL;
                    break;
                case "Fish Farmer's":
                    BaseFlag = CompressedFlags1_5.PANTRY_FISH_FARMER;
                    break;
                case "Garden":
                    BaseFlag = CompressedFlags1_5.PANTRY_GARDEN;
                    break;
                case "Artisan":
                    BaseFlag = CompressedFlags1_5.PANTRY_ARTISAN;
                    break;
                case "Brewer's":
                    BaseFlag = CompressedFlags1_5.PANTRY_BREWER;
                    break;

                case "Specialty Fish":
                    BaseFlag = CompressedFlags1_5.FISH_SPECIALITY;
                    break;
                case "Quality Fish":
                    BaseFlag = CompressedFlags1_5.FISH_QUALITY;
                    break;
                case "Master Fisher's":
                    BaseFlag = CompressedFlags1_5.FISH_MASTER;
                    break;

                case "Blacksmith's":
                    BaseFlag = CompressedFlags1_5.BOILER_BLACKSMITH;
                    break;
                case "Geologist's":
                    BaseFlag = CompressedFlags1_5.BOILER_GEOLOGIST;
                    break;
                case "Adventurer's":
                    BaseFlag = CompressedFlags1_5.BOILER_ADVENTURER;
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BOILER_ADVENTURER_SLIME,
                            CompressedFlags1_5.BOILER_ADVENTURER_BAT_WING,
                            CompressedFlags1_5.BOILER_ADVENTURER_SOLAR_ESSENCE,
                            CompressedFlags1_5.BOILER_ADVENTURER_VOID_ESSENCE,
                            CompressedFlags1_5.BOILER_ADVENTURER_BONE_FRAGMENTS
                        };
                        Flags.Add(flag);
                    }
                    break;
                case "Treasure Hunter's":
                    BaseFlag = CompressedFlags1_5.BOILER_TREASURE_HUNTER;
                    break;
                case "Engineer's":
                    BaseFlag = CompressedFlags1_5.BOILER_ENGINEER;
                    break;

                case "Chef's":
                    BaseFlag = CompressedFlags1_5.BULLETIN_CHEF;
                    break;
                case "Field Research":
                    BaseFlag = CompressedFlags1_5.BULLETIN_FIELD_RESEARCH;
                    break;
                case "Fodder":
                    BaseFlag = CompressedFlags1_5.BULLETIN_FODDER;
                    break;
                case "Enchanter's":
                    BaseFlag = CompressedFlags1_5.BULLETIN_ENCHANTER;
                    break;
                case "Children's":
                    BaseFlag = CompressedFlags1_5.BULLETIN_CHILDREN;
                    break;
                case "Forager's":
                    BaseFlag = CompressedFlags1_5.BULLETIN_FORAGER;
                    break;
                case "Home Cook's":
                    BaseFlag = CompressedFlags1_5.BULLETIN_HOME_COOK;
                    break;
                case "Dye":
                    BaseFlag = CompressedFlags1_5.BULLETIN_DYE;
                    HasRandom = true;
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_RED_CABBAGE,
                            CompressedFlags1_5.BULLETIN_DYE_IRIDIUM_BAR
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_AQUAMARINE,
                            CompressedFlags1_5.BULLETIN_DYE_BLUEBERRY
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_DUCK_FEATHER,
                            CompressedFlags1_5.BULLETIN_DYE_CACTUS_FRUIT
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_SUNFLOWER,
                            CompressedFlags1_5.BULLETIN_DYE_STARFRUIT
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_SEA_URCHIN,
                            CompressedFlags1_5.BULLETIN_DYE_AMARANTH
                        };
                        Flags.Add(flag);
                    }
                    {
                        var flag = new List<BigInteger>
                        {
                            CompressedFlags1_5.BULLETIN_DYE_RED_MUSHROOM,
                            CompressedFlags1_5.BULLETIN_DYE_BEET
                        };
                        Flags.Add(flag);
                    }
                    break;
            }
        }
    }

}

