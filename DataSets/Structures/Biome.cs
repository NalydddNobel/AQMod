namespace Aequus.DataSets.Structures;

public enum Biome : byte {
    Forest = 0,
    Underground,
    Underworld,
    Sky,
    Snow,
    UndergroundSnow,
    Desert,
    UndergroundDesert,
    Ocean,
    GlowingMushroom,
    Meteor,
    Aether,
    Jungle,
    UndergroundJungle,

    Corruption = 100,
    CorruptionUnderground,
    CorruptionDesert,
    CorruptionUndergroundDesert,
    CorruptionSnow,
    CorruptionUndergroundSnow,

    Crimson = 150,
    CrimsonUnderground,
    CrimsonDesert,
    CrimsonUndergroundDesert,
    CrimsonSnow,
    CrimsonUndergroundSnow,

    Hallow = 200,
    HallowUnderground,
    HallowDesert,
    HallowUndergroundDesert,
    HallowSnow,
    HallowUndergroundSnow,

    PollutedOcean = 230,

    Dungeon = 254,
    Temple = 255,
}
