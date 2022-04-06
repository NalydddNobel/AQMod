using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public abstract class DopeSwordBase : ModProjectile
    {
        public virtual float VisualHoldout => 8f;
        public virtual float HitboxHoldout => 45f;
        /// <summary>
        /// Multiplies the use time by this value when the player is using the right click function of the item. Lower numbers means faster speed
        /// </summary>
        public virtual float AltFunctionSpeedup => 0.3f;
        /// <summary>
        /// Multiplies the scale by this value when the player is using the right click function of the item. Higher numbers means larger weapon
        /// </summary>
        public virtual float AltFunctionScale => 1.2f;
        /// <summary>
        /// Multiplies the hitbox's dimensions by this value when the player is using the right click function of the item. Higher numbers means larger hitbox dimensions
        /// </summary>
        public virtual float AltFunctionHitboxScale => AltFunctionScale;
        public abstract float Radius { get; }

        public int swingTime;
        public int swingTimeMax;
        public bool damaging;
        public int damageTime;
        public float swing;
        public float swingMultiplier;
        public Vector2 angleVector;
        public int combo;
        public float ProgressSnippet => 1f / swingTimeMax;
        public float SwingProgress => swingTime / (float)swingTimeMax;
        public Vector2 AngleVector => Vector2.Normalize(angleVector);
        public Vector2 BaseAngleVector => Vector2.Normalize(Projectile.velocity);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return CollisionHelper.IsRectangleCollidingWithCircle(Main.player[Projectile.owner].Center, Radius * Projectile.scale, target.getRect()) ? null : false;
        }

        protected virtual void AdjustSwingTime(Player player, AequusPlayer aequusPlayer)
        {
        }
        public virtual void Initalize(Player player, AequusPlayer aequus)
        {
            Projectile.direction = player.direction;
            swingTimeMax = player.itemAnimationMax * (1 + Projectile.extraUpdates);
            if (player.altFunctionUse == 2)
            {
                swingTimeMax = (int)(swingTimeMax * AltFunctionSpeedup);
                Projectile.scale *= AltFunctionScale;
                Projectile.width = (int)(Projectile.width * AltFunctionHitboxScale);
                Projectile.height = (int)(Projectile.height * AltFunctionHitboxScale);
            }
            AdjustSwingTime(player, aequus);
            swingTimeMax = Math.Max(swingTimeMax, 3);
            swing = ProgressSnippet;
            swingMultiplier = 1f;
            combo = aequus.itemCombo;
            AequusHelpers.MeleeScale(Projectile);
        }

        protected void SetArmRotation(Player player)
        {
            SetArmRotation(player, angleVector);
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

        protected virtual void UpdateSwing(Player player, AequusPlayer aequus)
        {
        }

        protected virtual void OnReachMaxProgress()
        {
            Projectile.Kill();
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            var aequus = player.GetModPlayer<AequusPlayer>();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            damaging = false;
            if ((int)Projectile.ai[0] == 0)
            {
                Initalize(player, aequus);
                Projectile.ai[0]++;
            }
            else
            {
                swingTime++;
                if (CanDamage() != false)
                {
                    damaging = true;
                    damageTime++;
                }
            }
            SetArmRotation(player);
            UpdateSwing(player, aequus);
            var v = BaseAngleVector;
            if (damaging)
            {
                if (Projectile.position.X + Projectile.width / 2f < player.position.X + player.width / 2f)
                {
                    if (player.direction == 1)
                    {
                        player.direction = -1;
                        Projectile.direction = -1;
                    }
                }
                else
                {
                    if (player.direction == -1)
                    {
                        player.direction = 1;
                        Projectile.direction = 1;
                    }
                }
            }
            Projectile.position = Main.GetPlayerArmPosition(Projectile) + angleVector * HitboxHoldout;
            Projectile.position.X -= Projectile.width / 2f;
            Projectile.position.Y -= Projectile.height / 2f;
            Projectile.oldPos[0] = angleVector * VisualHoldout;
            Projectile.oldRot[0] = Projectile.rotation = Projectile.oldPos[0].ToRotation() + MathHelper.PiOver4;

            // Manually updating oldPos and oldRot 
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }

            if (SwingProgress >= 1f)
            {
                OnReachMaxProgress();
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(swingTime);
            writer.Write(swingTimeMax);
            writer.Write(damaging);
            writer.Write(damageTime);
            writer.Write(swing);
            writer.Write(swingMultiplier);
            writer.Write(angleVector.X);
            writer.Write(angleVector.Y);
            writer.Write(combo);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            swingTime = reader.ReadInt32();
            swingTimeMax = reader.ReadInt32();
            damaging = reader.ReadBoolean();
            damageTime = reader.ReadInt32();
            swing = reader.ReadSingle();
            swingMultiplier = reader.ReadSingle();
            swingMultiplier = reader.ReadSingle();
            angleVector.X = reader.ReadSingle();
            angleVector.Y = reader.ReadSingle();
            combo = reader.ReadInt32();
        }
    }
}