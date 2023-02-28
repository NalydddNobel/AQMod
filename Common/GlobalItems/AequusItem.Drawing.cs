using Aequus;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory)
            {
                if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.EquipAccessory)
                {
                    PreDraw_CrownOfBlood(item, spriteBatch, position, frame, scale);
                }
            }
            else
            {
                if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.HotbarItem && HasWeaponCooldown.Contains(item.type))
                {
                    PreDraw_Cooldowns(item, spriteBatch, position, frame, scale);
                }
            }
            return true;
        }

        internal void PreDraw_CrownOfBlood(Item item, SpriteBatch sb, Vector2 position, Rectangle frame, float scale)
        {
            var aequus = Main.LocalPlayer.GetModPlayer<AequusPlayer>();
            if (aequus.accBloodCrownSlot > -1 && AequusUI.CurrentItemSlot.Slot == aequus.accBloodCrownSlot)
            {
                var backFrame = TextureAssets.InventoryBack16.Value.Frame();
                var drawPosition = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
                var color = new Color(150, 60, 60, 255);

                sb.Draw(TextureAssets.InventoryBack16.Value, drawPosition, backFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (reversedGravity)
            {
                rotation = MathHelper.Pi - rotation;
            }
            return true;
        }
    }
}