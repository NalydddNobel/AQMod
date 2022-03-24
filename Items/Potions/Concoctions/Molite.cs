using AQMod.NPCs.Friendly;
using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AQMod.Items.Potions.Concoctions
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
                AQUtils.DrawUIBack(spriteBatch, mod.GetTexture("Items/ConcoctionBack"), position, frame, scale, ConcoctionUI.CococtionItemBGColor);
            }
            return true;
        }
    }
}