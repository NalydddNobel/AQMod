using Aequus;
using Aequus.Common.Utilities.Drawing;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class HellsBoonProj : ModProjectile
    {
        private const int goOutTime = 15;

        private float _portaloffset = 0f;

        public static StaticMiscShaderInfo SpikeFade { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SpikeFade = new StaticMiscShaderInfo("MiscEffects", "Aequus:SpikeFade", "SpikeFadePass", true);
            }
        }

        public override void Unload()
        {
            SpikeFade = null;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override bool? CanCutTiles() => Projectile.ai[1] > 15f && Projectile.ai[1] < 15f + goOutTime;

        public override void AI()
        {
            if (Projectile.frameCounter == 0)
            {
                Projectile.frameCounter++;
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
                Projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;
                _portaloffset = Main.rand.NextFloat(-0.1f, 0.1f);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Projectile.position.X;
                Projectile.localAI[1] = Projectile.position.Y;
            }
            if (Projectile.ai[1] > 15f)
            {
                if (Projectile.ai[1] < 15f + goOutTime)
                {
                    Projectile.ai[1] += Main.rand.NextFloat(1f, 2.5f);
                    if (Main.netMode != NetmodeID.Server && (int)Projectile.ai[1] >= 15 + goOutTime)
                    {
                        SoundEngine.PlaySound(SoundID.Tink.WithVolume(0.5f).WithPitchOffset(0.05f), Projectile.Center);
                    }
                    float progress = (Projectile.ai[1] - 15f) / goOutTime;
                    Projectile.ai[0] = MathHelper.Lerp(0f, 80f, progress);
                    Projectile.timeLeft += Main.rand.Next(12);
                }
            }
            else
            {
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
                }
                Projectile.ai[1] += Main.rand.NextFloat(1f) + 0.01f;
                if (Main.rand.NextBool(6))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 36 : 17);
                    Main.dust[d].velocity = Projectile.velocity * 2f;
                    Main.dust[d].noGravity = true;
                }
            }
            Projectile.position = new Vector2(Projectile.localAI[0] + Projectile.velocity.X * Projectile.ai[0], Projectile.localAI[1] + Projectile.velocity.Y * Projectile.ai[0]);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            SoundEngine.PlaySound(SoundID.Dig.WithVolume(0.25f).WithPitch(0.5f), Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(90f), Projectile.width, Projectile.height, Main.rand.NextBool() ? 36 : 17, Scale: Main.rand.NextFloat(0.6f, 1f));
                if (Main.rand.NextBool())
                {
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            int frameWidth = texture.Width / Main.projFrames[Projectile.type];
            int frameHeight = (int)Projectile.ai[0] + 8;
            if (frameHeight > texture.Height)
            {
                frameHeight = texture.Height;
            }
            var drawData = new DrawData(texture, Projectile.Center - Main.screenPosition, new Rectangle(frameWidth * Projectile.frame, 0, frameWidth - 2, frameHeight), lightColor.MaxRGBA(200), Projectile.rotation + MathHelper.PiOver2, new Vector2(frameWidth / 2f, 16f), Projectile.scale, SpriteEffects.None, 0);
            if (Projectile.ai[0] > 42f)
            {
                drawData.scale.Y += (Projectile.ai[0] - 42f) / 66f;
            }

            if (Aequus.HQ)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin_World(shader: true);
                var effect = SpikeFade.ShaderData;
                effect.UseOpacity(1f / texture.Height * frameHeight + _portaloffset);
                effect.UseColor(new Vector3(0.65f, 0.3f, 1f));
                effect.Apply(drawData);
                drawData.Draw(Main.spriteBatch);
                drawData.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin_World(shader: false);;
            }
            else
            {
                float alpha = 0f;
                if (Projectile.ai[1] > 15f)
                {
                    if (Projectile.ai[1] < 15f + goOutTime)
                    {
                        alpha = (Projectile.ai[1] - 15f) / goOutTime;
                    }
                    else
                    {
                        alpha = 1f;
                    }
                }
                drawData.color *= alpha;
                drawData.Draw(Main.spriteBatch);
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }

    public class HellsBoonSpawner : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hide = true;
            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }
            int size = (int)(35f * Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee));
            int projectileCount = size / 4;
            var tilePos = Projectile.Center.ToTileCoordinates();
            tilePos.Fluffize(fluff: 10);
            var sizeCorner = new Point(tilePos.X - size / 2, tilePos.Y - size / 2);
            sizeCorner.Fluffize(fluff: 10);
            var validSpots = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int tileX = sizeCorner.X + i;
                    int tileY = sizeCorner.Y + j;
                    if (Main.tile[tileX, tileY].IsSolid())
                    {
                        if (Main.tile[tileX + 1, tileY].IsSolid() &&
                            Main.tile[tileX - 1, tileY].IsSolid() &&
                            Main.tile[tileX, tileY + 1].IsSolid() &&
                            Main.tile[tileX, tileY - 1].IsSolid())
                        {
                            continue;
                        }
                        var pos = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
                        pos += Vector2.Normalize(Projectile.Center - pos) * 18f;
                        if (Collision.CanHitLine(Projectile.Center, 2, 2, pos, 0, 0))
                        {
                            validSpots.Add(new Point(tileX, tileY));
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
            var source = Projectile.GetSource_FromAI();
            for (int i = 0; i < projectileCount; i++)
            {
                int random = Main.rand.Next(validSpots.Count);
                var spawnPosition = new Vector2(validSpots[random].X * 16 + 8f, validSpots[random].Y * 16f + 8f);
                Projectile.NewProjectile(source, spawnPosition, Vector2.Normalize(Projectile.Center - spawnPosition).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<HellsBoonProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.Kill();
        }
    }
}
