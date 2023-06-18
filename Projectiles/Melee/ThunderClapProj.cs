using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee {
    public class ThunderClapProj : ModProjectile
    {
        public static Asset<Texture2D> ChainTexture { get; private set; }

        public Vector2 targetVector;
        private bool _didEffects;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                ChainTexture = ModContent.Request<Texture2D>(this.GetPath() + "Chain", AssetRequestMode.ImmediateLoad);
            }
        }

        public override void SetStaticDefaults()
        {
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void Unload()
        {
            ChainTexture = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 250;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;

            if (!player.frozen && !player.stoned)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    AI_Init();
                }
                float multiplier = 0.45f;
                if (Projectile.ai[0] < 0.1f)
                {
                    multiplier *= 0.1f + Projectile.ai[0] / 0.1f * 0.9f;
                }
                if (Projectile.ai[0] < MathHelper.Pi)
                {
                    multiplier *= (1f - Projectile.ai[0] / MathHelper.Pi) * 0.95f + 0.05f;
                }
                Projectile.ai[0] += player.GetAttackSpeed(DamageClass.Melee) * multiplier;
                if (Projectile.ai[0] > MathHelper.TwoPi)
                {
                    if (Projectile.alpha < 255)
                    {
                        Projectile.alpha += 20;
                        if (Projectile.alpha > 255)
                        {
                            Projectile.alpha = 255;
                        }
                    }
                    if (Projectile.ai[0] > MathHelper.TwoPi + 3f)
                    {
                        Projectile.Kill();
                    }
                    if (!_didEffects)
                    {
                        var targetPos = Projectile.Center + targetVector;
                        if (Main.netMode != NetmodeID.Server)
                        {
                            foreach (var v in Helper.CircularVector(40))
                            {
                                var d = Dust.NewDustPerfect(targetPos, ModContent.DustType<MonoDust>(), v * 10f, 0, new Color(222, 150, 20, 50));
                                d.scale = Main.rand.NextFloat(0.9f, 1.35f);
                            }
                            for (int i = 0; i < 30; i++)
                            {
                                var d = Dust.NewDustPerfect(targetPos, DustID.Torch, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(8f, 13.5f));
                                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                                d.noGravity = true;
                            }
                            for (int i = 0; i < 18; i++)
                            {
                                var d = Dust.NewDustPerfect(targetPos, DustID.Smoke, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(2f, 8f));
                                d.scale = Main.rand.NextFloat(1.4f, 2f);
                                d.noGravity = true;
                            }
                            SoundEngine.PlaySound(SoundID.Item14, targetPos);
                            SoundEngine.PlaySound(Aequus.GetSounds("RedSprite/thunderClap", 2), targetPos);
                        }
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), targetPos, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<ThunderClapExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Projectile.netUpdate = true;
                        }
                        _didEffects = true;
                    }
                }
                else
                {
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha -= 20;
                        if (Projectile.alpha < 0)
                        {
                            Projectile.alpha = 0;
                        }
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        targetVector = Vector2.Lerp(targetVector, new Vector2(Main.mouseX - Main.screenWidth / 2f, Main.mouseY - Main.screenHeight / 2f), Math.Min(0.015f + player.velocity.Length() * 0.001f, 0.2f));
                    }
                }
                Projectile.hide = false;
            }

            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            player.ChangeDir(Projectile.direction);
        }
        private void AI_Init()
        {
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Helper.ScaleUp(Projectile);

            if (Main.myPlayer == Projectile.owner)
            {
                targetVector.X = Main.mouseX - Main.screenWidth / 2f;
                targetVector.Y = Main.mouseY - Main.screenHeight / 2f;
                float maxLength = 240f * Projectile.scale;
                if (targetVector.Length() > maxLength)
                {
                    targetVector = Vector2.Normalize(targetVector) * maxLength;
                }
            }
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = texture.Size() / 2f;
            float progress = Math.Min(Projectile.ai[0] / MathHelper.TwoPi, 1f);
            DrawHand(texture, frame, origin, 1, progress);
            DrawHand(texture, frame, origin, -1, progress);
            return false;
        }
        public void DrawHand(Texture2D texture, Rectangle frame, Vector2 origin, int dir, float progress)
        {
            var startLocation = Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir)) * 200f;
            var endLocation = Projectile.Center + targetVector + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir)) * Math.Max(200f * (1f - progress), 8f);

            var drawPos = Vector2.Lerp(startLocation, endLocation, progress);
            float rotation = Projectile.rotation;
            if (progress > 0.5f)
            {
                rotation += MathHelper.PiOver2 * -dir * (progress - 0.5f) * 2f;
            }
            var effects = dir == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            DrawArmsTo(drawPos - rotation.ToRotationVector2() * -dir * origin.X, dir, rotation, progress);

            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frame, Color.White * Projectile.Opacity, rotation, origin, Projectile.scale * 1.35f, effects, 0);
        }
        public void DrawArmsTo(Vector2 to, int dir, float rotation, float progress)
        {
            var texture = ChainTexture.Value;
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var velocityVector = Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir));
            var position = Projectile.Center + velocityVector * 20f;
            float speed = texture.Width * Projectile.scale;

            var to2 = to - rotation.ToRotationVector2() * -dir * 40f;
            float distance = Vector2.Distance(position, to2);
            for (int i = 0; i < 250 && distance > texture.Width; i++)
            {
                if (i > 3)
                {
                    var color = Color.White;
                    if (i - 3 < 20)
                    {
                        float progress2 = 1f - Helper.CalcProgress(20 + 1, i + 1 - 3);
                        if (distance < texture.Width * 6f)
                        {
                            progress2 = MathHelper.Lerp(progress2, 1f, 1f - distance / (texture.Width * 6f));
                        }
                        color *= progress2 * progress2;
                    }

                    Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, color * Projectile.Opacity, velocityVector.ToRotation(), origin, Projectile.scale, SpriteEffects.None, 0);
                }
                distance = Vector2.Distance(position, to);
                position += velocityVector * speed;
                float lerpAmount = 0.3f + Math.Max((i - 25) * 0.01f + 0.2f * progress, 0);
                if (distance < texture.Width * 6 && lerpAmount < 0.9f)
                {
                    lerpAmount = Math.Min(lerpAmount * 1.5f, 0.9f);
                    to2 = to;
                }
                velocityVector = Vector2.Lerp(velocityVector, Vector2.Normalize(to2 - position), lerpAmount);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetVector);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetVector = reader.ReadVector2();
        }
    }

    public class ThunderClapExplosion : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.timeLeft = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
            Projectile.penetrate = -1;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.IsTheDestroyer())
            {
                Projectile.damage -= 10;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 480);
        }
    }
}