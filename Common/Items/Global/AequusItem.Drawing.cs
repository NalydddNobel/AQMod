using Aequus;
using Aequus.Common.Items.SlotDecals;
using Aequus.Common.UI;
using Aequus.Content.ItemPrefixes.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes {
        public byte armorPrefixAnimation;

        private float Rescale(Texture2D itemTexture, Texture2D newTexture, float scale) {
            float maxSide = Math.Max(itemTexture.Width, itemTexture.Height);
            float newMaxSide = Math.Max(newTexture.Width, newTexture.Height);
            return scale * (maxSide / newMaxSide);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (Main.playerInventory) {
                SlotDecals.DrawFullSlotDecals(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }
            else {
                if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.HotbarItem && HasCooldown.Contains(item.type)) {
                    PreDraw_Cooldowns(item, spriteBatch, position, frame, scale);
                }
            }

            if (item.type == ItemID.HelFire && NameTag == "Resonance") {
                var texture = AequusTextures.Resonance.Value;
                frame = texture.Frame();
                origin = frame.Size() / 2f;
                spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, Rescale(TextureAssets.Item[item.type].Value, texture, scale), SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        private void PostDraw_ArmorAnimation(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {

            if (armorPrefixAnimation <= 0) {
                return;
            }
            armorPrefixAnimation--;

            spriteBatch.Draw(
                AequusTextures.Bloom0,
                position,
                null,
                Color.Black,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                MathF.Pow(armorPrefixAnimation / 15f * Main.inventoryScale, 2f),
                SpriteEffects.None,
                0f
            );

            position += new Vector2(Main.rand.NextFloat(-armorPrefixAnimation, armorPrefixAnimation), Main.rand.NextFloat(-armorPrefixAnimation, armorPrefixAnimation)) * 0.65f * Main.inventoryScale;

            spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor * 0.66f, 0f, origin, scale, SpriteEffects.None, 0f);
        }
        private void PostDraw_PrefixPotions(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (item.prefix >= PrefixID.Count
                && PrefixLoader.GetPrefix(item.prefix) is PotionPrefixBase potionPrefix
                && potionPrefix.HasGlint) {
                var texture = TextureAssets.Item[item.type].Value;

                Main.spriteBatch.End();
                spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

                var drawData = new DrawData(
                    texture,
                    position,
                    frame,
                    (itemColor.A > 0 ? itemColor : Main.inventoryBack) with { A = 160 } * Helper.Wave(Main.GlobalTimeWrappedHourly, 0.66f, 1f), 0f,
                    origin,
                    scale, SpriteEffects.None, 0
                );

                var effect = GameShaders.Misc[potionPrefix.ShaderKey];
                effect.Apply(drawData);

                drawData.Draw(spriteBatch);

                Main.spriteBatch.End();
                spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
            }
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (Main.playerInventory) {
                PostDraw_ArmorAnimation(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }
            PostDraw_PrefixPotions(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            if (reversedGravity) {
                rotation = MathHelper.Pi - rotation;
            }
            return true;
        }
    }
}