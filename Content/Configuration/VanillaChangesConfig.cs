using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;

public class VanillaChangesConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static VanillaChangesConfig Instance;

    [DefaultValue(true)]
    [ReloadRequired]
    public System.Boolean SlimeCrownInSurfaceChests { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public System.Boolean MoveTreasureMagnet { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public System.Boolean MoveToolbelt { get; set; }

    [DefaultValue(true)]
    [ReloadRequired]
    public System.Boolean RestorationPotionRecipe { get; set; }
}