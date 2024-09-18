namespace Aequus.Common.Entities.Bestiary;

public partial class BestiaryTagCollection {
    public static readonly BestiaryTagCollection Corruption = new BestiaryTagCollection(
        BestiaryBiomeTag.TheCorruption,
        BestiaryBiomeTag.UndergroundCorruption,
        BestiaryBiomeTag.CorruptDesert,
        BestiaryBiomeTag.CorruptUndergroundDesert,
        BestiaryBiomeTag.CorruptIce
    );

    public static readonly BestiaryTagCollection Crimson = new BestiaryTagCollection(
        BestiaryBiomeTag.TheCrimson,
        BestiaryBiomeTag.UndergroundCrimson,
        BestiaryBiomeTag.CrimsonDesert,
        BestiaryBiomeTag.CrimsonUndergroundDesert,
        BestiaryBiomeTag.CrimsonIce
    );

    public static readonly BestiaryTagCollection Hallow = new BestiaryTagCollection(
        BestiaryBiomeTag.TheHallow,
        BestiaryBiomeTag.UndergroundHallow,
        BestiaryBiomeTag.HallowDesert,
        BestiaryBiomeTag.HallowUndergroundDesert,
        BestiaryBiomeTag.HallowIce
    );

    public static readonly BestiaryTagCollection SolarPillar = new BestiaryTagCollection(
        BestiaryBiomeTag.SolarPillar
    );

    public static readonly BestiaryTagCollection VortexPillar = new BestiaryTagCollection(
        BestiaryBiomeTag.VortexPillar
    );

    public static readonly BestiaryTagCollection NebulaPillar = new BestiaryTagCollection(
        BestiaryBiomeTag.NebulaPillar
    );

    public static readonly BestiaryTagCollection StardustPillar = new BestiaryTagCollection(
        BestiaryBiomeTag.StardustPillar
    );

    public static readonly BestiaryTagCollection CelestialPillars = new BestiaryTagCollection(SolarPillar, VortexPillar, NebulaPillar, StardustPillar);

    public static readonly BestiaryTagCollection Underworld = new BestiaryTagCollection(
        BestiaryBiomeTag.TheUnderworld
    );

    public static readonly BestiaryTagCollection Snow = new BestiaryTagCollection(
        BestiaryInvasionTag.FrostLegion,
        BestiaryBiomeTag.Snow,
        BestiaryBiomeTag.UndergroundSnow,
        BestiaryBiomeTag.CorruptIce,
        BestiaryBiomeTag.CrimsonIce,
        BestiaryBiomeTag.HallowIce
    );

    public static readonly BestiaryTagCollection Jungle = new BestiaryTagCollection(
        BestiaryBiomeTag.Jungle,
        BestiaryBiomeTag.UndergroundJungle,
        BestiaryBiomeTag.TheTemple
    );

    public static readonly BestiaryTagCollection GlowingMushroom = new BestiaryTagCollection(
        BestiaryBiomeTag.SurfaceMushroom,
        BestiaryBiomeTag.UndergroundMushroom
    );

    public static readonly BestiaryTagCollection Ocean = new BestiaryTagCollection(
        BestiaryBiomeTag.Ocean
    );

    public static readonly BestiaryTagCollection SkySpace = new BestiaryTagCollection(
        BestiaryBiomeTag.Sky
    );

    public static readonly BestiaryTagCollection Eclipse = new BestiaryTagCollection(
        BestiaryEventTag.Eclipse
    );

    public static readonly BestiaryTagCollection BloodMoon = new BestiaryTagCollection(
        BestiaryEventTag.BloodMoon
    );

    public static readonly BestiaryTagCollection GoblinInvaison = new BestiaryTagCollection(
        BestiaryInvasionTag.Goblins
    );

    public static readonly BestiaryTagCollection FrostLegion = new BestiaryTagCollection(
        BestiaryInvasionTag.FrostLegion
    );

    public static readonly BestiaryTagCollection FrostMoon = new BestiaryTagCollection(
        BestiaryInvasionTag.FrostMoon
    );

    public static readonly BestiaryTagCollection PumpkinMoon = new BestiaryTagCollection(
        BestiaryInvasionTag.PumpkinMoon
    );

    public static readonly BestiaryTagCollection Rain = new BestiaryTagCollection(
        BestiaryEventTag.Rain
    );
}
