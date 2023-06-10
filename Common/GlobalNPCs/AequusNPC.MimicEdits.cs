using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs {
    public partial class AequusNPC : GlobalNPC {
        private void ModifyLoot_Mimics(NPC npc, NPCLoot npcLoot) {
            if (npc.type == NPCID.Mimic) {
                var phmCondition = new FuncNPCInstanceConditional((npc) => !Main.hardMode && npc.ai[3] != 3f, "Regular Mimic", null);

                npcLoot.AddConditionRule(new FuncNPCInstanceConditional((npc) => !Main.hardMode && npc.ai[3] != 3f && !Main.remixWorld, "Regular Mimic: No Remix Seed", null), new OneFromOptionsDropRule(1, 1,
                    ItemID.BandofRegeneration, ItemID.MagicMirror, ItemID.CloudinaBottle, ItemID.HermesBoots, ItemID.Mace, ItemID.ShoeSpikes
                ));
                npcLoot.AddConditionRule(phmCondition, new OneFromOptionsDropRule(2, 1,
                    ModContent.ItemType<Bellows>()
                ));
                npcLoot.AddConditionRule(phmCondition, new OneFromOptionsDropRule(3, 1,
                    ModContent.ItemType<GlowCore>(), ModContent.ItemType<MiningPetSpawner>()
                ));

                npcLoot.AddConditionRule(
                    new FuncNPCInstanceConditional((npc) => npc.ai[3] == 3f && !Main.remixWorld, "Shadow Mimic", "Mods.Aequus.DropCondition.ShadowMimic"), 
                    new OneFromOptionsDropRule(1, 1,
                    ItemID.Sunfury, ItemID.FlowerofFire, ItemID.Flamelash, ItemID.DarkLance, ItemID.HellwingBow
                ));
                npcLoot.AddConditionRule(
                    new FuncNPCInstanceConditional((npc) => npc.ai[3] == 3f && Main.remixWorld, "Shadow Mimic: Remix Seed", "Mods.Aequus.DropCondition.ShadowMimic"), 
                    new OneFromOptionsDropRule(1, 1,
                    ItemID.Sunfury, ItemID.UnholyTrident, ItemID.Flamelash, ItemID.DarkLance, ItemID.HellwingBow
                ));
            }
            else if (npc.type == NPCID.IceMimic) {
                npcLoot.AddMultiConditionRule(new OneFromOptionsDropRule(1, 1,
                    ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.SnowballCannon, ItemID.BlizzardinaBottle, ItemID.FlurryBoots, ItemID.Fish
                ), new Conditions.IsPreHardmode(), new Conditions.NotRemixSeed());
                npcLoot.AddMultiConditionRule(new OneFromOptionsDropRule(1, 1,
                    ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.IceBow, ItemID.BlizzardinaBottle, ItemID.FlurryBoots, ItemID.Fish
                ), new Conditions.IsPreHardmode(), new Conditions.RemixSeed());
            }
        }

        private void LockMimicLoot(int npcId) {
            Helper.AddDropRuleCondition(npcId, new Conditions.IsHardmode(), (r) => {
                var ratesInfo = new DropRateInfoChainFeed() { parentDroprateChance = 1f, };
                var l = new List<DropRateInfo>();
                r.ReportDroprates(l, ratesInfo);
                return l.ContainsAny((d) => new Item(d.itemId).rare > ItemRarityID.Orange);
            });
        }
        private void AddRecipes_PatchMimicLoot() {
            if (!GameplayConfig.Instance.EarlyMimics)
                return;

            LockMimicLoot(NPCID.Mimic);
            LockMimicLoot(NPCID.IceMimic);
        }

        public static void PreHardmodeMimics(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (Main.hardMode || !GameplayConfig.Instance.EarlyMimics
                || spawnInfo.SpawnTileY < ((int)Main.worldSurface + 100) || spawnInfo.Water)
                return;

            var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
            if (TileID.Sets.IcesSnow[tile.TileType]) {
                pool[NPCID.IceMimic] = spawnInfo.SpawnTileY >= (int)Main.rockLayer ? 0.014f : 0.025f;
                return;
            }

            if (spawnInfo.SpawnTileY >= Main.UnderworldLayer && !NPC.downedBoss3) {
                return;
            }

            if (TileID.Sets.Conversion.Stone[tile.TileType] || TileID.Sets.Ash[tile.TileType]) {
                pool[NPCID.Mimic] = spawnInfo.SpawnTileY >= Main.rockLayer ? 0.02f : 0.009f;
            }
        }
    }
}