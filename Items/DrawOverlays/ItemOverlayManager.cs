using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.DrawOverlays
{
    public class ItemOverlayManager : GlobalItem
    {
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return true;
            if (item.ModItem is IItemOverlaysDrawInventory itemOverlay &&
                itemOverlay.InventoryDraw.PreDrawInv(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale) == true)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return;
            if (item.ModItem is IItemOverlaysDrawInventory itemOverlay)
            {
                itemOverlay.InventoryDraw.PostDrawInv(Main.LocalPlayer, Main.LocalPlayer.GetModPlayer<AQPlayer>(), item, position, frame, drawColor, itemColor, origin, scale);
            }
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return true;
            if (item.ModItem is IItemOverlaysWorldDraw itemOverlay &&
                itemOverlay.WorldDraw.PreDrawWorld(item, lightColor, alphaColor, ref rotation, ref scale, whoAmI) == true)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return;
            if (item.ModItem is IItemOverlaysWorldDraw itemOverlay)
            {
                itemOverlay.WorldDraw.PostDrawWorld(item, lightColor, alphaColor, rotation, scale, whoAmI);
            }
        }
    }
}