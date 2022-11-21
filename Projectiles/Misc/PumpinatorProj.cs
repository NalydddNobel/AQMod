using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class PumpinatorProj : ModProjectile
    {
        public virtual bool PushPlayers => true;
        public virtual bool OnlyPushHostilePlayers => false;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
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
            var myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.immortal &&
                    Projectile.Colliding(myRect, npc.getRect()) &&
                    PushableEntitiesDatabase.NPCIDs.Contains(Main.npc[i].type))
                {
                    npc.velocity += Vector2.Normalize(Projectile.velocity) * Projectile.knockBack / 30f * npc.knockBackResist;
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
                    proj.velocity += Vector2.Normalize(Projectile.velocity) * Projectile.knockBack;
                    proj.netUpdate = true;
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
                            player.velocity += Vector2.Normalize(Projectile.velocity) * Projectile.knockBack * 0.025f;
                    }
                }
            }
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
        public virtual void DoDust()
        {
            if (Main.rand.NextBool(10))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, Color.White.UseA(128) * 0.5f);
                d.velocity = new Vector2(-Projectile.velocity.X * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f) + Main.windSpeedCurrent, -Projectile.velocity.Y * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f));
                d.fadeIn = d.scale + 1f;
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