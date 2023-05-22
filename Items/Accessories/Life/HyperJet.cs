using Aequus.Common.Recipes;
using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Accessories.Misc.Info;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Life {
    public class HyperJet : ModItem, ItemHooks.IUpdateVoidBag {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            CrownOfBloodItem.NoBoost.Add(Type);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
            Item.rare = ItemRarityID.Orange;
        }

        private void UpdateHyperJet(AequusPlayer aequus) {
            aequus.accHyperJet = Math.Max(aequus.accHyperJet, 1);
        }

        public override void UpdateInventory(Player player) {
            UpdateHyperJet(player.Aequus());
        }

        void ItemHooks.IUpdateVoidBag.UpdateBank(Player player, AequusPlayer aequus, int slot, int bank) {
            UpdateHyperJet(player.Aequus());
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            UpdateHyperJet(player.Aequus());
        }

        public static void RespawnTime(Player player, AequusPlayer aequus) {
            if (aequus.accHyperJet < 180) {
                aequus.accHyperJet++;
                return;
            }
            if (NPC.AnyDanger(quickBossNPCCheck: true, ignorePillarsAndMoonlordCountdown: false)) {
                return;
            }

            var spawnTile = player.GetSpawn();
            var spawnCoords = spawnTile.ToWorldCoordinates();
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && Main.npc[i].damage > 0) {
                    if (Vector2.Distance(spawnCoords, Main.npc[i].Center) < 800f) {
                        return;
                    }
                }
            }

            if (player.respawnTimer > 5) {
                player.respawnTimer = Math.Max(player.respawnTimer - 4, 5);
            }
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<HoloLens>());
        }
    }
}