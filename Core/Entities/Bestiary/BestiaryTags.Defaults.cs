namespace Aequu2.Core.Entites.Bestiary;

public partial class BestiaryTags {
    public static readonly BestiaryTags Corruption = new BestiaryTags(
        BestiaryBiomeTag.TheCorruption,
        BestiaryBiomeTag.UndergroundCorruption,
        BestiaryBiomeTag.CorruptDesert,
        BestiaryBiomeTag.CorruptUndergroundDesert,
        BestiaryBiomeTag.CorruptIce
    );

    public static readonly BestiaryTags Crimson = new BestiaryTags(
        BestiaryBiomeTag.TheCrimson,
        BestiaryBiomeTag.UndergroundCrimson,
        BestiaryBiomeTag.CrimsonDesert,
        BestiaryBiomeTag.CrimsonUndergroundDesert,
        BestiaryBiomeTag.CrimsonIce
    );

    public static readonly BestiaryTags Hallow = new BestiaryTags(
        BestiaryBiomeTag.TheHallow,
        BestiaryBiomeTag.UndergroundHallow,
        BestiaryBiomeTag.HallowDesert,
        BestiaryBiomeTag.HallowUndergroundDesert,
        BestiaryBiomeTag.HallowIce
    );

    public static readonly BestiaryTags SolarPillar = new BestiaryTags(
        BestiaryBiomeTag.SolarPillar
    );

    public static readonly BestiaryTags VortexPillar = new BestiaryTags(
        BestiaryBiomeTag.VortexPillar
    );

    public static readonly BestiaryTags NebulaPillar = new BestiaryTags(
        BestiaryBiomeTag.NebulaPillar
    );

    public static readonly BestiaryTags StardustPillar = new BestiaryTags(
        BestiaryBiomeTag.StardustPillar
    );

    public static readonly BestiaryTags CelestialPillars = new BestiaryTags(SolarPillar, VortexPillar, NebulaPillar, StardustPillar);

    public static readonly BestiaryTags Underworld = new BestiaryTags(
        BestiaryBiomeTag.TheUnderworld
    );

    public static readonly BestiaryTags Snow = new BestiaryTags(
        BestiaryInvasionTag.FrostLegion,
        BestiaryBiomeTag.Snow,
        BestiaryBiomeTag.UndergroundSnow,
        BestiaryBiomeTag.CorruptIce,
        BestiaryBiomeTag.CrimsonIce,
        BestiaryBiomeTag.HallowIce
    );

    public static readonly BestiaryTags Jungle = new BestiaryTags(
        BestiaryBiomeTag.Jungle,
        BestiaryBiomeTag.UndergroundJungle,
        BestiaryBiomeTag.TheTemple
    );

    public static readonly BestiaryTags GlowingMushroom = new BestiaryTags(
        BestiaryBiomeTag.SurfaceMushroom,
        BestiaryBiomeTag.UndergroundMushroom
    );

    public static readonly BestiaryTags Ocean = new BestiaryTags(
        BestiaryBiomeTag.Ocean
    );

    public static readonly BestiaryTags SkySpace = new BestiaryTags(
        BestiaryBiomeTag.Sky
    );

    public static readonly BestiaryTags Eclipse = new BestiaryTags(
        BestiaryEventTag.Eclipse
    );

    public static readonly BestiaryTags BloodMoon = new BestiaryTags(
        BestiaryEventTag.BloodMoon
    );

    public static readonly BestiaryTags GoblinInvaison = new BestiaryTags(
        BestiaryInvasionTag.Goblins
    );

    public static readonly BestiaryTags FrostLegion = new BestiaryTags(
        BestiaryInvasionTag.FrostLegion
    );

    public static readonly BestiaryTags FrostMoon = new BestiaryTags(
        BestiaryInvasionTag.FrostMoon
    );

    public static readonly BestiaryTags PumpkinMoon = new BestiaryTags(
        BestiaryInvasionTag.PumpkinMoon
    );

    public static readonly BestiaryTags Rain = new BestiaryTags(
        BestiaryEventTag.Rain
    );
}
