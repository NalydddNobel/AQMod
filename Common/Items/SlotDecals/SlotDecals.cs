using Aequus.Items.Accessories.CrownOfBlood;
using Terraria.GameContent;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ID;
using Aequus.Common.UI;

namespace Aequus.Common.Items.SlotDecals {
    public class SlotDecals : ModSystem {
        public static bool DrawSentryChipDecals;

        private static bool ValidSlotForSentryChipDecals(int context, int slot) {
            return context == ItemSlot.Context.EquipAccessory && slot >= 0 && slot > 2 && slot < 10;
        }

        public override void PostUpdatePlayers() {
            if (Main.netMode == NetmodeID.Server || Main.gameMenu) {
                DrawSentryChipDecals = false;
                return;
            }
            var player = Main.LocalPlayer;
            var aequusPlayer = player.Aequus();
            DrawSentryChipDecals = aequusPlayer.accSentryInheritence != null;
        }

        public static void DrawSlotDecal(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, Rectangle? frame, Color color) {
            spriteBatch.Draw(texture, center, frame, color,
                0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        public static void DrawEmptySlotDecals(SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor) {
            var center = position + TextureAssets.InventoryBack.Size() / 2f * Main.inventoryScale;
            CrownOfBloodItem.DrawSlot(spriteBatch, center);
            if (DrawSentryChipDecals && ValidSlotForSentryChipDecals(context, slot)) {
                DrawSlotDecal(spriteBatch, AequusTextures.InventoryDecal, center, null, Color.White);
            }
        }

        public static void DrawFullSlotDecals(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            CrownOfBloodItem.DrawSlot(spriteBatch, position);
            CrownOfBloodItem.DrawItem(item, spriteBatch, position, frame, origin, scale);
            if (DrawSentryChipDecals && ValidSlotForSentryChipDecals(AequusUI.CurrentItemSlot.Context, AequusUI.CurrentItemSlot.Slot)) {
                DrawSlotDecal(spriteBatch, AequusTextures.InventoryDecal, position, null, Color.White);
            }
        }
    }
}