/* 
 * bit mask used for defining bundle configurations
 * 
 * These are initiated in order, each one corresponding to the next bit in a BigInteger which is as large as necessary
 * 
 * You can define a specific state you're interested in searching for by
 * ORing these flags together, for example
 * 
 * BigInteger searchState = (
 *  CRAFTS_CONSTRUCTION |
 *  BOILER_ENGINEER
 * )
 * 
 * will limit your seed search to those that have the construction bundle and the engineer bundle.
 */

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
using SeedFinding.Bundles;

namespace SeedFinding.Bundles1_6
{
    public class CompressedFlags
    {
        static CompressedFlags()
        {
            BigInteger flag = new BigInteger();
            FieldInfo[] Flags = typeof(CompressedFlags).GetFields();
            for (int i = 0; i < Flags.Length; i++)
            {
                Flags[i].SetValue(flag, (BigInteger)0b1 << i);
                //Console.WriteLine(Flags[i].Name + " " + Flags[i].GetValue(flag).ToString());
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

            //Forest Selection
                CRAFTS_FOREST_MOSS,
                CRAFTS_FOREST_FIBER,
                CRAFTS_FOREST_ACORN,
                CRAFTS_FOREST_MAPLE_SEED,

            CRAFTS_EXOTIC, CRAFTS_WILD_MEDICINE,
            
        //Pantry
            
            //Spring Crops Selection
                PANTRY_SPRING_CROPS_PARSNIP,
                PANTRY_SPRING_CROPS_GREEN_BEAN,
                PANTRY_SPRING_CROPS_CAULIFLOWER,
                PANTRY_SPRING_CROPS_POTATO,
                PANTRY_SPRING_CROPS_KALE,
                PANTRY_SPRING_CROPS_CARROT,

            //Summer Crops Selection
                PANTRY_SUMMER_CROPS_TOMATO,
                PANTRY_SUMMER_CROPS_HOT_PEPPER,
                PANTRY_SUMMER_CROPS_BLUEBERRY,
                PANTRY_SUMMER_CROPS_MELON,
                PANTRY_SUMMER_CROPS_SUMMER_SQUASH,

            //Fall Crops Selection
                PANTRY_FALL_CROPS_CORN,
                PANTRY_FALL_CROPS_EGGPLANT,
                PANTRY_FALL_CROPS_PUMPKIN,
                PANTRY_FALL_CROPS_YAM,
                PANTRY_FALL_CROPS_BROCCOLI,

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
            BULLETIN_HELPERS,
            BULLETIN_SPIRITS_EVE,
            BULLETIN_WINTER_STAR,

            //Dye selection
                BULLETIN_DYE_RED_MUSHROOM, BULLETIN_DYE_BEET,
                BULLETIN_DYE_SEA_URCHIN, BULLETIN_DYE_AMARANTH,
                BULLETIN_DYE_SUNFLOWER,  BULLETIN_DYE_STARFRUIT,
                BULLETIN_DYE_DUCK_FEATHER, BULLETIN_DYE_CACTUS_FRUIT,
                BULLETIN_DYE_AQUAMARINE, BULLETIN_DYE_BLUEBERRY,
                BULLETIN_DYE_RED_CABBAGE, BULLETIN_DYE_IRIDIUM_BAR;

    }

}

