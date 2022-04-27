using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Potions;
using Aequus.Items.Weapons.Melee;
using Aequus.Localization;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Configuration
{
    public sealed class ClientConfiguration : ConfigurationBase
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static ClientConfiguration Instance;

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

        internal static void AddText()
        {
            ModTranslationHelpers.NewFromDict("Configuration.Client.ScreenshakeIntensity", "Label", (s) => AequusHelpers.ItemText<Baguette>() + "  " + s);
            ModTranslationHelpers.NewFromDict("Configuration.Client.FlashIntensity", "Label", (s) => AequusHelpers.ItemText<NoonPotion>() + "  " + s);
            ModTranslationHelpers.NewFromDict("Configuration.Client.HighQuality", "Label", (s) => AequusHelpers.ItemText<MirrorsCall>() + "  " + s);
        }
    }
}