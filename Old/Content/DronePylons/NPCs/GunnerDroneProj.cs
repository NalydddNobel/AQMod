using Aequu2.Content.Dusts;
using System;

namespace Aequu2.Old.Content.DronePylons.NPCs;

public class GunnerDroneProj : ModProjectile {
    public override string Texture => Aequu2Textures.None.Path;

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.timeLeft *= 5;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
        Projectile.manualDirectionChange = true;
    }

    public override void AI() {
        const float BeamReach = 1600f;

        Projectile.alpha += 16;
        if (Projectile.alpha > 255) {
            Projectile.Kill();
        }

        var npc = Main.npc[(int)Projectile.ai[0]];
        if (!npc.active || npc.ModNPC is not TownDroneBase townDrone) {
            Projectile.Kill();
            return;
        }
        Projectile.Center = npc.Center;
        Projectile.rotation = npc.rotation;
        Projectile.direction = npc.spriteDirection;

        Vector2 startingPoint = Projectile.Center;
        Vector2 normal = Projectile.rotation.ToRotationVector2() * Projectile.direction;
        int i = 0;
        for (; i < BeamReach; i += 8) {
            if (Collision.SolidCollision(startingPoint + normal * i - new Vector2(4f), 10, 10)) {
                break;
            }
        }

        Projectile.ai[1] = i;

        Color color = townDrone.GetPylonColor();
        if (Projectile.localAI[0] == 0f) {
            Projectile.localAI[0]++;

            for (int k = 0; k < i; k += 4) {
                if (Main.rand.NextBool(3)) {
                    Dust d = Dust.NewDustPerfect(startingPoint + normal * k, ModContent.DustType<MonoDust>(), newColor: color with { A = 100 } * 0.6f, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.noGravity = true;
                    d.velocity *= 0.8f;
                }
            }
        }

        if (Main.rand.NextBool(3)) {
            Dust endBeamDust = Dust.NewDustPerfect(startingPoint + normal * i, ModContent.DustType<MonoDust>(), newColor: color with { A = 40 }, Scale: Main.rand.NextFloat(1f, 2.5f));
            endBeamDust.noGravity = true;
        }
    }

    public override bool? CanHitNPC(NPC target) {
        return target.friendly || target.damage <= 0 ? false : null;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        float _ = float.NaN;
        var normal = Projectile.rotation.ToRotationVector2();
        var end = Projectile.Center + normal * Projectile.ai[1] * Projectile.direction;
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
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        Vector2 normal = Projectile.rotation.ToRotationVector2() * Projectile.direction;
        Vector2[] indices = new Vector2[] {
                drawPos,
                drawPos + normal * Projectile.ai[1] / 2f,
                drawPos + normal * Projectile.ai[1], };

        for (int i = 0; i < indices.Length; i++) {
            Main.ReverseGravitySupport(indices[i]);
        }
        int amount = (int)(5 * (ExtendedMod.HighQualityEffects ? 1f : 0.5f));

        float[] rotations = Helper.GenerateRotationArr(indices);
        Color laserColor = townDrone.GetPylonColor() with { A = 100 } * MathF.Pow(Projectile.Opacity, 2f);
        DrawHelper.DrawBasicVertexLine(Aequu2Textures.Trail, indices, rotations,
            p => laserColor,
            p => 4f
        );
        DrawHelper.DrawBasicVertexLine(Aequu2Textures.Trail, indices, rotations,
            p => laserColor with { A = 0 } * 0.5f,
            p => 8f
        );
        return false;
    }
}