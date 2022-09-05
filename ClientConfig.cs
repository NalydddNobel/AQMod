using Aequus.Items.Boss.Summons;
using Aequus.Items.Consumables.BuffPotions;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc;
using Aequus.Items.Weapons.Summon.Necro;
using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace Aequus
{
    public sealed class ClientConfig : ConfigurationBase
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

        [Header(Key + "Client.General.Header")]

        [MemberBGColor]
        [Name("Client.General.NPCShopQuotes")]
        [Desc("Client.General.NPCShopQuotes")]
        [DefaultValue(true)]
        public bool NPCShopQuotes { get; set; }

        [MemberBGColor_Secondary]
        [Name("Client.General.OtherNPCShopQuotes")]
        [Desc("Client.General.OtherNPCShopQuotes")]
        [DefaultValue(true)]
        public bool OtherNPCShopQuotes { get; set; }

        [MemberBGColor]
        [Name("Client.General.InfoDebugLogs")]
        [Desc("Client.General.InfoDebugLogs")]
        [DefaultValue(false)]
        public bool InfoDebugLogs { get; set; }

        public override void AddCustomTranslations()
        {
            Text("Visuals.ScreenshakeIntensity", new
            {
                Baguette = AequusText.ItemText<Baguette>(),
            });
            Text("Visuals.FlashIntensity", new
            {
                NoonPotion = AequusText.ItemText<NoonPotion>(),
            });
            Text("Visuals.HighQuality", new
            {
                Fluorescence = AequusText.ItemText<Fluorescence>(),
            });
            Text("Visuals.HighQualityShaders", new
            {
                FrozenTear = AequusText.ItemText<FrozenTear>(),
            });
            Text("Visuals.FlashShaderRepetitions", new
            {
                SupernovaFruit = AequusText.ItemText<SupernovaFruit>(),
            });
            Text("Visuals.NecromancyOutlines", new
            {
                Insurgency = AequusText.ItemText<Insurgency>(),
            });
            Text("General.NPCShopQuotes", new
            {
                Teacup = AequusText.ItemText(ItemID.Teacup),
            });
            Text("General.OtherNPCShopQuotes", new
            {
                CoffeeCup = AequusText.ItemText(ItemID.CoffeeCup),
            });
            Text("General.InfoDebugLogs", new
            {
                RadioThing = AequusText.ItemText(ItemID.DontStarveShaderItem),
            });
        }
    }
}