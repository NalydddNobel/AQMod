using Aequus.Buffs.Debuffs;
using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using ModGlobalsNet;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
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

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return false;
            }

            var players = GetCloseEnoughPlayers(npc);

            if (npc.HasBuff<SoulStolen>())
            {
                CheckSouls(players);
            }

            if (npc.type == NPCID.DungeonGuardian || npc.SpawnedFromStatue)
            {
                return false;
            }

            if (NecromancyDatabase.TryGetByNetID(npc, out var info))
            {
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                if ((info.PowerNeeded != 0f || zombie.zombieDebuffTier >= 100f) && CheckRecruitable(npc, zombie, info, players))
                {
                    zombie.SpawnZombie(npc);
                }
            }
            return false;
        }
        public void CheckSouls(List<(Player, AequusPlayer, float)> players)
        {
            int closest = -1;
            float closestDistance = 2000f;
            foreach (var p in players)
            {
                if (p.Item3 < closestDistance && p.Item2.candleSouls < p.Item2.soulCandleLimit)
                {
                    closest = p.Item1.whoAmI;
                    closestDistance = p.Item3;
                }
            }
            if (closest != -1)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.player[closest].Aequus().candleSouls++; // bru
                }
                else
                {
                    PacketHandler.Send((p) =>
                    {
                        p.Write(closest);
                    }, PacketType.GiveoutEnemySoul);
                }
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
        public bool CheckRecruitable(NPC npc, NecromancyNPC zombie, GhostInfo info, List<(Player, AequusPlayer, float)> players)
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
    }
}