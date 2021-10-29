using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AQMod.Common.PlayerData.Layers
{
    public class PostDrawWings : TempPlayerLayerWrapper
    {
        public override void Draw(PlayerDrawInfo info)
        {
            var mod = AQMod.Instance;
            var drawPlayer = info.drawPlayer.GetModPlayer<AQPlayer>();
            if (info.drawPlayer.wings > 0)
            {
                var wingPosition = info.drawPlayer.Center + new Vector2(0f, info.drawPlayer.gfxOffY - info.drawPlayer.mount.PlayerOffset / 2);
                var drawPosition = wingPosition - new Vector2(0f, info.drawPlayer.gravDir * 17f);
                var effects = info.drawPlayer.gravDir != 1f ? info.drawPlayer.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically : info.drawPlayer.direction != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                //int edgelorth = mod.GetEquipSlot("EdgelorthWings", EquipType.Wings);
                //if (info.drawPlayer.wings == edgelorth)
                //{
                //    var texture = DrawUtils.Textures.Extras[ExtraID.EdgelorthWings];
                //    var frame = new Rectangle(0, Main.wingsTexture[info.drawPlayer.wings].Height / 6 * info.drawPlayer.wingFrame, Main.wingsTexture[info.drawPlayer.wings].Width, Main.wingsTexture[info.drawPlayer.wings].Height / 6);
                //    var orig = new Vector2(Main.wingsTexture[info.drawPlayer.wings].Width / 2, Main.wingsTexture[info.drawPlayer.wings].Height / 12);

                //    drawPosition += new Vector2(-22f * info.drawPlayer.direction, 20f);

                //    var d = new DrawData(texture, new Vector2((int)(drawPosition.X - Main.screenPosition.X), (int)(drawPosition.Y - Main.screenPosition.Y)), frame,
                //        Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f)), info.drawPlayer.bodyRotation, orig,
                //        1f, effects, 0);

                //    Main.playerDrawData.Add(d);
                //}
            }
        }
    }
}