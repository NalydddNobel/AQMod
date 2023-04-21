using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBlood : ModItem, ItemHooks.IUpdateItemDye {
        public const int AccessorySlot = 0;
        public const int ArmorSlot = Player.SupportedSlotsArmor + AccessorySlot;

        internal static float equipEffect;

        public override void Load() {
            LoadUnnecessaryTooltips();
        }

        public override void Unload() {
            UnnecessaryTooltips.Clear();
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
            Item.hasVanityEffects = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) {
            return slot > AccessorySlot;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accCrownOfBlood = Item;
            aequus.accCrownOfBloodItemClone = player.armor[ArmorSlot];
            player.AddBuff(aequus.CrownOfBloodHearts > 0 ? ModContent.BuffType<CrownOfBloodDebuff>() : ModContent.BuffType<CrownOfBloodBuff>(), 8);
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }

        private static bool CheckItemSlot(AequusUI.ItemSlotContext context) {
            return context.Context != ItemSlot.Context.EquipAccessory || context.Slot != ArmorSlot;
        }
        internal static void DrawBehindItem(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (!CheckItemSlot(AequusUI.CurrentItemSlot)) {
                return;
            }
        }
        internal static void DrawOverItem(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (!CheckItemSlot(AequusUI.CurrentItemSlot)) {
                return;
            }
        }
    }

    public class CrownOfBloodBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
            var item = Main.LocalPlayer.Aequus().accCrownOfBloodItemClone;
            if (item == null || item.IsAir || item.ToolTip == null || item.ToolTip.Lines <= 0 || !CrownOfBlood.ExtractTooltip(item, out string tooltip)) {
                tip = string.Format(tip, TextHelper.GetTextValue("Items.CrownOfBlood.NoItem"));
                return;
            }

            tip = string.Format(tip, tooltip);
        }

        public override bool RightClick(int buffIndex) {
            return false;
        }
    }

    public class CrownOfBloodDebuff : CrownOfBloodBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
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

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {

        public Item accCrownOfBlood;
        public Item accCrownOfBloodItemClone;

        private int crownOfBloodHearts;
        /// <summary>
        /// The amount of hearts consumed by the <see cref="CrownOfBlood"/>.
        /// </summary>
        public int CrownOfBloodHearts { get => crownOfBloodHearts; set => crownOfBloodHearts = Math.Clamp(value, 0, Player.TotalHearts() - 1); }
        public int crownOfBloodRegenTime;

        private void ResetCrownOfBlood() {
            accCrownOfBlood = null;
            accCrownOfBloodItemClone = null;
        }

        private void ClearCrownOfBlood() {
            accCrownOfBlood = null;
            accCrownOfBloodItemClone = null;
            crownOfBloodHearts = 0;
            crownOfBloodRegenTime = 0;
        }

        private void UpdateCrownOfBlood() {

            if (Main.myPlayer == Player.whoAmI) {
                if (accCrownOfBloodItemClone != null && !accCrownOfBloodItemClone.IsAir) {
                    CrownOfBlood.equipEffect = Math.Min(CrownOfBlood.equipEffect + 0.02f, 1f);
                }
                else {
                    CrownOfBlood.equipEffect = Math.Max(CrownOfBlood.equipEffect - 0.02f, 0f);
                }
            }

            if (crownOfBloodHearts <= 0) {
                crownOfBloodRegenTime = 0;
                return;
            }

            if (Main.myPlayer == Player.whoAmI) {
                CrownOfBloodHeartsOverlay.AnimationTimer += 2f + crownOfBloodHearts / 5f;
            }

            if (timeSinceLastHit > 300) {
                crownOfBloodRegenTime++;
                if (crownOfBloodRegenTime > 50) {
                    crownOfBloodRegenTime = 0;

                    crownOfBloodHearts--;
                    if (crownOfBloodHearts <= 0) {
                        return;
                    }
                }
            }
            else {
                crownOfBloodRegenTime = 0;
            }

            int hearts = Player.TotalHearts();
            int heartsLeft = Math.Max(hearts - crownOfBloodHearts, 1);
            Player.statLife = Math.Min(Player.statLife, Player.statLifeMax2 / hearts * heartsLeft);
        }

        private void InflictCrownOfBloodDownside(Player.HurtInfo hit) {
            if (accCrownOfBlood != null) {
                CrownOfBloodHearts += hit.Damage / Player.HealthPerHeart();
            }
        }
    }
}