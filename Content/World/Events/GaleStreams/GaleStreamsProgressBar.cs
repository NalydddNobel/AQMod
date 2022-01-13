using AQMod.Content.World.Events.ProgressBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events.GaleStreams
{
    public class GaleStreamsProgressBar : EventProgressBar
    {
        public override Texture2D IconTexture => ModContent.GetTexture("AQMod/Assets/UI/event_galestreams");
        public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.GaleStreams");
        public override Color NameBGColor => new Color(20, 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10 + MathHelper.Pi), 128);
        public override float EventProgress => (int)(Main.windSpeed * 100).Abs() / 300f;

        public override bool IsActive() => EventProgressBarLoader.ShouldShowGaleStreamsProgressBar && GaleStreams.EventActive(Main.LocalPlayer);
        public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.EventProgress.GaleStreams", (int)(Main.windSpeed * 100).Abs(), 300);
    }
}
