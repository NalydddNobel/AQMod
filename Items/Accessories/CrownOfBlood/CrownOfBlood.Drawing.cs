using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

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

            spriteBatch.Draw(AequusTextures.InventoryBack_CrownOfBlood, position, null, Color.White * opacity,
                0f, AequusTextures.InventoryBack_CrownOfBlood.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);

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

    public class CrownOfBloodHeartsOverlay : ModResourceOverlay {
        public static float AnimationTimer;

        public static readonly Rectangle ClassicHeartFrame = new(0, 0, 22, 22);
        public static readonly Rectangle ClassicHeartFrame_Glow = new(0, 24, 22, 22);
        public static readonly Rectangle ClassicHeartFrame_Cracks = new(0, 48, 22, 22);
        public static readonly Rectangle FancyHeartFrame = new(24, 0, 22, 22);
        public static readonly Rectangle FancyHeartFrame_Glow = new(24, 24, 22, 22);
        public static readonly Rectangle FancyHeartFrame_Cracks = new(24, 48, 22, 22);
        public static readonly Rectangle BarFrame = new(48, 0, 12, 12);
        public static readonly Rectangle BarFrame_Cracks = new(62, 0, 12, 12);

        public static Texture2D GetHeartTexture(ResourceOverlayDrawContext context, Player player, int resourceNumber) {
            if (player.ConsumedLifeFruit > resourceNumber) {
                return AequusTextures.Heart2;
            }
            return AequusTextures.Heart;
        }

        private float GetHeartFade(AequusPlayer aequus, int totalHearts, int corruptedHearts, int resourceNumber) {
            return resourceNumber == totalHearts - corruptedHearts && aequus.crownOfBloodRegenTime > 15
                ? 1f - (aequus.crownOfBloodRegenTime - 15f) / 35f
                : 1f;
        }

        private void DrawCorruptedHealthPiece(Texture2D heart, Vector2 position, Rectangle heartFrame, Rectangle glowFrame, Rectangle cracksFrame, Vector2 origin, float heartFade, int resourceNumber) {

            if (heartFade < 1f) {
                Main.spriteBatch.Draw(heart, position, glowFrame,
                    Color.White with { A = 0 } * heartFade, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(heart, position, heartFrame,
                Color.White * heartFade, 0f, origin, heartFade, SpriteEffects.None, 0f);

            float wave = MathF.Sin(AnimationTimer / 60f + resourceNumber * 0.6f);
            if (wave > 0f) {
                float scale = 0.95f + 0.3f * wave;
                var shake = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * wave * 2f;

                Main.spriteBatch.Draw(heart, position + shake, heartFrame,
                    Color.White * wave * heartFade, 0f, origin, heartFade * scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(heart, position + shake, cracksFrame,
                    Color.White with { A = 0 } * wave * heartFade, 0f, origin, heartFade * scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawHeartFlare(float heartFade, Vector2 position) {
            var flarePosition = position;
            float flareEffect = (1f - heartFade);
            float flareFade = 1f - MathF.Pow(flareEffect, 2f);
            Main.spriteBatch.Draw(
                AequusTextures.Bloom0,
                flarePosition,
                null,
                Color.Red with { A = 0 } * flareFade,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                flareEffect * 0.6f,
                SpriteEffects.None,
                0f
            );
            Main.spriteBatch.Draw(
                AequusTextures.Flare,
                flarePosition,
                null,
                Color.Red with { A = 0 } * flareFade,
                0f,
                AequusTextures.Flare.Size() / 2f,
                new Vector2(0.8f, 1f) * flareEffect,
                SpriteEffects.None,
                0f
            );
            Main.spriteBatch.Draw(
                AequusTextures.Flare,
                flarePosition,
                null,
                Color.Red with { A = 0 } * flareFade,
                MathHelper.PiOver2,
                AequusTextures.Flare.Size() / 2f,
                flareEffect * 0.8f,
                SpriteEffects.None,
                0f
            );
        }

        private void DrawFancyHeart(ResourceOverlayDrawContext context, Player player, AequusPlayer aequus, int totalHearts, int corruptedHearts) {
            float heartFade = GetHeartFade(aequus, totalHearts, corruptedHearts, context.resourceNumber);

            Main.spriteBatch.Draw(AequusTextures.Bloom0, context.position + new Vector2(0f, -2f), null,
                Color.Black * heartFade, 0f, AequusTextures.Bloom0.Size() / 2, 0.4f, SpriteEffects.None, 0f);

            DrawCorruptedHealthPiece(GetHeartTexture(context, player, context.resourceNumber),
                context.position, FancyHeartFrame, FancyHeartFrame_Glow, FancyHeartFrame_Cracks, context.origin, heartFade, context.resourceNumber);

            if (heartFade < 1f) {
                DrawHeartFlare(heartFade, context.position + new Vector2(0f, -2f));
            }
        }

        private void DrawClassicHeart(ResourceOverlayDrawContext context, Player player, AequusPlayer aequus, int totalHearts, int corruptedHearts) {
            float heartFade = GetHeartFade(aequus, totalHearts, corruptedHearts, context.resourceNumber);

            Main.spriteBatch.Draw(AequusTextures.Bloom0, context.position + new Vector2(0f, -2f), null,
                Color.Black * heartFade, 0f, AequusTextures.Bloom0.Size() / 2, 0.4f, SpriteEffects.None, 0f);

            DrawCorruptedHealthPiece(GetHeartTexture(context, player, context.resourceNumber),
                context.position, ClassicHeartFrame, ClassicHeartFrame_Glow, ClassicHeartFrame_Cracks, context.origin, heartFade, context.resourceNumber);

            if (heartFade < 1f) {
                DrawHeartFlare(heartFade, context.position + new Vector2(0f, -2f));
            }
        }

        private void DrawBarHeart(ResourceOverlayDrawContext context, Player player, AequusPlayer aequus, int totalHearts, int corruptedHearts) {
            float heartFade = GetHeartFade(aequus, totalHearts, corruptedHearts, context.resourceNumber);

            var y = (context.resourceNumber % 2) * 14;
            var heart = GetHeartTexture(context, player, context.resourceNumber);
            var position = context.position + new Vector2(-12f, 0f);
            var heartFrame = BarFrame with { Y = y };
            var cracksFrame = BarFrame_Cracks with { Y = y };
            var origin = context.origin;
            int resourceNumber = context.resourceNumber;
            if (heartFade < 1f) {
                Main.spriteBatch.Draw(heart, position, heartFrame,
                    Color.White with { A = 0 } * heartFade, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(heart, position, heartFrame,
                Color.White * heartFade, 0f, origin, 1f, SpriteEffects.None, 0f);

            float wave = MathF.Sin(AnimationTimer / 60f + resourceNumber * 0.6f);
            if (wave > 0f) {
                Main.spriteBatch.Draw(heart, position, cracksFrame,
                    Color.White with { A = 0 } * wave * heartFade, 0f, origin, 1f, SpriteEffects.None, 0f);
            }

            if (heartFade < 1f) {
                DrawHeartFlare(heartFade, context.position + new Vector2(-6f, 6f));
            }
        }

        public override void PostDrawResource(ResourceOverlayDrawContext context) {
            var player = Main.LocalPlayer;
            var aequus = player.Aequus();
            int totalHearts = player.TotalHearts();
            int corruptedHearts = aequus.CrownOfBloodHearts;
            switch (context.DisplaySet) {
                case FancyClassicPlayerResourcesDisplaySet:
                    if (context.resourceNumber < totalHearts - corruptedHearts || !context.texture.Name.Contains("Heart_Fill")) {
                        break;
                    }

                    DrawFancyHeart(context, player, aequus, totalHearts, corruptedHearts);
                    break;

                case ClassicPlayerResourcesDisplaySet:
                    if (context.resourceNumber < totalHearts - corruptedHearts || !context.texture.Name.Contains("Heart")) {
                        break;
                    }

                    DrawClassicHeart(context, player, aequus, totalHearts, corruptedHearts);
                    break;

                case HorizontalBarsPlayerResourcesDisplaySet:
                    if (context.resourceNumber < 0 || context.resourceNumber < totalHearts - corruptedHearts || !context.texture.Name.Contains("HP_Fill")) {
                        break;
                    }

                    DrawBarHeart(context, player, aequus, totalHearts, corruptedHearts);
                    break;
            }
        }
    }
}