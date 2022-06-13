using Aequus.Common;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.BuffPotions;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Summons;
using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Aequus
{
    public sealed class ClientConfig : ConfigurationBase, IPostSetupContent
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static ClientConfig Instance;

        [Header(Key + "Client.Headers.Visuals")]

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.ScreenshakeIntensityLabel")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float ScreenshakeIntensity { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.FlashIntensityLabel")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float FlashIntensity { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.HighQualityLabel")]
        [DefaultValue(true)]
        public bool HighQuality { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.HighQualityShadersLabel")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool HighQualityShaders { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.FlashShaderRepetitionsLabel")]
        [Tooltip(Key + "Client.FlashShaderRepetitionsTooltip")]
        [Increment(4)]
        [DefaultValue(40)]
        [Range(10, 80)]
        [Slider()]
        [SliderColor(30, 50, 120, 255)]
        public int FlashShaderRepetitions { get; set; }

        [Header(Key + "Client.Headers.Misc")]

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.NPCShopQuotesLabel")]
        [Tooltip(Key + "Client.NPCShopQuotesTooltip")]
        [DefaultValue(true)]
        public bool NPCShopQuotes { get; set; }

        [BackgroundColor(80, 80, 130, 180)]
        [Label(Key + "Client.OtherNPCShopQuotes")]
        [Tooltip(Key + "Client.OtherNPCShopQuotesTooltip")]
        [DefaultValue(true)]
        public bool OtherNPCShopQuotes { get; set; }

        [BackgroundColor(47, 29, 140, 180)]
        [Label(Key + "Client.InfoDebugLogsLabel")]
        [Tooltip(Key + "Client.InfoDebugLogsTooltip")]
        [DefaultValue(false)]
        public bool InfoDebugLogs { get; set; }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            AequusText.NewFromDict("Configuration.Client.ScreenshakeIntensity", "Label", (s) => AequusText.ItemText<Baguette>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.FlashIntensity", "Label", (s) => AequusText.ItemText<NoonPotion>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.HighQuality", "Label", (s) => AequusText.ItemText<Fluorescence>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.HighQualityShaders", "Label", (s) => AequusText.ItemText<FrozenTear>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.FlashShaderRepetitions", "Label", (s) => AequusText.ItemText<SupernovaFruit>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.NPCShopQuotes", "Label", (s) => AequusText.ItemText(ItemID.Teacup) + "  " + s);
            AequusText.NewFromDict("Configuration.Client.InfoDebugLogs", "Label", (s) => AequusText.ItemText(ItemID.DontStarveShaderItem) + "  " + s);
        }

        void ILoadable.Load(Mod mod)
        {
        }

        void ILoadable.Unload()
        {
        }
    }
}