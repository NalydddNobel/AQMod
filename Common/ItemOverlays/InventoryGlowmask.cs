using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.ItemOverlays
{
    /// <summary>
    /// Same as <see cref="LegacyGlowmask"/> but also applies the mask in the inventory
    /// </summary>
    public class InventoryGlowmask : LegacyGlowmask
    {
        public InventoryGlowmask(GlowID glowmask) : base(glowmask)
        {
        }

        public InventoryGlowmask(GlowID glowmask, Color drawColor) : base(glowmask, drawColor)
        {
        }

        public override void PostDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = DrawUtils.LegacyTextureCache.Glows[glowmask];
            Main.spriteBatch.Draw(texture, position, frame, this.drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}