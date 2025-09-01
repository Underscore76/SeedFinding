/* 
 * 64 bit mask used for defining bundle configurations
 * you can define a specific state you're interested in searching for by
 * ORing these flags together, for example
 * ulong searchState = (
 *  CRAFTS_CONSTRUCTION |
 *  BOILER_ENGINEER
 * )
 * will limit your seed search to those that have the construction bundle and the engineer bundle.
 */

using System;
using System.Numerics;
using System.Reflection;
namespace SeedFinding.Bundles1_5
{
    public class CompressedFlags1_5
    {
        static CompressedFlags1_5()
        {
            BigInteger flag = new();
            FieldInfo[] Flags = typeof(CompressedFlags1_5).GetFields();
            for (int i = 0; i < Flags.Length; i++)
            {
                Flags[i].SetValue(flag, (BigInteger)0b1 << i);
            }
        }
        public static BigInteger
        //Crafts Room

            //Spring Forage Selection
                CRAFTS_SPRING_FORAGE_WILD_HORSERADISH,
                CRAFTS_SPRING_FORAGE_DAFFODIL,
                CRAFTS_SPRING_FORAGE_LEEK,
                CRAFTS_SPRING_FORAGE_DANDELION,
                CRAFTS_SPRING_FORAGE_SPRING_ONION,

            //Winter Forage Selection
                CRAFTS_WINTER_FORAGE_WINTER_ROOT,
                CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT,
                CRAFTS_WINTER_FORAGE_SNOW_YAM,
                CRAFTS_WINTER_FORAGE_CROCUS,
                CRAFTS_WINTER_FORAGE_HOLLY,

            CRAFTS_CONSTRUCTION, CRAFTS_STICKY, CRAFTS_FOREST,

            CRAFTS_EXOTIC, CRAFTS_WILD_MEDICINE,
            
        //Pantry
            PANTRY_QUALITY, PANTRY_RARE,

            //Quality Crops Selection
                PANTRY_QUALITY_PARSNIP, PANTRY_QUALITY_GREEN_BEAN, PANTRY_QUALITY_POTATO, PANTRY_QUALITY_CAULIFLOWER,
                PANTRY_QUALITY_MELON, PANTRY_QUALITY_BLUEBERRY, PANTRY_QUALITY_HOT_PEPPER,
                PANTRY_QUALITY_PUMPKIN, PANTRY_QUALITY_YAM, PANTRY_QUALITY_EGGPLANT,

            PANTRY_ANIMAL, PANTRY_FISH_FARMER, PANTRY_GARDEN,

            PANTRY_ARTISAN, PANTRY_BREWER,

        //Fish Tank

            FISH_SPECIALITY, FISH_QUALITY, FISH_MASTER,

        //Boiler Room

            BOILER_BLACKSMITH,
            BOILER_GEOLOGIST,
            BOILER_ADVENTURER,
            BOILER_TREASURE_HUNTER,
            BOILER_ENGINEER,

            //Adventurer selection
                BOILER_ADVENTURER_SLIME,
                BOILER_ADVENTURER_BAT_WING,
                BOILER_ADVENTURER_SOLAR_ESSENCE,
                BOILER_ADVENTURER_VOID_ESSENCE,
                BOILER_ADVENTURER_BONE_FRAGMENTS,

        //Bulletin Board

            BULLETIN_CHEF,
            BULLETIN_DYE,
            BULLETIN_FIELD_RESEARCH,
            BULLETIN_FODDER,
            BULLETIN_ENCHANTER,
            BULLETIN_CHILDREN,
            BULLETIN_FORAGER,
            BULLETIN_HOME_COOK,

            //Dye selection
                BULLETIN_DYE_RED_MUSHROOM, BULLETIN_DYE_BEET,
                BULLETIN_DYE_SEA_URCHIN, BULLETIN_DYE_AMARANTH,
                BULLETIN_DYE_SUNFLOWER,  BULLETIN_DYE_STARFRUIT,
                BULLETIN_DYE_DUCK_FEATHER, BULLETIN_DYE_CACTUS_FRUIT,
                BULLETIN_DYE_AQUAMARINE, BULLETIN_DYE_BLUEBERRY,
                BULLETIN_DYE_RED_CABBAGE, BULLETIN_DYE_IRIDIUM_BAR;

    }

}

