using Aequus.Buffs.Misc;
using Aequus.Common.Recipes;
using Aequus.Common.Utilities;
using Aequus.Items.Accessories.Misc;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items.Accessories.Misc {
    public class FaultyCoin : ModItem {

        /// <summary>
        /// Default Value: <see cref="Item.platinum"/> (1000000) (1 Platinum)
        /// </summary>
        public static long MoneyAmount = Item.platinum;
        public float removeFailAnimation;

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.increasedEnemyMoney += 0.05f;
            aequus.accFaultyCoinLoan = Math.Max(MoneyAmount, aequus.accFaultyCoinLoan);
            aequus.accFaultyCoinItem = Item;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) {
            return player.Aequus().accFaultyCoinDebt <= 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            var player = Main.LocalPlayer;
            var aequus = player.Aequus();
            if (Main.mouseRight && Main.mouseRightRelease && player.CanAfford((int)aequus.accFaultyCoinDebt)) {
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
                tooltips.GetIndex("Material") + 1,
                new(Mod, "FaultyCoinDebt", TextHelper.GetTextValueWith("Items.FaultyCoin.Debt", new { Debt = TextHelper.PriceTextColored(tooltipDebt, alphaPulse: true) }))
            );
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<FoolsGoldRing>());
        }

        public void OnRemoveAccessory(Player player) {
            player.BuyItem((int)player.Aequus().accFaultyCoinDebt);
            player.Aequus().accFaultyCoinDebt = 0;
            SoundEngine.PlaySound(SoundID.Coins);
        }

        public void OnUnsuccessfulRemove(Player player) {
            SoundEngine.PlaySound(AequusSounds.coinHit);
            removeFailAnimation = 30f;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {

            float rotation = MathF.Sin(removeFailAnimation / 2f) * removeFailAnimation / 60f;

            if (removeFailAnimation > 0f) {
                float progress = removeFailAnimation / 30f;

                spriteBatch.Draw(
                    AequusTextures.Bloom0,
                    position,
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
                        position + (v * bulletProgress * 40f * Main.inventoryScale),
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
                position,
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

namespace Aequus.Buffs.Misc {
    public class FaultyCoinBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override bool RightClick(int buffIndex) {
            return false;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public void ProcFaultyCoin(NPC npc, AequusNPC aequus, Player player, AequusPlayer aequusPlayer) {

            if (npc.value <= 0f || !player.HasBuff<FaultyCoinBuff>()) {
                return;
            }

            npc.value *= 2f;
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public long accFaultyCoinLoan;
        [SaveData("Debt")]
        public long accFaultyCoinDebt;
        public Item accFaultyCoinItem;
        private int _faultyCoinCheck;

        private void ResetEffects_FaultyCoin() {
            accFaultyCoinLoan = 0;
            accFaultyCoinItem = null;
        }

        private void PostUpdate_FaultyCoin() {

            if (accFaultyCoinItem == null) {
                _faultyCoinCheck = 0;
                return;
            }

            _faultyCoinCheck++;
            long loan = accFaultyCoinLoan - accFaultyCoinDebt;
            if (loan > 0) {
                Helper.DropMoney(Player.GetSource_Accessory(accFaultyCoinItem), Player.Hitbox, loan);
                accFaultyCoinDebt = accFaultyCoinLoan;
            }

            if (_faultyCoinCheck >= 90) {
                if (!Player.CanAfford((int)accFaultyCoinDebt)) {
                    Player.AddBuff(ModContent.BuffType<FaultyCoinBuff>(), 120);
                }
                _faultyCoinCheck = 0;
            }
        }

        private static bool ItemSlot_OverrideLeftClick_FaultyCoin(Item[] inv, int context, int slot) {

            if (inv[slot].IsAir || inv[slot].ModItem is not FaultyCoin faultyCoin) {
                return false;
            }

            if (Math.Abs(context) == ItemSlot.Context.EquipAccessory) {
                long amount = Main.LocalPlayer.Aequus().accFaultyCoinDebt;
                if (!Main.LocalPlayer.CanAfford((int)amount)) {
                    faultyCoin.OnUnsuccessfulRemove(Main.LocalPlayer);
                    return true;
                }
                faultyCoin.OnRemoveAccessory(Main.LocalPlayer);
            }

            return false;
        }
    }
}