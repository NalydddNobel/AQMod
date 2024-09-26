using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences;
public class GameplayConfig : ConfigurationBase, IPostSetupContent {
    public static GameplayConfig Instance;

    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("General")]

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool ShowNPCSettleDownMessage { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(0.6f)]
    [Range(0.6f, 1f)]
    [Increment(0.05f)]
    [DefaultSliderColor]
    public float DamageReductionCap { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool AdamantiteMimics { get; set; }

    [Header("World")]

#if POLLUTED_OCEAN
    [DefaultBackgroundColor]
    public Content.Biomes.PollutedOcean.Generation.PollutedOceanGenerationSideConfig PollutedOceanSide { get; set; }
#endif

    [DefaultBackgroundColor]
    [DefaultValue(1f)]
    [Range(0f, 1f)]
    public float CaveVariety { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool HardmodeChests { get; set; }

    [Header("Recipes")]

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool VoidBagRecipe { get; set; }

    [Header("VanillaChanges")]

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EyeOfCthulhuOres { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool MoveTreasureMagnet { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyGravityGlobe { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyPortalGun { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyWiring { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyMimics { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyHallow { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyHardmodeMonsters { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyHardmodeVillagers { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyHardmodeShops { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyGreenJellyfish { get; set; }

    [DefaultBackgroundColor]
    [DefaultValue(true)]
    [ReloadRequired]
    public bool EarlyAnglerFish { get; set; }

    public static Condition ConditionEarlyPortalGun => new(TextHelper.GetOrRegister("Condition.EarlyPortalGun"), () => Instance.EarlyPortalGun);
    public static Condition ConditionEarlyGravityGlobe => new(TextHelper.GetOrRegister("Condition.EarlyGravityGlobe"), () => Instance.EarlyGravityGlobe);
}