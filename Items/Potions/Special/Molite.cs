using AQMod.NPCs.Friendly;
using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AQMod.Items.Potions.Special
{
    public class Molite : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (ConcoctionUI.Active && InvUI.Hooks.CurrentSlotContext == ItemSlot.Context.InventoryItem && Main.LocalPlayer.IsTalkingTo<Memorialist>())
            {
                var texture = mod.GetTexture("Items/ConcoctionBack");
                int frameY = texture.Height;
                var uiFrame = new Rectangle(0, texture.Height - frameY, texture.Width, frameY);
                position.Y += uiFrame.Y * Main.inventoryScale;
                var center = position + frame.Size() / 2f * scale;

                spriteBatch.Draw(texture, center, uiFrame, ConcoctionUI.CococtionItemBGColor, 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedShrimp, 5);
            r.AddRecipe();
        }
    }
}