using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class PumpinatorProj : ModProjectile
    {
        public virtual bool PushPlayers => true;
        public virtual bool OnlyPushHostilePlayers => false;

        public int dustPushIndex;

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.alpha = 200;
            Projectile.extraUpdates = 2;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            int minX = (int)Projectile.position.X / 16;
            int minY = (int)Projectile.position.Y / 16;
            int maxX = minX + Math.Min(Projectile.width / 16, 1);
            int maxY = minY + Math.Min(Projectile.height / 16, 1);
            int colldingTiles = 0;
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        colldingTiles++;
                    }
                }
            }
            if (colldingTiles > 8)
            {
                Projectile.velocity *= 0.97f - (colldingTiles - 8) * 0.01f;
            }
            if (Projectile.numUpdates == -1)
                PushEntites();
            DoDust();
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 6;
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 2 + (255 - Projectile.alpha) / 14;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.scale += 0.01f;
        }
        public virtual void PushEntites()
        {
            var myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.immortal &&
                    Projectile.Colliding(myRect, npc.getRect()) &&
                    PushableEntitiesDatabase.NPCIDs.Contains(Main.npc[i].type))
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.01f * npc.knockBackResist);
                    npc.netUpdate = true;
                }
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var proj = Main.projectile[i];
                if (i != Projectile.whoAmI && proj.active &&
                    Projectile.Colliding(myRect, proj.getRect()) &&
                    PushableEntitiesDatabase.ProjectileIDs.Contains(Main.projectile[i].type))
                {
                    proj.velocity = Vector2.Lerp(proj.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.01f);
                    proj.netUpdate = true;
                }
            }
            for (int i = 0; i < Main.maxItems; i++)
            {
                var item = Main.item[i];
                if (item.active &&
                    Projectile.Colliding(myRect, item.getRect()))
                {
                    item.velocity = Vector2.Lerp(item.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.015f);
                }
            }
            if (PushPlayers)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var player = Main.player[i];
                    if (i != Projectile.owner && player.active && !player.noKnockback)
                    {
                        if (OnlyPushHostilePlayers && (!player.hostile || player.team == 0 || player.team == Main.player[Projectile.owner].team))
                        {
                            continue;
                        }
                        if (Projectile.Colliding(myRect, player.getRect()))
                        {
                            player.velocity = Vector2.Lerp(player.velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.01f);
                        }
                    }
                }
            }
            myRect.Inflate(30, 30);
            if (Main.netMode != NetmodeID.Server)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                for (int i = dustPushIndex; i < Main.maxDust; i++)
                {
                    if (Main.dust[i].active && Main.dust[i].scale <= 3f && myRect.Contains(Main.dust[i].position.ToPoint()))
                    {
                        Main.dust[i].scale = Math.Max(Main.dust[i].scale, 0.2f);
                        Main.dust[i].velocity = Vector2.Lerp(Main.dust[i].velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.1f);
                        if (stopWatch.ElapsedMilliseconds >= 1)
                        {
                            dustPushIndex = 1;
                            break;
                        }
                    }
                }
                dustPushIndex = 0;
                for (int i = 0; i < Main.maxGore; i++)
                {
                    if (Main.gore[i].active && Main.gore[i].scale <= 3f && myRect.Contains(Main.gore[i].position.ToPoint()))
                    {
                        Main.gore[i].timeLeft = Math.Max(Main.gore[i].timeLeft, 60);
                        Main.gore[i].velocity = Vector2.Lerp(Main.gore[i].velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.1f);
                    }
                }
                for (int i = 0; i < Main.maxRain; i++)
                {
                    if (Main.rain[i].active && myRect.Contains(Main.rain[i].position.ToPoint()))
                    {
                        Main.rain[i].velocity = Vector2.Lerp(Main.rain[i].velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.1f);
                    }
                }
                for (int i = 0; i < Main.maxCombatText; i++)
                {
                    if (Main.combatText[i].active && myRect.Contains(Main.combatText[i].position.ToPoint()))
                    {
                        Main.combatText[i].lifeTime = Math.Max(Main.combatText[i].lifeTime, 90);
                        Main.combatText[i].velocity = Vector2.Lerp(Main.combatText[i].velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.1f);
                    }
                }
                for (int i = 0; i < Main.maxItemText; i++)
                {
                    if (Main.popupText[i].active && myRect.Contains(Main.popupText[i].position.ToPoint()))
                    {
                        Main.popupText[i].lifeTime = Math.Max(Main.popupText[i].lifeTime, 90);
                        Main.popupText[i].velocity = Vector2.Lerp(Main.popupText[i].velocity, Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length(), Projectile.knockBack * 0.1f);
                    }
                }
                stopWatch.Stop();
            }
        }
        public virtual void DoDust()
        {
            if (Main.rand.NextBool(10))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, Color.White.UseA(128) * 0.5f, Scale: 0.75f);
                d.velocity = new Vector2(-Projectile.velocity.X * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f) + Main.windSpeedCurrent, -Projectile.velocity.Y * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f));
                d.fadeIn = 1.5f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.width);
            writer.Write(Projectile.height);
            writer.Write(Projectile.extraUpdates);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.width = reader.ReadInt32();
            Projectile.height = reader.ReadInt32();
            Projectile.extraUpdates = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
            float scale = Projectile.scale;

            if (Projectile.timeLeft > 40)
                scale *= Projectile.Opacity;

            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, new Color(128, 128, 128, 0) * Projectile.Opacity, Projectile.rotation,
                origin, new Vector2(scale * 1.3f, scale * 0.9f), SpriteEffects.None, 0);
            return false;
        }
    }
}