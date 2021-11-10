using AQMod.Assets.DrawCode;
using AQMod.Assets.DrawCode.DrawTypes;
using AQMod.Assets.DrawCode.ParticlesLayers;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Particles;
using AQMod.Effects.HotAndColdCurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public sealed class FriendlyWind : ModProjectile
    {
        public static int NewWind(Player player, Vector2 position, Vector2 velocity, float windSpeed, int lifeSpan = 300, int size = 40)
        {
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<FriendlyWind>(), 0, windSpeed, player.whoAmI);
            Main.projectile[p].width = size;
            Main.projectile[p].height = size;
            Main.projectile[p].Center = position;
            Main.projectile[p].timeLeft = lifeSpan;
            return p;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            //projectile.hide = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            int minX = (int)projectile.position.X / 16;
            int minY = (int)projectile.position.Y / 16;
            int maxX = minX + Math.Min(projectile.width / 16, 1);
            int maxY = minY + Math.Min(projectile.height / 16, 1);
            int colldingTiles = 0;
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (Main.tile[i, j].active() && AQWorldGen.ActiveAndFullySolid(Main.tile[i, j]))
                    {
                        colldingTiles++;
                    }
                }
            }
            if (colldingTiles > 8)
            {
                projectile.velocity *= 0.97f - ((colldingTiles - 8) * 0.01f);
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var target = Main.npc[i];
                if (target.active)
                {
                    var aQNPC = target.GetGlobalNPC<AQNPC>();
                    if (aQNPC.ShouldApplyWindMechanics(target) && projectile.getRect().Intersects(target.getRect()))
                    {
                        aQNPC.ApplyWindMechanics(target, Vector2.Normalize(projectile.velocity) * projectile.knockBack);
                    }
                }
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var target = Main.projectile[i];
                if (i != projectile.whoAmI && target.active)
                {
                    var aQProjectile = target.GetGlobalProjectile<AQProjectile>();
                    if (aQProjectile.ShouldApplyWindMechanics(target) && projectile.Colliding(projectile.getRect(), target.getRect()))
                    {
                        aQProjectile.ApplyWindMechanics(target, Vector2.Normalize(projectile.velocity) * projectile.knockBack);
                    }
                }
            }
            if (Main.netMode != NetmodeID.Server && AQMod.GameWorldActive && Main.rand.NextBool(3))
            {
                var rect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                var velocity = new Vector2(-projectile.velocity.X + Main.rand.NextFloat(-1f, 1f) + Main.windSpeed, -projectile.velocity.Y + Main.rand.NextFloat(-1f, 1f));
                ParticleLayers.AddParticle_PostDrawPlayers(
                    new MonoParticle(dustPos, velocity * 0.5f,
                    new Color(0.5f, 0.5f, 0.5f, 0f), Main.rand.NextFloat(0.2f, 1.2f)));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            HotAndColdCurrentLayer.AddToHotCurrentList(new testdraw(ModContent.GetTexture("AQMod/Assets/Textures/debugtextures/hotcur")));
            HotAndColdCurrentLayer.AddToColdCurrentList(new ArrowDraw(Color.Blue, projectile.oldPos.AsAddAll(-Main.screenPosition)));
            return false;
        }
    }
}