using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class DeathEffects : GlobalNPC
    {
        public enum Context
        {
            None = 0,
            Snowgrave,
        }

        public override bool InstancePerEntity => true;

        public Context context;
        public int timer;

        public bool HasDeathContext => timer > 0 && context != Context.None;

        public void SetContext(Context context, int time)
        {
            if (timer < time)
            {
                this.context = context;
                timer = time;
            }
        }

        public void ClearContext()
        {
            context = Context.None;
            timer = 0;
        }

        public override void Load()
        {
            On.Terraria.NPC.VanillaHitEffect += NPC_VanillaHitEffect;
        }
        private static void NPC_VanillaHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                var deathEffects = self.GetGlobalNPC<DeathEffects>();
                if (deathEffects.HasDeathContext && Main.netMode != NetmodeID.Server)
                {
                    if (deathEffects.context == Context.Snowgrave && self.life <= 0 && SnowgraveCorpse.CanFreezeNPC(self))
                    {
                        SoundEngine.PlaySound(SoundID.Item30, self.Center);
                        return;
                    }
                }
            }
            catch
            {

            }
            orig(self, hitDirection, dmg);
        }

        public override void AI(NPC npc)
        {
            if (timer > 0)
                timer--;
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (HasDeathContext)
            {
                if (context == Context.Snowgrave && Main.netMode != NetmodeID.Server)
                {
                    DeathEffect_SnowgraveFreeze(npc);
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return false;
            }

            var players = GetCloseEnoughPlayers(npc);

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
        public List<Player> GetCloseEnoughPlayers(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return new List<Player>() { Main.LocalPlayer, };
            }
            var list = new List<Player>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    if (npc.Distance(Main.player[i].Center) < 2000f)
                    {
                        list.Add(Main.player[i]);
                    }
                }
            }
            return list;
        }
        public bool CheckRecruitable(NPC npc, NecromancyNPC zombie, GhostInfo info, List<Player> players)
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