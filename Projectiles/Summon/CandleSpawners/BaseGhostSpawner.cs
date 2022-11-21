using Aequus.Buffs.Misc;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public abstract class BaseGhostSpawner : ModProjectile
    {
        public override string Texture => "Aequus/Projectiles/Summon/CandleSpawners/SpawnEffect";
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

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPvp(Player target)
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
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: 2f);
                        Main.projectile[p].Kill();
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

        public override void Kill(int timeLeft)
        {
            if ((int)Projectile.ai[0] == 2 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = (int)Projectile.position.X + Projectile.width / 2;
                int y = (int)Projectile.position.Y + Projectile.height / 2;
                int type = NPCType();
                int n = NPC.NewNPC(new EntitySource_Misc("Aequus:NecromancySpawn"), x, y, type);
                OnSpawnZombie(Main.npc[n], Main.npc[n].GetGlobalNPC<NecromancyNPC>());
                PacketSystem.SyncNPC(Main.npc[n]);
            }
        }

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

        protected void QuickDrawAura(Vector2 drawCoords, Color mainColor, Color auraColor)
        {
            var frame = TextureAssets.Projectile[Type].Value.Frame(verticalFrames: 2, frameY: 0);
            var origin = frame.Size() / 2f + new Vector2(-1f);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height), auraColor * Projectile.Opacity,
                Projectile.rotation, origin, Projectile.scale * Projectile.Opacity * 0.66f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, frame, mainColor * Projectile.Opacity,
                Projectile.rotation, origin, Projectile.scale * Projectile.Opacity * 0.66f, SpriteEffects.None, 0);
        }
    }
}