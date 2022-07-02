using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords
{
    public sealed class MirrorsCallProj : SwordProjectileBaseCircular
    {
        public override string Texture => AequusHelpers.GetPath<MirrorsCall>();

        public override float Radius => 104.65f;
        public override float VisualHoldout => 8f;
        public override float HitboxHoldout => 45f;
        public override float AltFunctionSpeedup => 0.6f;
        public override float AltFunctionScale => 2f;
        public override float AltFunctionHitboxScale => 1.25f;

        public SwordSlashPrimRenderer prim;
        public SwordSlashPrimRenderer colorPrim;
        public float colorProgress;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 48;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 216;
            Projectile.height = 216;
            Projectile.extraUpdates = 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            return SwingProgress > 0.3f && damageTime < 8;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                bool rightClick = Main.player[Projectile.owner].altFunctionUse == 2;
                float explosionScale = !rightClick ? 0.66f : 1f;
                AequusEffects.Shake.Set(8f * explosionScale);
                MirrorsCallExplosion.ExplosionEffects(Projectile.owner, target.Center, colorProgress, explosionScale);
                float damageScale = !rightClick ? 0.8f : 1.8f;
                int p = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center,
                    Vector2.Normalize(target.Center - Main.player[Projectile.owner].Center), ModContent.ProjectileType<MirrorsCallExplosion>(), (int)(Projectile.damage * damageScale), Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = explosionScale;
                Main.projectile[p].width = (int)(Main.projectile[p].width * explosionScale);
                Main.projectile[p].height = (int)(Main.projectile[p].height * explosionScale);
                Main.projectile[p].Center = target.Center;
                Main.projectile[p].ModProjectile<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }

        public override void Initalize(Player player, AequusPlayer aequusPlayer)
        {
            base.Initalize(player, aequusPlayer);
            int direction = -Projectile.direction;
            if (combo > 0)
            {
                direction = -direction;
            }
            angleVector = BaseAngleVector.RotatedBy(MathHelper.PiOver2 * -direction);
            colorProgress = Main.rand.NextFloat(0f, 8f);
        }

        protected override void UpdateSwing(Player player, AequusPlayer aequus)
        {
            int direction = -Projectile.direction;
            if (combo > 0)
            {
                direction = -direction;
            }
            else if (damageTime == 1 && Main.myPlayer == player.whoAmI && player.altFunctionUse != 2)
            {
                int damage = (int)(Projectile.damage * 0.25f);
                var source = Projectile.GetSource_FromAI();
                int p = Projectile.NewProjectile(source, player.Center, Projectile.velocity * 20f, ModContent.ProjectileType<MirrorsCallBullet>(), damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].ModProjectile<MirrorsCallBullet>().colorProgress = colorProgress;
                p = Projectile.NewProjectile(source, player.Center, Projectile.velocity.RotatedBy(-0.2f) * 20f, ModContent.ProjectileType<MirrorsCallBullet>(), damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].ModProjectile<MirrorsCallBullet>().colorProgress = colorProgress + Main.rand.NextFloat(0.2f);
                p = Projectile.NewProjectile(source, player.Center, Projectile.velocity.RotatedBy(0.2f) * 20f, ModContent.ProjectileType<MirrorsCallBullet>(), damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].ModProjectile<MirrorsCallBullet>().colorProgress = colorProgress - Main.rand.NextFloat(0.2f);
            }
            float rotationSpeed = MathHelper.Pi * swing * swingMultiplier * direction;
            angleVector = AngleVector.RotatedBy(rotationSpeed);

            colorProgress += Main.rand.NextFloat(-0.2f, 0.2f);

            UpdateSwing_Dust(player, direction);
        }
        private void UpdateSwing_Dust(Player player, int direction)
        {
            int dustChance = (int)(3 / Projectile.scale);
            if (Main.netMode != NetmodeID.Server && (dustChance <= 1 || Main.rand.NextBool(dustChance)))
            {
                var d = Dust.NewDustDirect(player.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, AequusHelpers.GetRainbowHue(Projectile, colorProgress).UseA(0) * Main.rand.NextFloat(0.5f, 1.25f), Main.rand.NextFloat(0.75f, 1.5f));
                d.position = player.Center + angleVector * Main.rand.NextFloat(20f, Radius * Projectile.scale);
                d.velocity *= 0.1f;
                d.velocity += AngleVector.RotatedBy(MathHelper.PiOver2 * direction + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(1.5f, 3.5f);
            }
        }

        protected override void OnReachMaxProgress()
        {
            if (swingMultiplier < 0.05f)
            {
                base.OnReachMaxProgress();
                Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            else
            {
                //Main.NewText(Main.player[Projectile.owner].meleeSpeed);
                swingMultiplier *= 0.9f - (Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee) - 1f) / 8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var origin = new Vector2(0f, texture.Height);
            var center = Main.player[Projectile.owner].Center;
            var handPosition = center + AngleVector * VisualHoldout;
            var drawColor = Projectile.GetAlpha(lightColor);
            var drawCoords = handPosition - Main.screenPosition;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            float size = texture.Size().Length();
            var effects = SpriteEffects.None;
            float trailOutwards = texture.Size().Length() * Projectile.scale - 38f * Projectile.scale;
            bool reverseTrail = Projectile.direction == -1 ? combo > 0 : combo == 0;
            var oldPos = Array.ConvertAll(Projectile.oldPos, (v) => Vector2.Normalize(v) * trailOutwards);
            if (Main.LocalPlayer.gravDir == -1)
            {
                for (int i = 0; i < oldPos.Length; i++)
                {
                    oldPos[i].Y = -oldPos[i].Y;
                }
            }
            if (prim == null)
            {
                prim = new SwordSlashPrimRenderer(TextureAssets.Extra[ExtrasID.EmpressBladeTrail].Value, TrailRenderer.DefaultPass, (p) => new Vector2(40f) * Projectile.scale, (p) => new Color(255, 255, 255, 0) * (1f - p));
            }
            if (colorPrim == null)
            {
                colorPrim = new SwordSlashPrimRenderer(TextureAssets.Extra[ExtrasID.EmpressBladeTrail].Value, TrailRenderer.DefaultPass, (p) => new Vector2(40f) * Projectile.scale, (p) => AequusHelpers.GetRainbowHue(Projectile, colorProgress).UseA(0) * (1f - p));
            }
            if (reverseTrail)
            {
                prim.coord1 = 0f;
                prim.coord2 = 1f;
                colorPrim.coord1 = 0f;
                colorPrim.coord2 = 1f;
            }
            else
            {
                prim.coord1 = 1f;
                prim.coord2 = 0f;
                colorPrim.coord1 = 1f;
                colorPrim.coord2 = 0f;
            }
            prim.drawOffset = center;
            colorPrim.drawOffset = center;
            prim.Draw(oldPos);
            colorPrim.Draw(oldPos);

            float intensity = 0f;
            if (SwingProgress > 0.25f && SwingProgress < 0.75f)
            {
                intensity = (float)Math.Sin((SwingProgress - 0.25f) * 2f * MathHelper.Pi);
            }

            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, texture, handPosition - Main.screenPosition, null, Projectile.rotation, origin, Projectile.scale, effects, rainbowOffsetScaleMultiplier: 4f + 16f * intensity);
            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, texture, handPosition - Main.screenPosition, null, Projectile.rotation, origin, Projectile.scale, effects, drawWhite: false, rainbowOffsetScaleMultiplier: 4f + 16f * intensity);

            //if (SwingProgress > 0.25f && SwingProgress < 0.75f)
            //{
            //    Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            //    Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            //    var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
            //    var shineOrigin = shine.Size() / 2f;
            //    var shineLocation = handPosition - Main.screenPosition + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((size - 8f) * Projectile.scale);
            //    MirrorsCall.DrawRainbowAura(Main.spriteBatch, shine, shineLocation, null, 0f, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, opacity: intensity * intensity);
            //    MirrorsCall.DrawRainbowAura(Main.spriteBatch, shine, shineLocation, null, MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, opacity: intensity * intensity);
            //}

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(colorProgress);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            colorProgress = reader.ReadSingle();
        }
    }

    public sealed class MirrorsCallBullet : ModProjectile
    {
        public float colorProgress;

        public override string Texture => "Aequus/Assets/Bullet";
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            var target = Projectile.FindTargetWithinRange(400f);
            if (target != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * 20f, 0.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                AequusEffects.Shake.Set(4f);
                MirrorsCallExplosion.ExplosionEffects(Projectile.owner, target.Center, colorProgress, 0.75f);
                int p = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center,
                    Vector2.Normalize(target.Center - Main.player[Projectile.owner].Center), ModContent.ProjectileType<MirrorsCallExplosion>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = 0.4f;
                Main.projectile[p].width = (int)(Main.projectile[p].width * 0.4f);
                Main.projectile[p].height = (int)(Main.projectile[p].height * 0.4f);
                Main.projectile[p].Center = target.Center;
                Main.projectile[p].ModProjectile<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var bloom = TextureCache.Bloom[0].Value;
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, bloom, drawCoordinates, null, 0f, bloom.Size() / 2f, Projectile.scale * 0.55f, opacity: 0.5f, drawWhite: false, rainbowScaleMultiplier: 0.6f, rainbowOffsetScaleMultiplier: 8f);
            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, bloom, drawCoordinates, null, 0f, bloom.Size() / 2f, Projectile.scale * 0.7f, opacity: 0.25f, drawWhite: false, rainbowScaleMultiplier: 0.75f, rainbowOffsetScaleMultiplier: 8f);
            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, texture, drawCoordinates, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale);
            MirrorsCall.DrawRainbowAura(Projectile.owner, Main.spriteBatch, texture, drawCoordinates, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale, opacity: 0.5f, drawWhite: false);
            return false;
        }
    }

    public sealed class MirrorsCallExplosion : ModProjectile
    {
        public float colorProgress;

        public override string Texture => Aequus.BlankTexture;
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.timeLeft = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
            Projectile.penetrate = -1;
        }

        public static void ExplosionEffects(int player, Vector2 location, float colorProgress, float scale)
        {
            int amt = (int)(90 * scale);
            for (int i = 0; i < amt; i++)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(40 * scale);
                var d = Dust.NewDustPerfect(location + v, ModContent.DustType<MonoDust>(), v / 2.5f, 0, AequusHelpers.GetRainbowHue(player, colorProgress + Main.rand.NextFloat(-0.2f, 0.2f)).UseA(0) * Main.rand.NextFloat(0.6f, 1.1f) * scale, Main.rand.NextFloat(0.8f, 1.8f));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner && Projectile.scale >= 0.4f)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center,
                    Vector2.Normalize(Projectile.Center - Main.player[Projectile.owner].Center), Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = Projectile.scale - 0.2f;
                Main.projectile[p].ModProjectile<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }
    }
}