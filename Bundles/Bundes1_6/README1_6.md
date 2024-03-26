# Remixed Bundles

Two implementations exist here:

1. `BundleGenerator.Generate(int seed)` - the debug generator, basically a clone of SDV's implementation with some reduced ops/cutting extra stuff to speed things up. This will return a similar structured result to the existing python bundle scripts. It's a bit slower, and it's a bit slower to test if it satifies your requirements.

2. `RemixedBundles.Generate(int seed)` - the faster generator, this computes a bitmask representation of the complete bundle state that makes checking if your requirements are satisfied significantly easier.

The basis for this is the `CompressedFlags` field, which is a mask of all the different fields you could have in a bundle (full listing is below and visible in `CompressedFlags.cs`). The `CompressedRemixBundles` class is a wrapper around the generated bitmask that provides some helper methods for checking if a bundle satisfies your requirements and backing out the original dictionary representation of the bundles.

An example method to check if a bundle satisfies your requirements is as follows:

```cs
BigInteger DesiredBundles = (
    CompressedFlags.CRAFTS_CONSTRUCTION |
    CompressedFlags.CRAFTS_EXOTIC |
    CompressedFlags.PANTRY_RARE |
    CompressedFlags.FISH_QUALITY
);
bool valid = RemixedBundles.Generate(int seed).Satisfies(DesiredBundles);

```

## Crafts Room Choices

### Spring Forage Selection (set at most 4, if you only care about specific ones, only set those flags)
* `CRAFTS_SPRING_FORAGE_WILD_HORSERADISH`
* `CRAFTS_SPRING_FORAGE_DAFFODIL`
* `CRAFTS_SPRING_FORAGE_LEEK`
* `CRAFTS_SPRING_FORAGE_DANDELION`
* `CRAFTS_SPRING_FORAGE_SPRING_ONION`

### Winter Forage Selection (set at most 4, if you only care about specific ones, only set those flags)
* `CRAFTS_WINTER_FORAGE_WINTER_ROOT`
* `CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT`
* `CRAFTS_WINTER_FORAGE_SNOW_YAM`
* `CRAFTS_WINTER_FORAGE_CROCUS`
* `CRAFTS_WINTER_FORAGE_HOLLY`

* `CRAFTS_CONSTRUCTION` or `CRAFTS_STICKY` or `CRAFTS_FOREST`

### Forest Selection (set at most 3, if you only care about specific ones, only set those flags)
* `CRAFTS_FOREST_MOSS`
* `CRAFTS_FOREST_FIBER`
* `CRAFTS_FOREST_ACORN`
* `CRAFTS_FOREST_MAPLE_SEED`

* `CRAFTS_EXOTIC` or `CRAFTS_WILD_MEDICINE`

## Pantry Choices

### Pantry Crops Selection (set at most 4, if you only care about specific ones, only set those flags)
* `PANTRY_SPRING_CROPS_PARSNIP`
* `PANTRY_SPRING_CROPS_GREEN_BEAN`
* `PANTRY_SPRING_CROPS_CAULIFLOWER`
* `PANTRY_SPRING_CROPS_POTATO`
* `PANTRY_SPRING_CROPS_KALE`
* `PANTRY_SPRING_CROPS_CARROT`

* `PANTRY_SUMMER_CROPS_TOMATO`
* `PANTRY_SUMMER_CROPS_HOT_PEPPER`
* `PANTRY_SUMMER_CROPS_BLUEBERRY`
* `PANTRY_SUMMER_CROPS_MELON`
* `PANTRY_SUMMER_CROPS_SUMMER_SQUASH`

* `PANTRY_FALL_CROPS_CORN`
* `PANTRY_FALL_CROPS_EGGPLANT`
* `PANTRY_FALL_CROPS_PUMPKIN`
* `PANTRY_FALL_CROPS_YAM`
* `PANTRY_FALL_CROPS_BROCCOLI`

* `PANTRY_QUALITY` or `PANTRY_RARE`

### Quality Specific flags (dont set these if you don't choose the Quality bundle, if you don't care between the options, don't set either flag)
* `PANTRY_QUALITY_PARSNIP` or `PANTRY_QUALITY_GREEN_BEAN` or `PANTRY_QUALITY_POTATO` or `PANTRY_QUALITY_CAULIFLOWER`
* `PANTRY_QUALITY_MELON` or `PANTRY_QUALITY_BLUEBERRY` or `PANTRY_QUALITY_HOT_PEPPER`
* `PANTRY_QUALITY_PUMPKIN` or `PANTRY_QUALITY_YAM` or `PANTRY_QUALITY_EGGPLANT`

* `PANTRY_ANIMAL` or `PANTRY_FISH_FARMER` or `PANTRY_GARDEN`
* `PANTRY_ARTISAN` or `PANTRY_BREWER`

## Fish Tank Choices
* `FISH_SPECIALITY` or `FISH_QUALITY` or `FISH_MASTER`

## Boiler Room Choices (set at most 3)
* `BOILER_BLACKSMITH`
* `BOILER_GEOLOGIST`
* `BOILER_ADVENTURER`
* `BOILER_TREASURE_HUNTER`
* `BOILER_ENGINEER`

### Adventurer Specific flags (only set if you choose the Adventurer bundle, set at most 4)
* `BOILER_ADVENTURER_SLIME`
* `BOILER_ADVENTURER_BAT_WING`
* `BOILER_ADVENTURER_SOLAR_ESSENCE`
* `BOILER_ADVENTURER_VOID_ESSENCE`
* `BOILER_ADVENTURER_BONE_FRAGMENTS`

## Bulletin Board Choices (set at most 5)
* `BULLETIN_CHEF`
* `BULLETIN_DYE`
* `BULLETIN_FIELD_RESEARCH`
* `BULLETIN_FODDER`
* `BULLETIN_ENCHANTER`
* `BULLETIN_CHILDREN`
* `BULLETIN_FORAGER`
* `BULLETIN_HOME_COOK`
* `BULLETIN_HELPERS`
* `BULLETIN_SPIRITS_EVE`
* `BULLETIN_WINTER_STAR`

### Dye Specific flags (only set if you choose the Dye bundle, if you don't care between the options, don't set either flag)
* `BULLETIN_DYE_RED_MUSHROOM` or `BULLETIN_DYE_BEET`
* `BULLETIN_DYE_SEA_URCHIN` or `BULLETIN_DYE_AMARANTH`
* `BULLETIN_DYE_SUNFLOWER` or `BULLETIN_DYE_STARFRUIT`
* `BULLETIN_DYE_DUCK_FEATHER` or `BULLETIN_DYE_CACTUS_FRUIT`
* `BULLETIN_DYE_AQUAMARINE` or `BULLETIN_DYE_BLUEBERRY`
* `BULLETIN_DYE_RED_CABBAGE` or `BULLETIN_DYE_IRIDIUM_BAR`
