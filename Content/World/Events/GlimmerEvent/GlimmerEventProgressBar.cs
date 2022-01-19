using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events.GlimmerEvent
{
    public class GlimmerEventProgressBar : EventProgressBar
    {
        public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "glimmerevent");
        public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.GlimmerEvent");
        public override Color NameBGColor => new Color(120, 20, 110, 128);
        public override float EventProgress => 1f - (float)GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer) / GlimmerEvent.MaxDistance;

        public override bool IsActive() => GlimmerEvent.IsAbleToShowInvasionProgressBar();
        public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.EventProgress.GlimmerEvent", GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer));
    }
}
