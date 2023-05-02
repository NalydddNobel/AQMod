using Aequus.Buffs.Debuffs;
using Aequus.Common.Net.Sounds;
using Aequus.Items.Tools;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords {
    public class BattleAxeProj : SwordProjectileBase
    {
        public override string Texture => AequusTextures.BattleAxe.Path;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.extraUpdates = 2;
            swordHeight = 70;
        }

        public override void AI()
        {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
        }


        public override float SwingProgress(float progress)
        {
            return GenericSwing3(progress);
        }

        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.4f && progress < 0.6f)
            {
                return scale + 0.3f * Helper.Wave(SwingProgress((progress - 0.4f) / 0.2f), 0f, 1f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress)
        {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            freezeFrame = 4;
            if (Main.rand.NextBool(5))
            {
                ModContent.GetInstance<BleedingDebuffSound>().Play(target.Center);
                target.AddBuff(ModContent.BuffType<BattleAxeBleeding>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);

            float colorIntensity = drawColor.ToVector3().Length() * Projectile.Opacity * 0.2f;
            Color swishColor = new(colorIntensity, colorIntensity, colorIntensity, 0f);

            DrawSwordAfterImages(texture, handPosition, frame, swishColor * 0.33f, rotationOffset, origin, effects);
            DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);
            var size = texture.Size().Length();
            if (AnimProgress > 0.35f && AnimProgress < 0.75f)
            {
                float intensity = (float)Math.Sin((AnimProgress - 0.35f) / 0.4f * MathHelper.Pi);
                var swish = AequusTextures.Swish.Value;
                var swishOrigin = swish.Size() / 2f;
                float r = BaseAngleVector.ToRotation() + ((AnimProgress - 0.45f) / 0.2f * 2f - 1f) * -swingDirection * 0.6f;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition + r.ToRotationVector2() * (size - 14f) * baseSwordScale;
                Main.EntitySpriteDraw(swish, swishLocation, null, swishColor * MathF.Pow(intensity, 2f), r + MathHelper.PiOver2, swishOrigin, 1f, effects, 0);
            }
            return false;
        }
    }
}
