using Aequus.Items.Accessories.Summon.Sentry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public class MechsSentryAccessorySlot : ModAccessorySlot
    {
        private static Color InventoryBackHack;

        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return checkItem.ModItem is not MechsSentry && base.CanAcceptItem(checkItem, context);
        }

        public override bool IsEnabled()
        {
            return Player.Aequus().accSentrySlot;
        }

        public override bool PreDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered)
        {
            var color = new Color(148, 62, 86).HueAdd(((int)context - 10) * -0.025f);
            Main.spriteBatch.Draw(TextureAssets.InventoryBack13.Value, position, null, color * (Main.inventoryBack.A / 255f), 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
            InventoryBackHack = Main.inventoryBack;
            Main.inventoryBack = Color.Transparent;
            return true;
        }

        public override void PostDraw(AccessorySlotType context, Item item, Vector2 position, bool isHovered)
        {
            Main.inventoryBack = InventoryBackHack;
        }

        public override void ApplyEquipEffects()
        {
            int proj = Player.Aequus().projectileIdentity;
            if (proj != -1)
            {
                int projWhoAmI = AequusHelpers.FindProjectileIdentity(Player.whoAmI, proj);
                if (projWhoAmI != -1 && Main.projectile[projWhoAmI].sentry)
                {
                    base.ApplyEquipEffects();
                }
            }
        }
    }
}