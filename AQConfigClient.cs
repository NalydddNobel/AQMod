using AQMod.Items.Accessories.HookUpgrades;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Items.Dyes;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable;
using AQMod.Items.Weapons.Magic;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod
{
    [Label("$Mods.AQMod.ClientConfig.Name")]
    [BackgroundColor(10, 10, 40, 220)]
    public class AQConfigClient : ModConfig
    {
        public static AQConfigClient Instance;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.AQMod.ClientConfig.Header.Visuals")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.ScreenshakesLabel")]
        [DefaultValue(true)]
        public bool Screenshakes { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectIntensityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float EffectIntensity { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.FlashIntensityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float FlashIntensity { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectQualityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float EffectQuality { get; set; }

        [Header("$Mods.AQMod.ClientConfig.Header.UI")]

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.DemonSiegeUpgradeTooltipLabel")]
        [Tooltip("$Mods.AQMod.ClientConfig.DemonSiegeUpgradeTooltipTooltip")]
        [DefaultValue(true)]
        public bool DemonSiegeUpgradeTooltip { get; set; }

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.HookBarbBlacklistTooltipLabel")]
        [Tooltip("$Mods.AQMod.ClientConfig.HookBarbBlacklistTooltipTooltip")]
        [DefaultValue(true)]
        public bool HookBarbBlacklistTooltip { get; set; }

        [Label("$Mods.AQMod.ClientConfig.ShowCompletedAnglerQuestsCountLabel")]
        [BackgroundColor(12, 44, 190, 180)]
        [DefaultValue(true)]
        public bool ShowCompletedQuestsCount { get; set; }

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.MapUITogglesPositionLabel")]
        [Range(60f, 1920f)]
        [DefaultValue(typeof(Vector2), "60, 60")]
        public Vector2 MapUITogglesPosition { get; set; }

        [Header(AQText.ConfigHeaderKey + "Misc")]

        [BackgroundColor(75, 80, 100, 180)]
        [Label("$Mods.AQMod.ClientConfig.XmasBackgroundLabel")]
        [Tooltip("$Mods.AQMod.ClientConfig.XmasBackgroundTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool XmasBackground { get; set; }

        [BackgroundColor(75, 80, 100, 180)]
        [Label("$Mods.AQMod.ClientConfig.XmasProgressMeterOverrideLabel")]
        [Tooltip("$Mods.AQMod.ClientConfig.XmasProgressMeterOverrideTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool XmasProgressMeterOverride { get; set; }

        public static void LoadTranslations()
        {
            AQText.AdjustTranslation("ClientConfig.Screenshakes", "ClientConfig.ScreenshakesLabel", (s) => AQText.Item(ModContent.ItemType<AArachnotronVisor>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.EffectIntensity", "ClientConfig.EffectIntensityLabel", (s) => AQText.Item(ModContent.ItemType<NalydDye>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.FlashIntensity", "ClientConfig.FlashIntensityLabel", (s) => AQText.Item(ModContent.ItemType<LightMatter>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.EffectQuality", "ClientConfig.EffectQualityLabel", (s) => AQText.Item(ModContent.ItemType<CosmicEnergy>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.DemonSiegeUpgradeTooltip", "ClientConfig.DemonSiegeUpgradeTooltipLabel", (s) => AQText.Item(ModContent.ItemType<GoreNestItem>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.HookBarbBlacklistTooltip", "ClientConfig.HookBarbBlacklistTooltipLabel", (s) => AQText.Item(ModContent.ItemType<Meathook>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.ShowCompletedAnglerQuestsCount", "ClientConfig.ShowCompletedAnglerQuestsCountLabel", (s) => AQText.Item(ItemID.AnglerEarring) + " " + s);
            AQText.AdjustTranslation("ClientConfig.MapUITogglesPosition", "ClientConfig.MapUITogglesPositionLabel", (s) => AQText.Item(ItemID.TrifoldMap) + " " + s);
            AQText.AdjustTranslation("ClientConfig.XmasBackground", "ClientConfig.XmasBackgroundLabel", (s) => AQText.Item(ModContent.ItemType<Snowgrave>()) + " " + s);
            AQText.AdjustTranslation("ClientConfig.XmasProgressMeterOverride", "ClientConfig.XmasProgressMeterOverrideLabel", (s) => AQText.Item(ItemID.SantaHat) + " " + s);
        }
    }
}