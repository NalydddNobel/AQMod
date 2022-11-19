using Aequus.Buffs.Misc;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public abstract class BaseGhostSpawner : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;
        public virtual float SpeedAdd => 0f;
        public virtual int AuraColor => ColorTargetID.BloodRed;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 40;
            Projectile.alpha = 255;
            Projectile.scale = 0.1f;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 25)
            {
                Projectile.scale -= 0.01f;
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 8;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                    }
                }
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.scale += 0.03f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
                Projectile.alpha -= 5;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            int buff = Main.player[Projectile.owner].FindBuffIndex(ModContent.BuffType<RitualBuff>());
            if (buff == -1)
                return;
            if (Main.player[Projectile.owner].buffTime[buff] < 10)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int x = (int)Projectile.position.X + Projectile.width / 2;
                        int y = (int)Projectile.position.Y + Projectile.height / 2;
                        int type = NPCType();
                        int n = NPC.NewNPC(new EntitySource_Misc("Aequus:NecromancySpawn"), x, y, type);
                        OnSpawnZombie(Main.npc[n], Main.npc[n].GetGlobalNPC<NecromancyNPC>());
                        PacketSystem.SyncNPC(Main.npc[n]);
                    }
                    SpawnEffects();
                }
                Projectile.ai[0] = 1f;
            }
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 27);
            }
        }

        public abstract int NPCType();

        protected virtual void OnSpawnZombie(NPC npc, NecromancyNPC zombie)
        {
            var aequus = Main.player[Projectile.owner].Aequus();
            if (aequus.ghostSlotsOld + 1 > aequus.ghostSlotsMax)
            {
                zombie.MakeRoomForMe(npc, out int killMinion);
                if (killMinion != -1)
                {
                    Main.npc[killMinion].KillMe();
                }
            }
            zombie.zombieOwner = Projectile.owner;
            zombie.SpawnZombie_SetZombieStats(npc, Projectile.Center, Projectile.velocity, 0, 0, out bool _);
            zombie.zombieTimerMax /= 3;
            zombie.zombieTimer /= 3;
            zombie.renderLayer = AuraColor;
            zombie.ghostDamage = Projectile.damage;
            zombie.ghostSpeed += SpeedAdd;
        }
        protected virtual void SpawnEffects()
        {
        }
    }
}