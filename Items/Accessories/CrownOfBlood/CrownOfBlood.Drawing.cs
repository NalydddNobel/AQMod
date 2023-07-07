using Aequus.Common.Items.SlotDecals;
using Aequus.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBloodItem {
        private static float _equipAnimation;
        private static bool _equipSound;

        internal static void UpdateEquipEffect(bool active, Item clonedItem) {
            if (_equipAnimation < 0.4f) {
                _equipSound = false;
            }
            else if (!_equipSound) {
                _equipSound = true;
                if (Main.playerInventory) {
                }
            }

            if (!active) {
                _equipAnimation = Math.Max(_equipAnimation - 0.01f, 0f);
                return;
            }

            if (!clonedItem.IsAir || _equipAnimation < 0.25f) {
                _equipAnimation = Math.Min(_equipAnimation + 0.01f, 1f);
            }
            else {
                _equipAnimation = Math.Max(_equipAnimation - 0.01f, 0.25f);
            }
        }

        public static void DrawSlotFull(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle itemFrame, Vector2 itemOrigin, float itemScale) {
            DrawSlot(spriteBatch, position);
            DrawItem(item, spriteBatch, position, itemFrame, itemOrigin, itemScale);
        }

        public static void DrawSlot(SpriteBatch spriteBatch, Vector2 position) {
            if (CheckItemSlot(AequusUI.CurrentItemSlot)) {
                return;
            }

            float opacity = MathF.Pow(Math.Min(_equipAnimation / 0.25f, 1f), 2f);
            var bloom = AequusTextures.Bloom0;
            var bloomOrigin = bloom.Size() / 2f;
            spriteBatch.Draw(bloom, position, null, Color.Red with { A = 100 } * opacity,
                0f, bloomOrigin, 0.75f * Main.inventoryScale, SpriteEffects.None, 0f);

            SlotDecals.DrawSlotDecal(spriteBatch, AequusTextures.InventoryBack_CrownOfBlood, position, null, Color.White * opacity);

            spriteBatch.Draw(bloom, position, null, Color.Black * opacity,
                0f, bloomOrigin, 0.5f * Main.inventoryScale, SpriteEffects.None, 0f);

            if (_equipAnimation > 0.45f && _equipAnimation < 0.95f) {
                float animation = Math.Min((_equipAnimation - 0.45f) / 0.5f, 1f);
                var flare = AequusTextures.Flare.Value;
                var flareColor = new Color(255, 60, 60, 0) * MathF.Sin(animation * MathHelper.Pi) * 0.5f;
                var flareOrigin = flare.Size() / 2f;
                var flareScale = new Vector2(0.5f, 1f) * Main.inventoryScale * (1f - animation * 0.3f);
                float flareDistance = MathF.Pow(1f - animation, 1.5f) * 40f;
                for (int i = 0; i < 8; i++) {
                    float rotation = (i * MathHelper.PiOver4 + Main.GlobalTimeWrappedHourly * 2.5f);
                    var v = rotation.ToRotationVector2() * flareDistance;
                    spriteBatch.Draw(flare, position + v, null, flareColor,
                        rotation + MathHelper.PiOver2, flareOrigin, flareScale, SpriteEffects.None, 0f);
                }
            }
        }

        public static void DrawItem(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Vector2 origin, float scale) {
            if (CheckItemSlot(AequusUI.CurrentItemSlot)) {
                return;
            }

            float itemOpacity = MathF.Pow(_equipAnimation, 3f);
            float distance = (4f + 14f * (1f - _equipAnimation) * Main.inventoryScale);
            var itemAuraColor = new Color(255, 60, 60, 0) * itemOpacity * 0.6f;
            for (int i = 0; i < 4; i++) {
                var v = (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 2.5f).ToRotationVector2() * distance;
                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position + v, frame, itemAuraColor,
                    0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}