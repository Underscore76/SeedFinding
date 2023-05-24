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
namespace SeedFinding.Bundles
{
    public class CompressedFlags
    {
        // Crafts Room 14 bits
        public const ulong CRAFTS_CONSTRUCTION = (ulong)0b01 << 62;
        public const ulong CRAFTS_STICKY = (ulong)0b10 << 62;

        public const ulong CRAFTS_EXOTIC = (ulong)0b01 << 60;
        public const ulong CRAFTS_WILD_MEDICINE = (ulong)0b10 << 60;

        public const ulong CRAFTS_SPRING_FORAGE_WILD_HORSERADISH = (ulong)0b00001 << 55;
        public const ulong CRAFTS_SPRING_FORAGE_DAFFODIL = (ulong)0b00010 << 55;
        public const ulong CRAFTS_SPRING_FORAGE_LEEK = (ulong)0b00100 << 55;
        public const ulong CRAFTS_SPRING_FORAGE_DANDELION = (ulong)0b01000 << 55;
        public const ulong CRAFTS_SPRING_FORAGE_SPRING_ONION = (ulong)0b10000 << 55;

        public const ulong CRAFTS_WINTER_FORAGE_WINTER_ROOT = (ulong)0b00001 << 50;
        public const ulong CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT = (ulong)0b00010 << 50;
        public const ulong CRAFTS_WINTER_FORAGE_SNOW_YAM = (ulong)0b00100 << 50;
        public const ulong CRAFTS_WINTER_FORAGE_CROCUS = (ulong)0b01000 << 50;
        public const ulong CRAFTS_WINTER_FORAGE_HOLLY = (ulong)0b10000 << 50;

        // Pantry 17 bits
        public const ulong PANTRY_QUALITY = (ulong)0b01 << 48;
        public const ulong PANTRY_RARE = (ulong)0b10 << 48;
        public const ulong PANTRY_ARTISAN = (ulong)0b01 << 46;
        public const ulong PANTRY_BREWER = (ulong)0b10 << 46;

        public const ulong PANTRY_ANIMAL = (ulong)0b001 << 43;
        public const ulong PANTRY_FISH_FARMER = (ulong)0b010 << 43;
        public const ulong PANTRY_GARDEN = (ulong)0b100 << 43;

        //0b0010001100100010110000001101101000000000000
        public const ulong PANTRY_QUALITY_PARSNIP = (ulong)0b0001 << 39;
        public const ulong PANTRY_QUALITY_GREEN_BEAN = (ulong)0b0010 << 39;
        public const ulong PANTRY_QUALITY_POTATO = (ulong)0b0100 << 39;
        public const ulong PANTRY_QUALITY_CAULIFLOWER = (ulong)0b1000 << 39;

        public const ulong PANTRY_QUALITY_MELON = (ulong)0b001 << 36;
        public const ulong PANTRY_QUALITY_BLUEBERRY = (ulong)0b010 << 36;
        public const ulong PANTRY_QUALITY_HOT_PEPPER = (ulong)0b100 << 36;

        public const ulong PANTRY_QUALITY_PUMPKIN = (ulong)0b001 << 33;
        public const ulong PANTRY_QUALITY_YAM = (ulong)0b010 << 33;
        public const ulong PANTRY_QUALITY_EGGPLANT = (ulong)0b100 << 33;

        // Fish Tank 3 bits
        public const ulong FISH_SPECIALITY = (ulong)0b001 << 30;
        public const ulong FISH_QUALITY = (ulong)0b010 << 30;
        public const ulong FISH_MASTER = (ulong)0b100 << 30;

        // Boiler 10 bits
        public const ulong BOILER_BLACKSMITH = (ulong)0b00001 << 25;
        public const ulong BOILER_GEOLOGIST = (ulong)0b00010 << 25;
        public const ulong BOILER_ADVENTURER = (ulong)0b00100 << 25;
        public const ulong BOILER_TREASURE_HUNTER = (ulong)0b01000 << 25;
        public const ulong BOILER_ENGINEER = (ulong)0b10000 << 25;

        public const ulong BOILER_ADVENTURER_SLIME = (ulong)0b00001 << 20;
        public const ulong BOILER_ADVENTURER_BAT_WING = (ulong)0b00010 << 20;
        public const ulong BOILER_ADVENTURER_SOLAR_ESSENCE = (ulong)0b00100 << 20;
        public const ulong BOILER_ADVENTURER_VOID_ESSENCE = (ulong)0b01000 << 20;
        public const ulong BOILER_ADVENTURER_BONE_FRAGMENTS = (ulong)0b10000 << 20;

        // Bulletin 20
        public const ulong BULLETIN_CHEF = (ulong)0b00000001 << 12;
        public const ulong BULLETIN_DYE = (ulong)0b00000010 << 12;
        public const ulong BULLETIN_FIELD_RESEARCH = (ulong)0b00000100 << 12;
        public const ulong BULLETIN_FODDER = (ulong)0b00001000 << 12;
        public const ulong BULLETIN_ENCHANTER = (ulong)0b00010000 << 12;
        public const ulong BULLETIN_CHILDREN = (ulong)0b00100000 << 12;
        public const ulong BULLETIN_FORAGER = (ulong)0b01000000 << 12;
        public const ulong BULLETIN_HOME_COOK = (ulong)0b10000000 << 12;

        public const ulong BULLETIN_DYE_RED_MUSHROOM = (ulong)0b01 << 10;
        public const ulong BULLETIN_DYE_BEET = (ulong)0b10 << 10;

        public const ulong BULLETIN_DYE_SEA_URCHIN = (ulong)0b01 << 8;
        public const ulong BULLETIN_DYE_AMARANTH = (ulong)0b10 << 8;

        public const ulong BULLETIN_DYE_SUNFLOWER = (ulong)0b01 << 6;
        public const ulong BULLETIN_DYE_STARFRUIT = (ulong)0b10 << 6;

        public const ulong BULLETIN_DYE_DUCK_FEATHER = (ulong)0b01 << 4;
        public const ulong BULLETIN_DYE_CACTUS_FRUIT = (ulong)0b10 << 4;

        public const ulong BULLETIN_DYE_AQUAMARINE = (ulong)0b01 << 2;
        public const ulong BULLETIN_DYE_BLUEBERRY = (ulong)0b10 << 2;

        public const ulong BULLETIN_DYE_RED_CABBAGE = (ulong)0b01;
        public const ulong BULLETIN_DYE_IRIDIUM_BAR = (ulong)0b10;

    }

}

