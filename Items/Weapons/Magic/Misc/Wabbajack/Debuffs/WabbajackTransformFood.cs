using Aequus.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.Misc.Wabbajack.Debuffs {
    public class WabbajackTransformFood : ModBuff {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void Update(NPC npc, ref int buffIndex) {
            int i = npc.whoAmI;
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), npc.Bottom, Microsoft.Xna.Framework.Vector2.Zero,
                    ModContent.ProjectileType<WabbajackEffect>(), 0, 0f, Main.myPlayer);
                try {
                    var foodDrops = new List<int>();

                    var drops = Main.ItemDropsDB.GetRulesForNPCID(npc.netID, includeGlobalDrops: true);
                    var info = new DropAttemptInfo {
                        npc = npc,
                        IsExpertMode = Main.expertMode,
                        IsMasterMode = Main.masterMode,
                        player = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)],
                        rng = Main.rand
                    };
                    var rateInfo = new List<DropRateInfo>();
                    foreach (var dropsItem in drops) {
                        if (dropsItem.CanDrop(info)) {
                            dropsItem.ReportDroprates(rateInfo, new DropRateInfoChainFeed(1f));
                        }
                    }
                    foreach (var itemDrop in rateInfo) {
                        var itemInstance = ContentSamples.ItemsByType[itemDrop.itemId];
                        if (itemInstance.buffTime > 0 && itemInstance.buffType > 0 && BuffID.Sets.IsFedState[itemInstance.buffType]) {
                            foodDrops.Add(itemDrop.itemId);
                        }
                    }
                    if (foodDrops.Count == 0) {
                        foreach (var pair in ContentSamples.ItemsByType) {
                            var itemInstance = pair.Value;
                            if (itemInstance.buffTime > 0 && itemInstance.buffType > 0 && BuffID.Sets.IsFedState[itemInstance.buffType]) {
                                foodDrops.Add(pair.Key);
                            }
                        }
                    }
                    if (foodDrops.Count > 0)
                        Item.NewItem(npc.GetSource_Loot("Aequus: Wabbajack!"), npc.Center, Main.rand.Next(foodDrops), 1);
                }
                catch {
                }
            }
            npc.DelBuff(buffIndex);
            buffIndex--;
            npc.KillEffects(quiet: Main.netMode == NetmodeID.MultiplayerClient);
        }
    }
}