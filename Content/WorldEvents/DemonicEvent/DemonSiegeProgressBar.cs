using AQMod.Content.WorldEvents.ProgressBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.DemonicEvent
{
    public class DemonSiegeProgressBar : EventProgressBar
    {
        public override Texture2D IconTexture => ModContent.GetTexture("AQMod/Assets/Textures/EventIcon_DemonSiege");
        public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.DemonSiege");
        public override Color NameBGColor => new Color(120, 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 20, 128);
        public override float EventProgress => 0f;

        public override bool IsActive() => DemonSiege.CloseEnoughToDemonSiege(Main.LocalPlayer);
        public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.Common.TimeLeft", AQUtils.TimeText3(DemonSiege.UpgradeTime));
    }
}
