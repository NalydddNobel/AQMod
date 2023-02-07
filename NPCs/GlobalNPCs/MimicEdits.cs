using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.GlobalNPCs
{
    public class MimicEdits : GlobalNPC
    {
        public override void Unload()
        {
            if (!Main.dedServ)
            {
                NPCID.Sets.TrailingMode[NPCID.Mimic] = -1;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if ((npc.type == NPCID.Mimic || npc.type == NPCID.IceMimic) && GameplayConfig.Instance.EarlyMimics && !Main.hardMode)
            {
                npc.value /= 5;
                npc.lifeMax /= 3;
                npc.damage /= 4;
                npc.defense /= 3;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!GameplayConfig.Instance.EarlyMimics)
                return;

            if (npc.type == NPCID.Mimic)
            {
                npcLoot.AddConditionToExistingRule<OneFromOptionsDropRule>((rule) => rule.dropIds.ContainsAny(ItemID.DualHook, ItemID.MagicDagger, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.StarCloak), new Conditions.IsHardmode());

                // Pre-Hardmode Mimic
                npcLoot.Add(new LeadingConditionRule(new FuncNPCInstanceConditional((npc) => !Main.hardMode && npc.ai[3] != 3f, "Regular Mimic", null))).OnSuccess(new OneFromOptionsDropRule(1, 1,
                    ItemID.BandofRegeneration, ItemID.MagicMirror, ItemID.CloudinaBottle, ItemID.HermesBoots, ItemID.Mace, ItemID.ShoeSpikes,
                    ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>(), ModContent.ItemType<BoneRing>()));

                // Pre-Hardmode Shadow Mimic
                npcLoot.Add(new LeadingConditionRule(new FuncNPCInstanceConditional((npc) => !Main.hardMode && npc.ai[3] == 3f, "Shadow Mimic", "Mods.Aequus.DropCondition.ShadowMimic"))).OnSuccess(new OneFromOptionsDropRule(1, 1,
                    ItemID.Sunfury, ItemID.FlowerofFire, ItemID.UnholyTrident, ItemID.Flamelash, ItemID.DarkLance, ItemID.HellwingBow));
            }
            else if (npc.type == NPCID.IceMimic)
            {
                npcLoot.AddConditionToExistingRule<CommonDrop>((rule) => rule.itemId == ItemID.ToySled, new Conditions.IsHardmode());

                // Pre-Hardmode Ice Mimic
                npcLoot.Add(new LeadingConditionRule(new Conditions.IsPreHardmode())).OnSuccess(new OneFromOptionsDropRule(1, 1,
                    ItemID.IceBoomerang, ItemID.IceBlade, ItemID.IceSkates, ItemID.SnowballCannon, ItemID.BlizzardinaBottle, ItemID.FlurryBoots,
                    ModContent.ItemType<CrystalDagger>()));
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
                    spriteBatch.Draw(texture.Value, (npc.oldPos[i] - screenPos + offset).Floor(), frame,
                        AequusHelpers.GetColor(npc.oldPos[i] + offset) * AequusHelpers.CalcProgress(trailLength, i) * 0.4f, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                }
                spriteBatch.Draw(texture.Value, (npc.position - screenPos + offset).Floor(), frame,
                    drawColor, npc.rotation, origin, npc.scale, spriteDirection, 0f);
                return false;
            }
            return true;
        }

        public static void PreHardmodeMimics(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode || !GameplayConfig.Instance.EarlyMimics)
                return;

            if (spawnInfo.SpawnTileY > Main.worldSurface)
            {
                pool[NPCID.Mimic] = 0.0033f;
                if (spawnInfo.Player.ZoneSnow)
                {
                    pool[NPCID.IceMimic] = 0.009f;
                }
            }
            if (spawnInfo.SpawnTileY > Main.rockLayer)
            {
                pool[NPCID.Mimic] = 0.011f;
                if (spawnInfo.Player.ZoneSnow)
                {
                    pool[NPCID.IceMimic] = 0.01f;
                }
            }
            if (spawnInfo.SpawnTileY > Main.UnderworldLayer - 32)
            {
                if (NPC.downedBoss3 || AequusWorld.downedEventDemon)
                {
                    pool[NPCID.Mimic] = 0.011f;
                }
                else
                {
                    pool[NPCID.Mimic] = 0f;
                }
            }
        }
    }
}