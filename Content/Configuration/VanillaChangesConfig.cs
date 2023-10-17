using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Content.Configuration;

public class VanillaChangesConfig : ModConfig {
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static VanillaChangesConfig Instance;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveTreasureMagnet;

    [DefaultValue(true)]
    [ReloadRequired]
    public bool RestorationPotionRecipe;
}