using Aequus.Items.Boss.Summons;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Materials;
using Aequus.Items.Potions;
using Aequus.Items.Weapons.Summon.Scepters;
using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Preferences
{
    public class ClientConfig : ConfigurationBase
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        protected override string ConfigKey => "Client";

        public static ClientConfig Instance;

        [Header(Key + "Client.Visuals.Header")]

        [MemberBGColor]
        [Name("Client.Visuals.ScreenshakeIntensity")]
        [Desc("Client.Visuals.ScreenshakeIntensity")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float ScreenshakeIntensity { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.FlashIntensity")]
        [Desc("Client.Visuals.FlashIntensity")]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [SliderColor(120, 40, 255, 255)]
        public float FlashIntensity { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.HighQuality")]
        [Desc("Client.Visuals.HighQuality")]
        [DefaultValue(true)]
        public bool HighQuality { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.HighQualityShaders")]
        [Desc("Client.Visuals.HighQualityShaders")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool HighQualityShaders { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.FlashShaderRepetitions")]
        [Desc("Client.Visuals.FlashShaderRepetitions")]
        [Increment(4)]
        [DefaultValue(40)]
        [Range(10, 80)]
        [Slider()]
        [SliderColor(30, 50, 120, 255)]
        public int FlashShaderRepetitions { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.NecromancyOutlines")]
        [Desc("Client.Visuals.NecromancyOutlines")]
        [DefaultValue(true)]
        public bool NecromancyOutlines { get; set; }

        [MemberBGColor]
        [Name("Client.Visuals.AdamantiteChestMimic")]
        [Desc("Client.Visuals.AdamantiteChestMimic")]
        [DefaultValue(true)]
        public bool AdamantiteChestMimic { get; set; }

        [Header(Key + "Client.General.Header")]

        [MemberBGColor]
        [Name("Client.General.InfoDebugLogs")]
        [Desc("Client.General.InfoDebugLogs")]
        [DefaultValue(false)]
        public bool InfoDebugLogs { get; set; }

        public override void AddCustomTranslations()
        {
            Text("Visuals.AdamantiteChestMimic");
            Text("Visuals.ScreenshakeIntensity", new
            {
                Baguette = TextHelper.ItemCommand<Baguette>(),
            });
            Text("Visuals.FlashIntensity", new
            {
                NoonPotion = TextHelper.ItemCommand<NoonPotion>(),
            });
            Text("Visuals.HighQuality", new
            {
                Fluorescence = TextHelper.ItemCommand<Fluorescence>(),
            });
            Text("Visuals.HighQualityShaders", new
            {
                FrozenTear = TextHelper.ItemCommand<FrozenTear>(),
            });
            Text("Visuals.FlashShaderRepetitions", new
            {
                SupernovaFruit = TextHelper.ItemCommand<SupernovaFruit>(),
            });
            Text("Visuals.NecromancyOutlines", new
            {
                Insurgency = TextHelper.ItemCommand<Insurgency>(),
            });
            Text("General.InfoDebugLogs", new
            {
                RadioThing = TextHelper.ItemCommand(ItemID.DontStarveShaderItem),
            });
        }
    }
}