﻿using Aequus.Common.Items.DropRules;
using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Items.Materials;
using Aequus.NPCs.Monsters.ChestMimics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs {
    public partial class AequusNPC {
        private void OnSpawn_MimicConversion(NPC npc) {
            if (npc.type == NPCID.IceMimic) {
                if (Main.hardMode && GameplayConfig.Instance.AdamantiteMimics && !Main.rand.NextBool(3)) {
                    npc.Transform(ModContent.NPCType<FrostMimic>());
                }
            }
            else if (npc.type == NPCID.Mimic) {
                if (NPC.downedBoss3 && npc.position.Y / 16f > Main.UnderworldLayer) {
                    npc.Transform(ModContent.NPCType<ShadowMimic>());
                }
                if (Main.hardMode && GameplayConfig.Instance.AdamantiteMimics && !Main.rand.NextBool(3)) {
                    if (npc.position.Y > Main.worldSurface * 16f && npc.position.Y < Main.UnderworldLayer * 16f) {
                        npc.Transform(ModContent.NPCType<AdamantiteMimic>());
                    }
                }
            }
        }

        private void PreHardmodeMimics(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (Main.hardMode || Main.remixWorld || !GameplayConfig.Instance.EarlyMimics
                || spawnInfo.SpawnTileY < ((int)Main.worldSurface + 100) || spawnInfo.Water)
                return;

            var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
            if (TileID.Sets.IcesSnow[tile.TileType]) {
                pool[NPCID.IceMimic] = 0.05f;
                return;
            }

            if (spawnInfo.SpawnTileY >= Main.UnderworldLayer && !NPC.downedBoss3) {
                return;
            }

            if (TileID.Sets.Conversion.Stone[tile.TileType] || TileID.Sets.Ash[tile.TileType]) {
                pool[NPCID.Mimic] = spawnInfo.SpawnTileY >= Main.rockLayer ? 0.04f : 0.01f;
            }
        }

        public void PreExtractBestiaryItemDrops(Aequus aequus, BestiaryDatabase bestiaryDatabase, ItemDropDatabase database) {
            if (!GameplayConfig.Instance.EarlyMimics) {
                return;
            }

            var rules = Helper.GetNPCLoot(NPCID.Mimic);
            rules.RemoveWhere(i => i is LeadingConditionRule conditionRule && conditionRule.condition is not Conditions.RemixSeedEasymode && conditionRule.condition is not Conditions.IsPreHardmode, includeGlobalDrops: false);
            var phmRemixRules = rules.FindAll(i => i is LeadingConditionRule conditionRule && conditionRule.condition is Conditions.RemixSeedEasymode);
            foreach (var i in phmRemixRules) {
                if (i.ChainedRules == null) {
                    continue;
                }
                rules.Remove(i);
                rules.AddRange(i.ChainedRules);
            }

            rules = Helper.GetNPCLoot(NPCID.IceMimic);
            rules.Clear();
            rules.AddMultiConditionRule(new OneFromOptionsDropRule(1, 1,
                ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.SnowballCannon, ItemID.BlizzardinaBottle, ItemID.FlurryBoots
            ), new Conditions.NotRemixSeed());
            rules.AddConditionRule(new Conditions.RemixSeed(), new OneFromOptionsDropRule(1, 1,
                ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.IceBow, ItemID.BlizzardinaBottle, ItemID.FlurryBoots
            ));
            rules.Add(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), 2, 1, 2));
            rules.Add(ItemDropRule.Common(ItemID.Fish, 10));
        }
    }
}