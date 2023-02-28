using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.CrabsonProjs
{
    public class CrabsonPearl : ModProjectile
    {
        public static Asset<Texture2D> WhiteTexture { get; private set; }

        private float _light;

        public int Amt => Main.expertMode ? Main.getGoodWorld ? 16 : 8 : 4;

        public override void Load()
        {
            if (!Main.dedServ)
                WhiteTexture = ModContent.Request<Texture2D>(this.GetPath() + "_White");
        }

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.scale = 0.6f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            if (Main.getGoodWorld)
            {
                Projectile.scale = 1f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            int explodeTime = 16;
            Projectile.localAI[0] += 0.016f;
            if (Projectile.timeLeft < explodeTime)
            {
                if (!Main.getGoodWorld)
                {
                    Projectile.tileCollide = true;
                }
                _light = 1f - Projectile.timeLeft / explodeTime;
                var center = Projectile.Center;
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(Projectile.Size.Length() * 1.2f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (center - Main.dust[d].position) / 8f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.2f, 0.6f));
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].position.Y
                > Projectile.position.Y + Projectile.height;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 2f)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 1.1f;
                if (Projectile.wet)
                {
                    if (Projectile.velocity.Y < -16f)
                        Projectile.velocity.Y = -16f;
                }
                else
                {
                    if (Projectile.velocity.Y < -8f)
                        Projectile.velocity.Y = -8f;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                    }
                }
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Shatter, center);
            float dustRadius = Projectile.Size.Length() * 0.6f;
            for (int i = 0; i < 35; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) / 2f;
            }
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10), Main.rand.NextFloat(1.5f, 2f));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) / 3f;
            }
            if ((int)Projectile.ai[1] == 1)
            {
                float add = MathHelper.TwoPi / Amt;
                float speed = 10f;
                if (Main.getGoodWorld)
                {
                    speed *= 2f;
                }
                for (float r = 0; r <= MathHelper.TwoPi; r += add + 0.001f)
                {
                    var normal = (r + Main.rand.NextFloat(-0.01f, 0.01f)).ToRotationVector2();
                    int p = Projectile.NewProjectile(new EntitySource_Parent(Projectile), center + normal * 20f, normal * speed, ModContent.ProjectileType<CrabsonPearlShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[p].timeLeft += Main.rand.Next(-10, 10);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Projectile.type].Value;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            var origin = texture.Size() / 2f;
            var drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.position + offset, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            if ((int)Projectile.ai[1] == 1)
            {
                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var line = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                float telegraphThingy = (float)Math.Pow(Projectile.localAI[0], 3f);
                for (int i = 0; i < Amt; i++)
                {
                    float r = Projectile.rotation + MathHelper.TwoPi / Amt * i;
                    Main.EntitySpriteDraw(line, Projectile.position + offset + r.ToRotationVector2() * 20f * Projectile.scale, null, Color.LightBlue.UseA(5) * telegraphThingy,
                        r + MathHelper.PiOver2, line.Size() / 2f, new Vector2(Projectile.scale * telegraphThingy, Projectile.scale * telegraphThingy * 4f), SpriteEffects.None, 0);
                }
            }
            if (_light > 0f)
            {
                Main.EntitySpriteDraw(WhiteTexture.Value, Projectile.position + offset, null, Color.White * _light, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}