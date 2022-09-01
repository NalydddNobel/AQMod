using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords
{
    public abstract class SwordProjectileBase : ModProjectile
    {
        public static Asset<Texture2D> SwishTexture => ModContent.Request<Texture2D>(typeof(SwordProjectileBase).Namespace.Replace('.', '/') + "/Swish");
        public static Asset<Texture2D> Swish2Texture => ModContent.Request<Texture2D>(typeof(SwordProjectileBase).Namespace.Replace('.', '/') + "/Swish2");

        public static SoundStyle HeavySwing => Aequus.GetSounds("heavysword_", 4, 0.66f, 0f, 0.2f);

        private bool _init;
        public int swingDirection;
        public int hitboxOutwards;
        public int visualOutwards;
        public float rotationOffset;
        public bool forced50;
        public float scale;

        public bool playedSound;

        public bool damaging;
        public int damageTime;

        public int combo;

        private Vector2 angleVector;
        public Vector2 AngleVector { get => angleVector; set => angleVector = Vector2.Normalize(value); }
        public Vector2 BaseAngleVector => Vector2.Normalize(Projectile.velocity);
        public float AnimProgress => 1f - (Main.player[Projectile.owner].itemAnimation * (Projectile.extraUpdates + 1) + Projectile.numUpdates + 1) / (float)(Main.player[Projectile.owner].itemAnimationMax * (Projectile.extraUpdates + 1));

        public virtual bool SwingSwitchDir => AnimProgress > 0.4f && AnimProgress < 0.6f;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 50;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanDamage()
        {
            return (AnimProgress > 0.4f && AnimProgress < 0.6f) ? null : false;
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];

            if (player.itemAnimation <= 1)
            {
                Projectile.Kill();
                return;
            }

            var aequus = player.GetModPlayer<AequusPlayer>();

            player.heldProj = Projectile.whoAmI;
            if (!_init)
            {
                Initialize(player, player.Aequus());
                scale = Projectile.scale;
                Projectile.netUpdate = true;
            }

            if (SwingSwitchDir)
            {
                UpdateDirection(player);
            }

            if (!player.frozen && !player.stoned)
            {
                var arm = Main.GetPlayerArmPosition(Projectile);
                float progress = AnimProgress;
                if (!forced50 && progress >= 0.5f)
                {
                    progress = 0.5f;
                    forced50 = true;
                }
                float swingProgress = SwingProgress(progress);
                AngleVector = GetOffsetVector(swingProgress);
                Projectile.position = arm + AngleVector * hitboxOutwards;
                Projectile.position.X -= Projectile.width / 2f;
                Projectile.position.Y -= Projectile.height / 2f;
                Projectile.rotation = (arm - Projectile.Center).ToRotation() + rotationOffset;
                UpdateSwing(progress, swingProgress);
                float s = Projectile.scale;
                Projectile.scale = GetScale(swingProgress);
                visualOutwards = (int)GetVisualOuter(progress, swingProgress);
                SetArmRotation(player, AngleVector);
            }

            _init = true;
        }

        public virtual void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
        }

        public virtual float SwingProgress(float progress)
        {
            return progress;
        }
        public static float GenericSwing2(float progress, float pow = 2f)
        {
            if (progress > 0.5f)
            {
                return 0.5f - GenericSwing2(0.5f - (progress - 0.5f), pow) + 0.5f;
            }
            return ((float)Math.Sin(Math.Pow(progress, pow) * MathHelper.TwoPi - MathHelper.PiOver2) + 1f) / 2f;
        }
        public static float GenericSwing1(float progress, float pow = 2f, float startSwishing = 0.15f)
        {
            float oldProg = progress;
            float max = 1f - startSwishing;
            if (progress < startSwishing)
            {
                progress *= (float)Math.Pow(progress / startSwishing, pow);
            }
            else if (progress > max)
            {
                progress -= max;
                progress = startSwishing - progress;
                progress *= (float)Math.Pow(progress / startSwishing, pow);
                progress = startSwishing - progress;
                progress += max;
            }
            return MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) / 2f + 0.5f, 0f, oldProg);
        }
        public virtual Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * MathHelper.Pi - MathHelper.PiOver2) * -swingDirection);
        }
        public virtual float GetScale(float progress)
        {
            return scale;
        }
        public virtual float GetVisualOuter(float progress, float swingProgress)
        {
            return visualOutwards;
        }

        public void UpdateDirection(Player player)
        {
            if (angleVector.X < 0f)
            {
                player.direction = -1;
                Projectile.direction = -1;
            }
            else if (angleVector.X > 0f)
            {
                player.direction = 1;
                Projectile.direction = 1;
            }
        }

        protected virtual void Initialize(Player player, AequusPlayer aequus)
        {
            AngleVector = Projectile.velocity;
            combo = aequus.itemCombo;
            AequusHelpers.CappedMeleeScale(Projectile);
            swingDirection = 1;
            UpdateDirection(player);
            swingDirection *= Projectile.direction;
        }

        protected virtual void SetArmRotation(Player player, Vector2 rotation)
        {
            float value = MathHelper.WrapAngle(rotation.ToRotation() + (float)Math.PI / 2f);
            float angle = Math.Abs(value);
            int dir = Math.Sign(value);
            //if (dir != player.direction && damaging)
            //{
            //	player.direction = dir;
            //}
            // arm angling code, thanks Split!
            int frame = (angle <= 0.6f) ? 1 : ((angle >= (MathHelper.PiOver2 - 0.1f) && angle <= MathHelper.PiOver4 * 3f) ? 3 : ((!(angle > MathHelper.Pi * 3f / 4f)) ? 2 : 4));
            player.bodyFrame.Y = player.bodyFrame.Height * frame;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(swingDirection == -1);
            writer.Write(combo);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingDirection = reader.ReadBoolean() ? -1 : 1;
            combo = reader.ReadInt32();
        }
    }
}