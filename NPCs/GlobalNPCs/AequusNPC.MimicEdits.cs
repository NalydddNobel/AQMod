using Aequus;
using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public partial class AequusNPC : GlobalNPC
    {
        private void Unload_MimicEdits()
        {
            if (!Main.dedServ)
            {
                NPCID.Sets.TrailingMode[NPCID.Mimic] = -1;
            }
        }

        private void SetDefaults_MimicEdits(NPC npc)
        {
            if ((npc.type != NPCID.Mimic && npc.type != NPCID.IceMimic) || !GameplayConfig.Instance.EarlyMimics || Main.hardMode)
            {
                return;
            }
            npc.value /= 5;
            npc.lifeMax /= 3;
            npc.damage /= 4;
            npc.defense /= 3;
        }

        private void ModifyLoot_Mimics(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Mimic)
            {
                var regularMimicConditon = new FuncNPCInstanceConditional(
                    (npc) => !Main.hardMode && npc.ai[3] != 3f,
                    "Regular Mimic",
                    null);
                npcLoot.Add(new LeadingConditionRule(regularMimicConditon))
                .OnSuccess(new OneFromOptionsDropRule(
                    1, 1,
                    ItemID.BandofRegeneration, ItemID.MagicMirror, ItemID.CloudinaBottle, ItemID.HermesBoots, ItemID.Mace, ItemID.ShoeSpikes
                ));
                npcLoot.Add(new LeadingConditionRule(regularMimicConditon))
                .OnSuccess(new OneFromOptionsDropRule(
                    2, 1,
                    ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>(), ModContent.ItemType<BoneRing>()
                ));
                npcLoot.Add(new LeadingConditionRule(regularMimicConditon))
                .OnSuccess(new OneFromOptionsDropRule(
                    3, 1,
                    ModContent.ItemType<GlowCore>(), ModContent.ItemType<MiningPetSpawner>()
                ));

                npcLoot.Add(new LeadingConditionRule(
                new FuncNPCInstanceConditional(
                    (npc) => !Main.hardMode && npc.ai[3] == 3f,
                    "Shadow Mimic",
                    "Mods.Aequus.DropCondition.ShadowMimic")))
                .OnSuccess(new OneFromOptionsDropRule(
                    1, 1,
                    ItemID.Sunfury, ItemID.FlowerofFire, ItemID.Flamelash, ItemID.DarkLance, ItemID.HellwingBow
                ));
            }
            else if (npc.type == NPCID.IceMimic)
            {
                npcLoot.Add(new LeadingConditionRule(new Conditions.IsPreHardmode()))
                .OnSuccess(new OneFromOptionsDropRule(
                    1, 1,
                    ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.SnowballCannon, ItemID.BlizzardinaBottle, ItemID.FlurryBoots
                ));
                npcLoot.Add(new LeadingConditionRule(new Conditions.IsPreHardmode()))
                .OnSuccess(new OneFromOptionsDropRule(
                    2, 1,
                    ModContent.ItemType<CrystalDagger>()
                ));
                npcLoot.Add(new LeadingConditionRule(new Conditions.IsPreHardmode()))
                .OnSuccess(ItemDropRule.Common(ItemID.Fish, 10));
            }
        }

        private void PatchRegularMimicLoot()
        {
            // ?????????????????????????? Does this really even work
            GlobalNPCExtensions.LockDrops(NPCID.Mimic, new Conditions.IsHardmode(), (r) =>
            {
                var ratesInfo = new DropRateInfoChainFeed() { parentDroprateChance = 1f, };
                var l = new List<DropRateInfo>();
                r.ReportDroprates(l, ratesInfo);
                return l.ContainsAny((d) => new Item(d.itemId).rare > ItemRarityID.Orange);
            });
        }

        private void PatchIceMimicLoot()
        {
            GlobalNPCExtensions.LockDrops(NPCID.IceMimic, new Conditions.IsHardmode(), (r) =>
            {
                var ratesInfo = new DropRateInfoChainFeed() { parentDroprateChance = 1f, };
                var l = new List<DropRateInfo>();
                r.ReportDroprates(l, ratesInfo);
                return l.ContainsAny((d) => new Item(d.itemId).rare > ItemRarityID.Orange);
            });
        }

        private void AddRecipes_PatchMimicLoot()
        {
            if (!GameplayConfig.Instance.EarlyMimics)
                return;

            PatchRegularMimicLoot();
            PatchIceMimicLoot();
        }

        private bool PreDraw_MimicEdits(NPC npc, SpriteBatch sb, Vector2 screenPos, Color drawColor)
        {
            if (AequusWorld.hardmodeChests && Main.hardMode && ClientConfig.Instance.AdamantiteChestMimic
                && npc.type == NPCID.Mimic && npc.frame.Y >= npc.frame.Height * 6 && npc.frame.Y < npc.frame.Height * 12)
            {
                if (NPCID.Sets.TrailingMode[NPCID.Mimic] == -1)
                    NPCID.Sets.TrailingMode[NPCID.Mimic] = 7;

                var texture = ModContent.Request<Texture2D>($"{ModContent.GetInstance<AequusNPC>().GetNoNamePath()}/Vanilla/AdamantiteMimic");
                var frame = texture.Value.Frame(verticalFrames: 6, frameY: npc.frame.Y / npc.frame.Height % 6);
                int trailLength = Math.Min(NPCID.Sets.TrailCacheLength[npc.type], 6);
                var offset = npc.Size / 2f + new Vector2(0f, -7f);
                var origin = frame.Size() / 2f;
                var spriteDirection = (-npc.spriteDirection).ToSpriteEffect();
                for (int i = 0; i < trailLength; i++)
                {
                    if (i < trailLength - 1 && (npc.oldPos[i] - npc.oldPos[i + 1]).Length() < 1f)
                    {
                        continue;
                    }
                    sb.Draw(texture.Value, (npc.oldPos[i] - screenPos + offset).Floor(), frame,
                        Helper.GetColor(npc.oldPos[i] + offset) * Helper.CalcProgress(trailLength, i) * 0.4f, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                }
                sb.Draw(texture.Value, (npc.position - screenPos + offset).Floor(), frame,
                    drawColor, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                return false;
            }
            return true;
        }

        public static void PreHardmodeMimics(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode || !GameplayConfig.Instance.EarlyMimics
                || spawnInfo.SpawnTileY < ((int)Main.worldSurface + 100) || spawnInfo.Water)
                return;

            var tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
            if (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock)
            {
                pool[NPCID.IceMimic] = 0.014f;
            }
            else if (TileID.Sets.IcesSnow[tile.TileType] && spawnInfo.SpawnTileY >= (int)Main.rockLayer) // Things like Ebonstone and ect
            {
                pool[NPCID.IceMimic] = 0.025f;
            }
            else if (tile.TileType == TileID.Stone)
            {
                pool[NPCID.Mimic] = spawnInfo.SpawnTileY >= Main.rockLayer ? 0.02f : 0.009f;
            }
            else if (TileID.Sets.Conversion.Stone[tile.TileType] && spawnInfo.SpawnTileY >= (int)Main.rockLayer) // Things like Ebonstone and ect
            {
                pool[NPCID.Mimic] = 0.02f;
            }
        }
    }
}