using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AequusRemake.Content.Configuration;

public class VanillaChangesConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static VanillaChangesConfig Instance;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool SlimeCrownInSurfaceChests { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveMagicConch { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveTreasureMagnet { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveToolbelt { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveGravityGlobe { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MovePortalGun { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public bool RestorationPotionRecipe { get; set; }
}