using AequusRemake.Content.Biomes.PollutedOcean.Generation;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AequusRemake.Content.Configuration;
internal class GameplayConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ClientSide;

    public static GameplayConfig Instance;

    [DefaultValue(PollutedOceanGenerationSideConfig.Automatic)]
    public PollutedOceanGenerationSideConfig PollutedOceanSide;

    [DefaultValue(1f)]
    [Range(0f, 1f)]
    public float CaveVariety;

    [DefaultValue(true)]
    public bool TownNPCSettleDownMessage;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool PreHardmodeMimics;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool ShadowMimics;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool HardmodeMimics;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool HardmodeChests;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool PDAGetsAequusRemakeItems;
}
