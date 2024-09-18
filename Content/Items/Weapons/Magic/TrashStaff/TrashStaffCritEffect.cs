using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Helpers;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Items.Weapons.Magic.TrashStaff;

public class TrashStaffCritEffect : ModProjectile {
    public override LocalizedText DisplayName => ModContent.GetInstance<TrashStaff>().DisplayName;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 70;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.extraUpdates = 2;
        Projectile.penetrate = -1;
        Projectile.hide = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 100;
    }

    public override void AI() {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.localAI[0]++;

        if (Projectile.timeLeft <= 60) {
            Projectile.alpha += 4;
            Projectile.velocity *= 0.95f;
            return;
        }

        var target = Projectile.FindTargetWithinRange(600f);
        if (target == null) {
            Projectile.timeLeft = 60;
            return;
        }

        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 10f, 0.01f + Math.Min(Projectile.localAI[0] * 0.001f, 0.08f));
    }

    public override bool? CanDamage() {
        return Projectile.localAI[0] > 7f;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.DisableCrit();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.friendly = false;
        Projectile.velocity = Projectile.DirectionTo(target.Center) * Math.Max(Projectile.velocity.Length() * 1.1f, 8f);
        Projectile.timeLeft = 60;
        Projectile.ai[1] = 1f;
        Projectile.netUpdate = true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        overPlayers.Add(index);
    }

    public override bool PreDraw(ref Color lightColor) {
        lightColor = LightingHelper.Get(Projectile.Center);
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
        var drawCoordinates = Projectile.Center - Main.screenPosition;
        var drawColor = Color.Lerp(lightColor, Color.White, 0.6f) * Projectile.Opacity;

        float scale = Projectile.scale * Projectile.Opacity;

        //Main.spriteBatch.End();
        //Main.spriteBatch.BeginWorld(shader: true);

        DrawHelper.ApplyUVEffect(AequusTextures.TrashStaffCritEffectStrip, new Vector2(30f, 1f), new Vector2(Main.GameUpdateCount / 30f % 1f, 0f));
        DrawHelper.VertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot,
            (p) => drawColor * (1f - p) * Projectile.Opacity,
            (p) => frame.Height / 2f * Projectile.Opacity * Projectile.scale,
            Projectile.Size / 2f - Main.screenPosition);
        DrawHelper.VertexStrip.DrawTrail();

        //Main.spriteBatch.End();
        //Main.spriteBatch.BeginWorld(shader: false);

        var spriteEffects = SpriteEffects.None;
        float rotation = Projectile.rotation;
        if (Math.Abs(MathHelper.WrapAngle(rotation)) >= MathHelper.PiOver2) {
            spriteEffects = SpriteEffects.FlipHorizontally;
            rotation -= MathHelper.Pi;
        }
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, rotation, origin, scale, spriteEffects, 0f);
        return false;
    }
}