using Aequus.Old.Core.Utilities;
using System;

namespace Aequus.Old.Content.DronePylons.NPCs;

public class GunnerDroneProj : ModProjectile {
    public override string Texture => AequusTextures.None.Path;

    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.timeLeft *= 5;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.manualDirectionChange = true;
    }

    public override void AI() {
        Projectile.alpha += 20;
        if (Projectile.alpha > 255)
            Projectile.Kill();
        var npc = Main.npc[(int)Projectile.ai[0]];
        if (!npc.active) {
            Projectile.Kill();
            return;
        }
        Projectile.Center = npc.Center;
        Projectile.rotation = npc.rotation;
        Projectile.direction = npc.spriteDirection;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        float _ = float.NaN;
        var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
        var end = Projectile.Center + normal * 800f * Projectile.direction;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, end, 10f * Projectile.scale, ref _);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.8f);
    }

    public override bool PreDraw(ref Color lightColor) {
        var npc = Main.npc[(int)Projectile.ai[0]];
        if (!npc.active || npc.ModNPC is not TownDroneBase townDrone) {
            return false;
        }

        var drawPos = Projectile.Center - Main.screenPosition;
        var drawColor = new Color(10, 200, 80, 0);
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        var n = Projectile.rotation.ToRotationVector2();
        var arr = new Vector2[] {
                drawPos,
                drawPos + n * 800f * Projectile.direction,
                drawPos + n * 800f * 2f * Projectile.direction, };

        for (int i = 0; i < arr.Length; i++) {
            Main.ReverseGravitySupport(arr[i]);
        }
        var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
        int amount = (int)(5 * (Aequus.HighQualityEffects ? 1f : 0.5f));

        float[] rotations = OldDrawHelper.GenerateRotationArr(arr);
        Color laserColor = townDrone.GetPylonColor() with { A = 0 } * 1.4f * Projectile.Opacity;
        DrawHelper.DrawBasicVertexLine(AequusTextures.Trail, arr, rotations, 
            p => laserColor * (float)Math.Pow(1f - p, 2f),
            p => 4f
        );
        return false;
    }
}