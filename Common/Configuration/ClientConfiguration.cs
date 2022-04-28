using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Potions;
using Aequus.Items.Weapons.Melee;
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

        internal static void OnModLoad(Aequus aequus)
        {
            AequusText.NewFromDict("Configuration.Client.ScreenshakeIntensity", "Label", (s) => AequusText.ItemText<Baguette>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.FlashIntensity", "Label", (s) => AequusText.ItemText<NoonPotion>() + "  " + s);
            AequusText.NewFromDict("Configuration.Client.HighQuality", "Label", (s) => AequusText.ItemText<MirrorsCall>() + "  " + s);
        }
    }
}