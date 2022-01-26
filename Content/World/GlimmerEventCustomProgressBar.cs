using AQMod.Assets;
using AQMod.Content.World.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.World
{
    public class GlimmerEventCustomProgressBar : EventProgressBar
    {
        public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "glimmerevent");
        public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.GlimmerEvent");
        public override Color NameBGColor => new Color(120, 20, 110, 128);
        public override float EventProgress => 1f - (float)EventGlimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer) / EventGlimmer.MaxDistance;

        public override bool IsActive() => EventGlimmer.IsAbleToShowInvasionProgressBar();
        public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.EventProgress.GlimmerEvent", EventGlimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer));
    }
}
