using Aequus;
using Aequus.Buffs.Misc;
using Aequus.Common.Net;
using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Misc.Money;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Accessories.Misc.Money {
    [AutoloadEquip(EquipType.HandsOn)]
    public class FoolsGoldRing : ModItem {
        /// <summary>
        /// Defalut Value: 10
        /// </summary>
        public static int GoldCoinsRequired = 10;

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accFoolsGoldRing++;
            player.equipmentBasedLuckBonus -= 0.05f;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<BusinessCard>());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            string colorText = TextHelper.ColorCommandStart(Colors.CoinGold, alphaPulse: true);
            foreach (var t in tooltips) {
                if (t.Mod != "Terraria") {
                    continue;
                }

                t.Text = t.Text.Replace("[[", colorText);
                t.Text = t.Text.Replace("]]", "]");
            }
        }

        public static void ProcOnPickupCoin(Item item, Player player, AequusPlayer aequus) {
            if (aequus.accFoolsGoldRing <= 0) {
                return;
            }

            int valueAdd = item.value * item.stack * aequus.accFoolsGoldRing / 5;
            if (valueAdd + aequus.accFoolsGoldRingCharge % Item.gold > Item.gold
                && aequus.accFoolsGoldRingCharge < Item.gold * GoldCoinsRequired) {
                aequus.showFoolsGoldRingChargeDuration = 180;
            }
            aequus.accFoolsGoldRingCharge += valueAdd;
        }

        public static void OnKillEffects(int x, int y, int width, int height) {
            if (Main.netMode != NetmodeID.Server) {

                int amt = Math.Max((int)((width + height) / 2f), 1);
                Rectangle rect = new(x - 20, y - 10, width + 40, height + 20);
                Vector2 center = new(x + width / 2f, y + height / 2f);

                SoundEngine.PlaySound(AequusSounds.slotMachine, center);

                for (int i = 0; i < amt; i++) {
                    float intensity = (float)Math.Pow(0.9f, i + 1);
                    ParticleSystem.New<ShinyFlashParticle>(ParticleLayer.AboveDust).Setup(Main.rand.NextFromRect(rect), Vector2.Zero, Color.Yellow.UseA(0), Color.White * 0.33f, Main.rand.NextFloat(0.5f, 1f) * intensity, 0.2f, 0f);
                }

                Vector2 pos = new(x - 20, y - 10);
                int dustWidth = width + 40;
                int dustHeight = height + 20;

                for (int i = 0; i < amt * 3; i++) {
                    var d = Dust.NewDustDirect(pos, dustWidth, dustHeight, DustID.SpelunkerGlowstickSparkle);
                    d.velocity *= 0.5f;
                    d.velocity = (d.position - center).SafeNormalize(Vector2.UnitY) * d.velocity.Length();
                }
            }
        }

        public static void DrawCounter(ref PlayerDrawSet drawInfo) {
            var aequus = drawInfo.drawPlayer.Aequus();
            if (aequus.showFoolsGoldRingChargeDuration <= 0 || aequus.accFoolsGoldRingCharge < Item.gold) {
                return;
            }

            float opacity = 1f;
            if (aequus.showFoolsGoldRingChargeDuration > 170) {
                opacity = 1f - (aequus.showFoolsGoldRingChargeDuration - 170) / 10f;
            }
            if (aequus.showFoolsGoldRingChargeDuration < 15) {
                opacity = aequus.showFoolsGoldRingChargeDuration / 15f;
            }

            Main.instance.LoadItem(ItemID.GoldCoin);

            var drawPosition = drawInfo.drawPlayer.Bottom + new Vector2(-drawInfo.drawPlayer.width / 2f, 14f + drawInfo.drawPlayer.gfxOffY) - Main.screenPosition;
            Helper.GetItemDrawData(ItemID.GoldCoin, out var frame);
            Main.spriteBatch.Draw(
                TextureAssets.Item[ItemID.GoldCoin].Value,
                (drawPosition - new Vector2(frame.Width - 4f, 0f)).Floor(),
                frame,
                Color.White * opacity,
                0f,
                frame.Size() / 2f,
                1f,
                SpriteEffects.None,
                0f
            );
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch,
                FontAssets.MouseText.Value,
                $"{Math.Clamp(aequus.accFoolsGoldRingCharge / Item.gold, 0, 10)}/{GoldCoinsRequired}",
                (drawPosition + new Vector2(0f, -frame.Height + 4f)).Floor(),
                Colors.CoinGold * opacity,
                (Colors.CoinGold * 0.3f) with { A = 255 } * opacity,
                0f,
                Vector2.Zero,
                Vector2.One
            );
        }
    }

    public class FoolsGoldRingEffectPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.FoolsGoldRingEffect;

        public void Send(NPC npc) {
            var p = GetPacket();
            p.Write((int)npc.position.X);
            p.Write((int)npc.position.Y);
            p.Write(npc.width);
            p.Write(npc.height);
            p.Send();
        }

        public override void Receive(BinaryReader reader) {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int w = reader.ReadInt32();
            int h = reader.ReadInt32();

            FoolsGoldRing.OnKillEffects(x, y, w, h);
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public void ProcFoolsGoldRing(NPC npc, AequusNPC aequus, Player player, AequusPlayer aequusPlayer) {

            if (npc.value <= 0f || !player.HasBuff<FoolsGoldRingBuff>()) {
                return;
            }

            aequusPlayer.accFoolsGoldRingCharge = 0;

            npc.value *= 3f;
            aequus.dropRerolls += 3f;
            doLuckyDropsEffect = true;

            if (Main.netMode != NetmodeID.Server) {
                FoolsGoldRing.OnKillEffects((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            }
            else {
                ModContent.GetInstance<FoolsGoldRingEffectPacket>().Send(npc);
            }

            doLuckyDropsEffect = false;
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        /// <summary>
        /// Set by <see cref="FoolsGoldRing"/>
        /// </summary>
        public int accFoolsGoldRing;
        public int accFoolsGoldRingCharge;
        public float increasedEnemyMoney;
        public int showFoolsGoldRingChargeDuration;

        public void ResetEffects_FoolsGoldRing() {
            accFoolsGoldRing = 0;
            increasedEnemyMoney = 0f;

            if (showFoolsGoldRingChargeDuration > 0)
                showFoolsGoldRingChargeDuration--;
        }

        public void PostUpdate_FoolsGoldRing() {
            if (accFoolsGoldRingCharge > Item.gold * FoolsGoldRing.GoldCoinsRequired) {
                Player.AddBuff(ModContent.BuffType<FoolsGoldRingBuff>(), 4);
            }
            if (accFoolsGoldRing <= 0) {
                accFoolsGoldRingCharge = 0;
                showFoolsGoldRingChargeDuration = 0;
            }
        }
    }
}