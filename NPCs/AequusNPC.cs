using Aequus.Buffs.Debuffs;
using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Weapons.Melee;
using ModGlobalsNet;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class AequusNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void Load()
        {
            On.Terraria.NPC.VanillaHitEffect += Hook_PreHitEffect;
        }
        private static void Hook_PreHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                if (Main.netMode != NetmodeID.Server && self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self))
                {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch
            {

            }
            orig(self, hitDirection, dmg);
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && npc.HasBuff<SnowgraveDebuff>())
            {
                DeathEffect_SnowgraveFreeze(npc);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient
                || npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
            {
                return false;
            }

            var players = GetCloseEnoughPlayers(npc);
            if (npc.HasBuff<SoulStolen>())
            {
                CheckSouls(players);
            }
            if (NecromancyDatabase.TryGetByNetID(npc, out var info))
            {
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                if ((info.PowerNeeded != 0f || zombie.zombieDebuffTier >= 100f) && GhostKill(npc, zombie, info, players))
                {
                    zombie.SpawnZombie(npc);
                }
            }

            return false;
        }
        public void CheckSouls(List<(Player, AequusPlayer, float)> players)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                foreach (var p in players)
                {
                    if (p.Item2.candleSouls < p.Item2.soulCandleLimit)
                    {
                        p.Item2.candleSouls++;
                    }
                }
            }
            else
            {
                List<int> candlePlayers = new List<int>();
                foreach (var p in players)
                {
                    if (p.Item2.candleSouls < p.Item2.soulCandleLimit)
                    {
                        candlePlayers.Add(p.Item1.whoAmI);
                    }
                }

                if (candlePlayers.Count > 0)
                {
                    PacketHandler.Send((p) =>
                    {
                        p.Write(candlePlayers.Count);
                        for (int i = 0; i < candlePlayers.Count; i++)
                        {
                            p.Write(candlePlayers[i]);
                        }
                    }, PacketType.GiveoutEnemySouls);
                }
            }
        }
        public bool GhostKill(NPC npc, NecromancyNPC zombie, GhostInfo info, List<(Player, AequusPlayer, float)> players)
        {
            if (zombie.zombieDrain > 0 && info.PowerNeeded <= zombie.zombieDebuffTier)
            {
                return true;
            }
            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].Aequus().dreamMask && Main.rand.NextBool(4))
            //    {
            //        zombie.zombieOwner = players[i].whoAmI;
            //        zombie.zombieDebuffTier = info.PowerNeeded;
            //        return true;
            //    }
            //}
            return false;
        }
        public void DeathEffect_SnowgraveFreeze(NPC npc)
        {
            if (SnowgraveCorpse.CanFreezeNPC(npc))
            {
                EffectsSystem.BehindProjs.Add(new SnowgraveCorpse(npc.Center, npc));
            }
        }
        public List<(Player, AequusPlayer, float)> GetCloseEnoughPlayers(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return new List<(Player, AequusPlayer, float)>() { (Main.LocalPlayer, Main.LocalPlayer.Aequus(), npc.Distance(Main.LocalPlayer.Center)), };
            }
            var list = new List<(Player, AequusPlayer, float)>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    float d = npc.Distance(Main.player[i].Center);
                    if (d < 2000f)
                    {
                        list.Add((Main.player[i], Main.player[i].Aequus(), d));
                    }
                }
            }
            return list;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalDagger>(), 50));
            }
            else if (npc.type == NPCID.DevourerHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.TombCrawlerHead
                || npc.type == NPCID.DiggerHead || npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.SeekerHead || npc.type == NPCID.BloodEelHead)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
            }
        }
    }
}