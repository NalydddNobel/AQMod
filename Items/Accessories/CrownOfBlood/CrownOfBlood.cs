using Aequus.Common.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Aequus.Items.Accessories.CrownOfBlood;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public class CrownOfBlood : ModItem, ItemHooks.IUpdateItemDye {
        /// <summary>
        /// Default Value: 1
        /// </summary>
        public static int AddedStacks = 1;

        public override void SetDefaults() {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {

            var aequus = player.Aequus();
            var equipModifiers = aequus.equipModifiers;
            var parameters = EquipEmpowermentParameters.Defense | EquipEmpowermentParameters.Abilities;
            var equip = equipModifiers.FirstUnempoweredAccessory(parameters);
            equip.type |= parameters;
            equip.addedStacks += AddedStacks;
            equip.bonusColor = EquipEmpowermentManager.CrownOfBloodEmpowermentColor;
            equip.slotColor = new(150, 60, 60, 255);
            aequus.accCrownOfBlood = Item;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            if (!isSetToHidden || !isNotInVanitySlot) {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }
    }

    public class CrownOfBloodHeartsOverlay : ModResourceOverlay {
        public static float AnimationTimer;

        public override void PostDrawResource(ResourceOverlayDrawContext context) {
            var player = Main.LocalPlayer;
            var aequus = player.Aequus();
            int totalHearts = player.TotalHearts();
            int corruptedHearts = aequus.CrownOfBloodHearts;
            if (context.resourceNumber < totalHearts - corruptedHearts) {
                return;
            }
            string name = context.texture.Name;
            if (!name.Contains("Heart_Fill")) {
                return;
            }
            float heartFade = 1f;
            var frame = AequusTextures.CrownOfBlood_Hearts.Frame(verticalFrames: 2);
            if (context.resourceNumber == totalHearts - corruptedHearts) {
                if (aequus.crownOfBloodRegen > 15) {
                    heartFade = 1f - (aequus.crownOfBloodRegen - 15f) / 35f;

                    Main.spriteBatch.Draw(
                        AequusTextures.CrownOfBlood_Hearts,
                        context.position,
                        frame,
                        Color.Red with { A = 0 } * heartFade,
                        0f,
                        context.origin,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }

            Main.spriteBatch.Draw(
                AequusTextures.Bloom0,
                context.position + new Vector2(0f, -2f),
                null,
                Color.Black,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                heartFade * 0.4f,
                SpriteEffects.None,
                0f
            );

            Main.spriteBatch.Draw(
                AequusTextures.CrownOfBlood_Hearts,
                context.position,
                frame,
                Color.White,
                0f,
                context.origin,
                heartFade,
                SpriteEffects.None,
                0f
            );

            float wave = MathF.Sin(AnimationTimer / 60f + context.resourceNumber * 0.6f);
            float scale = 0.95f + 0.3f * wave;
            var shake = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * wave * 2f;
            Main.spriteBatch.Draw(
                AequusTextures.CrownOfBlood_Hearts,
                context.position + shake,
                frame,
                Color.White * wave,
                0f,
                context.origin,
                heartFade * scale,
                SpriteEffects.None,
                0f
            );
            Main.spriteBatch.Draw(
                AequusTextures.CrownOfBlood_Hearts,
                context.position + shake,
                frame.Frame(0, 1),
                Color.Red with { A = 0 } * wave,
                0f,
                context.origin,
                heartFade * scale,
                SpriteEffects.None,
                0f
            );

            if (heartFade < 1f) {
                var flarePosition = context.position + new Vector2(0f, -2f);
                float flareEffect = (1f - heartFade);
                float flareFade = 1f - MathF.Pow(flareEffect, 2f);
                Main.spriteBatch.Draw(
                    AequusTextures.Bloom0,
                    flarePosition,
                    null,
                    Color.Red * flareFade,
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
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {

        public Item accCrownOfBlood;
        private int crownOfBloodHearts;
        /// <summary>
        /// The amount of hearts consumed by the <see cref="CrownOfBlood"/>.
        /// </summary>
        public int CrownOfBloodHearts { get => crownOfBloodHearts; set => crownOfBloodHearts = Math.Clamp(value, 0, Player.TotalHearts() - 1); }
        public int crownOfBloodRegen;

        private void ClearCrownOfBlood() {
            accCrownOfBlood = null;
            crownOfBloodHearts = 0;
            crownOfBloodRegen = 0;
        }

        private void UpdateCrownOfBloodHearts() {
            
            if (crownOfBloodHearts <= 0) {
                crownOfBloodRegen = 0;
                return;
            }

            if (Main.myPlayer == Player.whoAmI) {
                CrownOfBloodHeartsOverlay.AnimationTimer += 2f + crownOfBloodHearts / 5f;
            }

            if (timeSinceLastHit > 300) {
                crownOfBloodRegen++;
                if (crownOfBloodRegen > 50) {
                    crownOfBloodRegen = 0;

                    crownOfBloodHearts--;
                    if (crownOfBloodHearts <= 0) {
                        return;
                    }
                }
            }
            else {
                crownOfBloodRegen = 0;
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