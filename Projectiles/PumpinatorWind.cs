using Aequus.Common.Catalogues;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles
{
    public class PumpinatorWind : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.hide = true;
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
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.immortal &&
                    Projectile.Colliding(Projectile.getRect(), npc.getRect()) &&
                    WindMovementCatalogue.WindNPCs.Contains(Main.npc[i].type))
                {
                    npc.velocity += Vector2.Normalize(Projectile.velocity) * Projectile.knockBack / 30f;
                    npc.netUpdate = true;
                }
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var proj = Main.projectile[i];
                if (i != Projectile.whoAmI && proj.active &&
                    Projectile.Colliding(Projectile.getRect(), proj.getRect()) &&
                    WindMovementCatalogue.WindProjs.Contains(Main.projectile[i].type))
                {
                    proj.velocity += Vector2.Normalize(Projectile.velocity) * Projectile.knockBack;
                    proj.netUpdate = true;
                }
            }
            if (Main.rand.NextBool(3))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Color.White.UseA(128) * 0.5f);
                d.velocity = new Vector2(-Projectile.velocity.X + Main.rand.NextFloat(-1f, 1f) + Main.windSpeedCurrent, -Projectile.velocity.Y + Main.rand.NextFloat(-1f, 1f));
            }
            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 3;
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
    }
}