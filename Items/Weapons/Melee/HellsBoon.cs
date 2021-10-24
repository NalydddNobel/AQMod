using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(CommonUtils.GetPath(this) + "_Glow", new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 22;
            item.useTime = 38;
            item.useAnimation = 19;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shootSpeed = 35f;
            item.shoot = ModContent.ProjectileType<HellsBoonSpike>();
            item.scale = 1.25f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.GetAlphaDemonSiegeWeapon(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            HellsBoonSpike.SpawnCluster(Main.MouseWorld, (int)(item.shootSpeed / player.meleeSpeed), damage, knockBack, player);
            return false;
        }
    }

    public class HellsBoonSpike : ModProjectile
    {
        private float _portaloffset = 0f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
        }

        private const int goOutTime = 15;

        public override bool? CanCutTiles() => projectile.ai[1] > 15f && projectile.ai[1] < 15f + goOutTime;

        public override void AI()
        {
            if (projectile.frameCounter == 0)
            {
                projectile.frameCounter++;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;
                _portaloffset = Main.rand.NextFloat(-0.1f, 0.1f);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = projectile.position.X;
                projectile.localAI[1] = projectile.position.Y;
            }
            if (projectile.ai[1] > 15f)
            {
                if (projectile.ai[1] < 15f + goOutTime)
                {
                    projectile.ai[1] += Main.rand.NextFloat(1f, 2.5f);
                    if ((int)projectile.ai[1] >= 15 + goOutTime)
                    {
                        Main.PlaySound(new LegacySoundStyle(SoundID.Tink, 0, Terraria.Audio.SoundType.Sound).WithVolume(0.5f).WithPitchVariance(2f), projectile.Center);
                    }
                    float progress = (projectile.ai[1] - 15f) / goOutTime;
                    projectile.ai[0] = MathHelper.Lerp(0f, 45f, progress);
                    projectile.timeLeft += Main.rand.Next(6) - 1;
                }
            }
            else
            {
                projectile.ai[1] += Main.rand.NextFloat(0f, 1f);
                if (Main.rand.NextBool(6))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 36 : 17);
                    Main.dust[d].velocity = projectile.velocity * 2f;
                    Main.dust[d].noGravity = true;
                }
            }
            projectile.position = new Vector2(projectile.localAI[0] + projectile.velocity.X * projectile.ai[0], projectile.localAI[1] + projectile.velocity.Y * projectile.ai[0]);
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(new LegacySoundStyle(SoundID.Dig, 0, Terraria.Audio.SoundType.Sound).WithVolume(0.25f).WithPitchVariance(2f), projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 36 : 17);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            int frameWidth = texture.Width / Main.projFrames[projectile.type];
            int frameHeight = (int)projectile.ai[0] + 8;
            if (frameHeight > texture.Height)
            {
                frameHeight = texture.Height;
            }
            var frame = new Rectangle(frameWidth * projectile.frame, 0, frameWidth - 2, frameHeight);
            var orig = new Vector2(frameWidth / 2f, 16f);
            var effects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            bool resetBatch = false;
            var drawData = new DrawData(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation + MathHelper.PiOver2, orig, projectile.scale, SpriteEffects.None, 0);
            if (AQConfigClient.Instance.SpotlightShader)
            {
                resetBatch = true;
                drawData.sourceRect = new Rectangle(frame.X, frame.Y, frame.Width, texture.Height);
                Main.spriteBatch.End();
                BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                var effect = GameShaders.Misc["AQMod:SpikeFade"];
                var sampler = TextureCache.Pixel.Value;
                effect.UseOpacity(1f / texture.Height * frameHeight + _portaloffset);
                effect.UseColor(new Vector3(0.65f, 0.3f, 1f));
                effect.Apply(drawData);
            }
            drawData.Draw(Main.spriteBatch); // super very smart :)
            drawData.Draw(Main.spriteBatch);
            drawData.Draw(Main.spriteBatch);
            if (resetBatch)
            {
                Main.spriteBatch.End();
                BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
            }
            return false;
        }

        public static void SpawnCluster(Vector2 position, int size, int damage, float knockback, Player player)
        {
            int projectileCount = size / 4;
            var tilePos = position.ToTileCoordinates();
            tilePos.Fluffize(fluff: 10);
            var sizeCorner = new Point(tilePos.X - size / 2, tilePos.Y - size / 2);
            sizeCorner.Fluffize(fluff: 10);
            List<Point> validSpots = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (AQWorldGen.ActiveAndSolid(sizeCorner.X + i, sizeCorner.Y + j))
                    {
                        if (AQWorldGen.ActiveAndSolid(sizeCorner.X + i + 1, sizeCorner.Y + j) ||
                            AQWorldGen.ActiveAndSolid(sizeCorner.X + i - 1, sizeCorner.Y + j) ||
                            AQWorldGen.ActiveAndSolid(sizeCorner.X + i, sizeCorner.Y + j + 1) ||
                            AQWorldGen.ActiveAndSolid(sizeCorner.X + i, sizeCorner.Y + j - 1))
                        {
                            var pos = new Vector2(sizeCorner.X + i - 1, sizeCorner.Y + j) * 16f + new Vector2(8f, 8f);
                            pos += Vector2.Normalize(position - pos) * 20f;
                            if (Collision.CanHitLine(position, 0, 0, pos, 0, 0))
                                validSpots.Add(new Point(sizeCorner.X + i, sizeCorner.Y + j));
                        }
                    }
                }
            }
            if (validSpots.Count <= 0)
            {
                return;
            }
            if (validSpots.Count < projectileCount)
            {
                projectileCount = validSpots.Count;
            }
            int type = ModContent.ProjectileType<HellsBoonSpike>();
            for (int i = 0; i < projectileCount - 1; i++)
            {
                int random = Main.rand.Next(validSpots.Count);
                var spawnPosition = new Vector2(validSpots[random].X * 16 + 8f, validSpots[random].Y * 16f + 8f);
                Projectile.NewProjectile(spawnPosition, Vector2.Normalize(position - spawnPosition), type, damage, knockback, player.whoAmI);
            }
            if (validSpots.Count == 1)
            {
                var spawnPosition = new Vector2(validSpots[0].X * 16 + 8f, validSpots[0].Y * 16f + 8f);
                Projectile.NewProjectile(spawnPosition, Vector2.Normalize(position - spawnPosition), type, damage, knockback, player.whoAmI);
            }
            else
            {
                int random = Main.rand.Next(validSpots.Count);
                var spawnPosition = new Vector2(validSpots[random].X * 16 + 8f, validSpots[random].Y * 16f + 8f);
                Projectile.NewProjectile(spawnPosition, Vector2.Normalize(position - spawnPosition), type, damage, knockback, player.whoAmI);
            }
        }
    }
}