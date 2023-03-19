﻿using Aequus.Common.Recipes;
using Aequus.Common.Utilities;
using Aequus.Items.Accessories.Misc;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories.Misc {
    public class FaultyCoin : ModItem {

        public const long MoneyAmount = Item.platinum;
        public float removeFailAnimation;

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accFaultyCoinLoan = Math.Max(MoneyAmount, aequus.accFaultyCoinLoan);
            aequus.accFaultyCoinItem = Item;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) {
            return player.Aequus().accFaultyCoinDebt <= 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            var player = Main.LocalPlayer;
            var aequus = player.Aequus();
            if (Main.mouseRight && Main.mouseRightRelease && player.CanBuyItem((int)aequus.accFaultyCoinDebt)) {
                OnRemoveAccessory(player);
            }

            long tooltipDebt = Math.Max(aequus.accFaultyCoinDebt, MoneyAmount);
            string colorText = TextHelper.ColorCommandStart(Colors.CoinPlatinum, alphaPulse: true);

            foreach (var t in tooltips) {
                if (t.Mod != "Terraria") {
                    continue;
                }

                t.Text = t.Text.Replace("[[", colorText);
                t.Text = t.Text.Replace("]]", "]");
            }

            tooltips.Insert(
                tooltips.GetIndex("EtherianManaWarning"),
                new(Mod, "FaultyCoinDebt", $"In order to unequip this accessory, you must pay {TextHelper.PriceTextColored(tooltipDebt, alphaPulse: true)}")
            );
        }

        public override void AddRecipes() {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<FoolsGoldRing>());
        }

        public void OnRemoveAccessory(Player player) {
            player.BuyItem((int)player.Aequus().accFaultyCoinDebt);
            player.Aequus().accFaultyCoinDebt = 0;
            SoundEngine.PlaySound(SoundID.Coins);
            //Main.NewText(Environment.StackTrace);
        }

        public void OnUnsuccessfulRemove(Player player) {
            SoundEngine.PlaySound(AequusSounds.coinHit);
            removeFailAnimation = 30f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {

            float rotation = MathF.Sin(removeFailAnimation / 2f) * removeFailAnimation / 60f;
            var center = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);

            if (removeFailAnimation > 0f) {
                float progress = removeFailAnimation / 30f;

                spriteBatch.Draw(
                    AequusTextures.Bloom0,
                    center,
                    null,
                    Color.Black * progress,
                    0f,
                    AequusTextures.Bloom0.Size() / 2f,
                    Main.inventoryScale * 0.6f * progress, SpriteEffects.None, 0f
                );

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var texture = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var bulletOrigin = texture.Size() / 2f;
                ulong seed = (ulong)Main.LocalPlayer.name.GetHashCode();
                float powProgress = MathF.Pow(progress, 2f);
                float bulletOpacity = 1f;
                if (removeFailAnimation > 15f) {
                    bulletOpacity = 1f - (removeFailAnimation - 15f) / 15f;
                }
                foreach (var r in Helper.Circular(10)) {

                    float randomFloat = Utils.RandomFloat(ref seed);

                    float bulletRotation = r + (randomFloat * 0.2f - 0.1f);
                    float bulletProgress = powProgress * Helper.Wave(Main.GlobalTimeWrappedHourly * Math.Max(randomFloat, 0.5f) * 3f + randomFloat * 10f, 1f, 1.2f);
                    var v = bulletRotation.ToRotationVector2();
                    spriteBatch.Draw(
                        texture,
                        center + (v * bulletProgress * 40f * Main.inventoryScale),
                        null,
                        Color.Black * bulletProgress * bulletOpacity,
                        bulletRotation + MathHelper.PiOver2,
                        bulletOrigin,
                        new Vector2(1f, 2f) * Main.inventoryScale * 0.2f, SpriteEffects.None, 0f
                    );
                }
            }

            spriteBatch.Draw(
                TextureAssets.Item[Type].Value,
                center,
                frame,
                drawColor,
                rotation,
                frame.Size() / 2f,
                scale, SpriteEffects.None, 0f
            );

            removeFailAnimation *= 0.9f;
            if (removeFailAnimation < 0.01f) {
                removeFailAnimation = 0f;
            }
            return false;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public long accFaultyCoinLoan;
        [SaveData("Debt")]
        public long accFaultyCoinDebt;
        public Item accFaultyCoinItem;

        public void ResetEffects_FaultyCoin() {
            accFaultyCoinLoan = 0;
            accFaultyCoinItem = null;
        }

        public void PostUpdate_FaultyCoin() {

            if (accFaultyCoinItem == null) {
                return;
            }

            long loan = accFaultyCoinLoan - accFaultyCoinDebt;
            if (loan > 0) {
                Helper.DropMoney(Player.GetSource_Accessory(accFaultyCoinItem), Player.Hitbox, loan);
                accFaultyCoinDebt = accFaultyCoinLoan;
            }
        }

        private static bool ItemSlot_OverrideLeftClick_FaultyCoin(Item[] inv, int context, int slot) {

            if (inv[slot].IsAir || inv[slot].ModItem is not FaultyCoin faultyCoin) {
                return false;
            }

            if (Math.Abs(context) == ItemSlot.Context.EquipAccessory) {
                long amount = Main.LocalPlayer.Aequus().accFaultyCoinDebt;
                if (!Main.LocalPlayer.CanBuyItem((int)amount)) {
                    faultyCoin.OnUnsuccessfulRemove(Main.LocalPlayer);
                    return true;
                }
                faultyCoin.OnRemoveAccessory(Main.LocalPlayer);
            }

            return false;
        }
    }
}