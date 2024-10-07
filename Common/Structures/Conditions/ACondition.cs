using Aequus.Common.Utilities;
using System;
using Terraria.Localization;

namespace Aequus.Common.Structures.Conditions;

#pragma warning disable IDE0200 // Remove unnecessary lambda expression
internal static class ACondition {
    #region In Biome
    public static readonly Condition InGlimmerBiome = New("InGlimmer", () => Main.LocalPlayer.InModBiome<Content.Events.GlimmerEvent.GlimmerZone>());

    public static readonly Condition InPeacefulGlimmerBiome = New("InPeacefulGlimmer", () => Main.LocalPlayer.InModBiome<Content.Events.GlimmerEvent.Peaceful.PeacefulGlimmerZone>());

    public static readonly Condition InDemonSiegeBiome = New("InDemonSiege", () => Main.LocalPlayer.InModBiome<Content.Events.DemonSiege.DemonSiegeZone>());

    public static readonly Condition InGaleStreamsBiome = New("InGaleStreams", () => Main.LocalPlayer.InModBiome<Content.Events.GaleStreams.GaleStreamsZone>());

    public static readonly Condition InMeadowsBiome = New("InMeadows", () => Main.LocalPlayer.InModBiome<Content.Biomes.Meadows.MeadowsBiome>());

    public static readonly Condition InOblivionAltarBiome = New("InGoreNest", () => Main.LocalPlayer.InModBiome<Content.Biomes.Oblivion.OblivionAltarBiome>());

#if POLLUTED_OCEAN
    public static readonly Condition InPollutedOceanSurfaceBiome = New("InPollutedOceanSurface", () => Main.LocalPlayer.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanSurface>());
    public static readonly Condition InPollutedOceanUndergroundBiome = New("InPollutedOceanUnderground", () => Main.LocalPlayer.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanUnderground>());
    public static readonly Condition InPollutedOceanBiome = New("InPollutedOcean", () => Main.LocalPlayer.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanSurface>() || Main.LocalPlayer.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanUnderground>());
#endif

#if !CRAB_CREVICE_DISABLE
    public static readonly Condition InCrabCreviceBiome = New("InCrabCrevice", () => Main.LocalPlayer.InModBiome<Content.Biomes.CrabCrevice.CrabCreviceBiome>());
#endif
    #endregion

    public static LocalizedText GetText(string name) {
        string key = $"Condition.{name}";
        return ALanguage.GetText(key);
    }

    public static Condition New(string Key, Func<bool> Condition) {
        return new Condition(GetText(Key), Condition);
    }
}
#pragma warning restore IDE0200 // Remove unnecessary lambda expression
