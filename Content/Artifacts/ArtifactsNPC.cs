using Aequus.Common.Utilities;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Artifacts
{
    public sealed class ArtifactsNPC : GlobalNPC
    {
        public NPC swarmParent;
        public bool initalized;
        public float specialAIValue;

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        private void SwarmStats(NPC npc)
        {
            if (npc.lifeMax > 10)
            {
                npc.lifeMax /= 2;
            }
            npc.life = Math.Min(npc.life, npc.lifeMax);
            npc.scale *= 0.8f;
        }
        private void SwarmSpawn(NPC parent, NPC swarmChild)
        {
            for (int i = 0; i < parent.localAI.Length; i++)
            {
                swarmChild.localAI[i] = parent.localAI[i];
            }
            swarmChild.velocity = -parent.velocity;
            if (swarmChild.velocity.Length() > 0f)
            {
                swarmChild.velocity += new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
            }
            if (swarmChild.noTileCollide)
            {
                swarmChild.velocity += new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                swarmChild.position += new Vector2(Main.rand.NextFloat(-64f, 64f), Main.rand.NextFloat(-64f, 64f));
            }
            swarmChild.frame = parent.frame;
            var a = swarmChild.GetGlobalNPC<ArtifactsNPC>();
            a.swarmParent = parent;
            if (parent.ModNPC != null)
            {
                try
                {
                    parent.ModNPC.ReflectiveCloneTo(swarmChild.ModNPC);
                    var npcPropertyFix = swarmChild.ModNPC.GetType().GetProperty("NPC", BindingFlags.Public | BindingFlags.Instance);
                    npcPropertyFix.SetValue(swarmChild.ModNPC, swarmChild);
                }
                catch
                {
                    ArtifactsSystem.SwarmsNPCBlacklist.Add(parent.type);
                }
            }
        }
        public override void PostAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || !ArtifactsSystem.SwarmsGameMode)
            {
                return;
            }
            if (swarmParent != null)
            {
                if ((int)specialAIValue == 0)
                {
                    specialAIValue = -1f;
                }
                if (!swarmParent.active || swarmParent.netID != npc.netID)
                {
                    swarmParent = null;
                }
                else
                {
                    if (npc.type == NPCID.EyeofCthulhu || npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
                    {
                        if ((int)npc.ai[0] == 0)
                        {
                            npc.ai[0] = 1;
                        }
                    }
                    else if (npc.type == NPCID.SkeletronHead || npc.type == NPCID.SkeletronPrime)
                    {
                        if ((int)npc.ai[1] == 0)
                        {
                            npc.ai[1] = 1;
                        }
                    }
                    else if (npc.type == NPCID.DukeFishron)
                    {
                        if ((int)npc.ai[0] == 0)
                        {
                            npc.ai[0] = 4;
                        }
                    }
                    else if (npc.type == NPCID.HallowBoss)
                    {
                        if ((int)npc.ai[0] == (int)swarmParent.ai[0])
                        {
                            npc.ai[0]++;
                            npc.ai[0] %= 13f;
                            npc.ai[0] = Math.Max(npc.ai[0], 1f);
                        }
                    }
                    else if (npc.type == NPCID.CultistBoss)
                    {
                        if (npc.ai[0] != 5f && npc.ai[0] != 6f)
                        {
                            npc.ai[3] = -1f;
                            npc.ai[2] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[0] = 5f;
                        }
                    }
                    else if (npc.type == NPCID.MartianSaucerCore)
                    {
                        if (npc.dontTakeDamage && swarmParent.dontTakeDamage)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                }
            }
            else
            {
                if (!initalized && !npc.townNPC)
                {
                    if (npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight)
                    {
                        SwarmStats(npc);
                    }
                    else if (npc.type == NPCID.GolemHeadFree)
                    {
                        var p = npc.Center;
                        int golem = NPC.FindFirstNPC(NPCID.Golem);
                        if (golem != -1)
                        {
                            int n = NPC.NewNPC(null, (int)p.X, (int)p.Y, NPCID.Golem, golem, Main.npc[golem].ai[0], Main.npc[golem].ai[1], Main.npc[golem].ai[2], Main.npc[golem].ai[3], Main.npc[golem].target);
                            if (n != -1)
                            {
                                SwarmStats(Main.npc[golem]);
                                SwarmStats(Main.npc[n]);
                                SwarmSpawn(Main.npc[golem], Main.npc[n]);
                            }
                        }
                    }
                    else if (!ArtifactsSystem.SwarmsNPCBlacklist.Contains(npc.type) && npc.aiStyle != NPCAIStyleID.Worm && npc.aiStyle != NPCAIStyleID.TheDestroyer && !NPCID.Sets.ProjectileNPC[npc.type])
                    {
                        var p = npc.Center;
                        int n = NPC.NewNPC(null, (int)p.X, (int)p.Y, npc.type, npc.whoAmI, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.target);
                        if (n != -1)
                        {
                            SwarmStats(npc);
                            SwarmStats(Main.npc[n]);
                            SwarmSpawn(npc, Main.npc[n]);
                        }
                    }
                }
                if (npc.type == NPCID.BrainofCthulhu)
                {
                    if ((int)npc.ai[0] == -1 && (int)specialAIValue == 0)
                    {
                        var p = npc.Center;
                        int n = NPC.NewNPC(null, (int)p.X, (int)p.Y, npc.type, npc.whoAmI, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.target);
                        if (n != -1)
                        {
                            SwarmStats(npc);
                            SwarmStats(Main.npc[n]);
                            SwarmSpawn(npc, Main.npc[n]);
                            Main.npc[n].localAI[0] = 1f;
                        }
                        Main.npc[n].alpha = 250;
                        specialAIValue++;
                    }
                }
            }

            initalized = true;
        }

        public override bool PreKill(NPC npc)
        {
            if (ArtifactsSystem.CommandGameMode)
            {
                var info = default(DropAttemptInfo);
                info.player = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
                info.npc = npc;
                info.IsExpertMode = Main.expertMode;
                info.IsMasterMode = Main.masterMode;
                info.IsInSimulation = false;
                info.rng = Main.rand;

                var extractedDrops = GetDrops(info);
                //foreach (var drop in extractedDrops)
                //{
                //    Main.NewText("{"+ AequusText.Item(drop.itemId) + ", " + drop.stackMin + ", " + drop.stackMax + "}", Main.DiscoColor);
                //}
                if (extractedDrops.Count > 0)
                {
                    var list = new List<Vector3>();
                    foreach (var d in extractedDrops)
                    {
                        list.Add(new Vector3(d.itemId, d.stackMin, d.stackMax));
                    }
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && npc.playerInteraction[i])
                        {
                            int p = Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(2f, 4f), ModContent.ProjectileType<ItemDropChooser>(), 0, 0f, i);
                            Main.projectile[p].Mod<ItemDropChooser>().drops = list;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        private List<DropRateInfo> GetDrops(DropAttemptInfo info)
        {
            var rulesForNPCID = Main.ItemDropsDB.GetRulesForNPCID(info.npc.netID, includeGlobalDrops: false);
            var list = new List<DropRateInfo>();
            var ratesInfo = new DropRateInfoChainFeed(1f);
            foreach (var dropRule in rulesForNPCID)
            {
                dropRule.ReportDroprates(list, ratesInfo);
            }
            for (int i = 0; i < list.Count; i++)
            {
                DropRateInfo dropRate = list[i];
                if (dropRate.conditions != null)
                {
                    foreach (var c in dropRate.conditions)
                    {
                        if (!c.CanDrop(info) || !c.CanShowItemDropInUI())
                        {
                            list.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
            return list;
        }
    }
}