using AequusRemake.Systems.CrossMod;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace AequusRemake.Core.Entites.Bestiary;

public partial class BestiaryTags {
    public BestiaryTags(params string[] names) : this(GetFiltersFromModBiomeNames(names)) { }

    private static IEnumerable<IFilterInfoProvider> GetFiltersFromModBiomeNames(string[] names) {
        foreach (string name in names) {
            if (!CrossModTools.GetContentFromName(name, out ModBiome modBiome)) {
                break;
            }

            yield return modBiome.ModBiomeBestiaryInfoElement;
        }
    }

    public static BestiaryTags CalamityAstral => new BestiaryTags(
        "CalamityMod/AbovegroundAstralBiome",
        "CalamityMod/AbovegroundAstralBiomeSurface",
        "CalamityMod/AbovegroundAstralDesertBiome",
        "CalamityMod/AbovegroundAstralSnowBiome",
        "CalamityMod/UndergroundAstralBiome"
    );

    public static BestiaryTags CalamitySulphurSea => new BestiaryTags(
        "CalamityMod/SulphurousSeaBiome"
    );

    public static BestiaryTags CalamityAbyss1 => new BestiaryTags(
        "CalamityMod/AbyssLayer1Biome"
    );

    public static BestiaryTags CalamityAbyss2 => new BestiaryTags(
        "CalamityMod/AbyssLayer2Biome"
    );

    public static BestiaryTags CalamityAbyss3 => new BestiaryTags(
        "CalamityMod/AbyssLayer3Biome"
    );

    public static BestiaryTags CalamityAbyss4 => new BestiaryTags(
        "CalamityMod/AbyssLayer4Biome"
    );

    public static BestiaryTags CalamityAbyss => new BestiaryTags(CalamityAbyss1, CalamityAbyss2, CalamityAbyss3, CalamityAbyss4);

    public static BestiaryTags CalamityAcidRain => new BestiaryTags(
        "CalamityMod/AcidRainBiome"
    );

    public static BestiaryTags CalamitySunkenSea => new BestiaryTags(
        "CalamityMod/SunkenSeaBiome",
        "CalamityMod/SunkenSeaBurrowsBiome",
        "CalamityMod/SunkenSeaPolypBiome",
        "CalamityMod/SunkenSeaReefsBiome",
        "CalamityMod/SunkenSeaShoresBiome"
    );

    public static BestiaryTags CalamityBrimstoneCrags => new BestiaryTags(
        "CalamityMod/BrimstoneCragsBiome"
    );

    public static BestiaryTags CalamityArsenalLab => new BestiaryTags(
        "CalamityMod/ArsenalLabBiome"
    );

    public static BestiaryTags ThoriumAquaticDepths => new BestiaryTags(
        "ThoriumMod/DepthsBiome"
    );
}
