using Aequus.Localization;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Content.Artifacts
{
    public sealed class ArtifactsNPC : GlobalNPC
    {
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