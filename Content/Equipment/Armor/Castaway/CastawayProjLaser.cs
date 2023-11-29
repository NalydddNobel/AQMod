using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Armor.Castaway;

public class CastawayProjLaser : ModProjectile {
    public override string Texture => AequusTextures.Trail.Path;

    private Vector2[] _lineSegments = Array.Empty<Vector2>();
    private float[] _lineRotations = Array.Empty<float>();

    private int _parentProjectile;
    private int _endProjectile;

    public int ParentProjectileIdentity { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
    public int EndProjectileIdentity { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.friendly = true;
        Projectile.timeLeft = 12;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
    }

    public override void AI() {
        try {
            Projectile.alpha += 20;

            if (_lineSegments.Length != 0 && Main.rand.NextBool(Projectile.damage == 0 ? 20 : 3)) {
                var d = Dust.NewDustPerfect(Main.rand.Next(_lineSegments), DustID.Electric, Scale: Main.rand.NextFloat(0.33f, 1f));
                d.noGravity = true;
                d.velocity *= 0;
            }

            if (_parentProjectile == -1 || _endProjectile == -1) {
                return;
            }

            _parentProjectile = ProjectileHelper.FindProjectileIdentity(Projectile.owner, ParentProjectileIdentity);
            if (_parentProjectile == -1 || !Main.projectile[_parentProjectile].active || Main.projectile[_parentProjectile].type != ModContent.ProjectileType<CastawayProjLaserMine>()) {
                _parentProjectile = -1;
                return;
            }

            Vector2 endPosition;
            bool npcOwner = EndProjectileIdentity <= -Main.maxProjectiles;
            if (npcOwner) {
                _endProjectile = -(EndProjectileIdentity + Main.maxProjectiles);
                if (!Main.npc[_endProjectile].active || !Main.npc[_endProjectile].CanBeChasedBy(Projectile)) {
                    _endProjectile = -1;
                    return;
                }

                endPosition = Main.npc[_endProjectile].Center;
            }
            else {
                _endProjectile = ProjectileHelper.FindProjectileIdentity(Projectile.owner, EndProjectileIdentity);
                if (_endProjectile == -1 || !Main.projectile[_endProjectile].active || Main.projectile[_endProjectile].type != ModContent.ProjectileType<CastawayProjLaserMine>()) {
                    _endProjectile = -1;
                    return;
                }

                endPosition = Main.projectile[_endProjectile].Center;
            }

            Projectile.Center = Main.projectile[_parentProjectile].Center;
            var distance = endPosition - Projectile.Center;
            int segmentCount = (int)Math.Max(distance.Length() / 16f, 3f);
            if (_lineSegments.Length != segmentCount || _lineRotations.Length != segmentCount) {
                _lineSegments = new Vector2[segmentCount];
                _lineRotations = new float[segmentCount];
                for (int i = 0; i < segmentCount; i++) {
                    _lineSegments[i] = Projectile.Center + distance / (segmentCount - 1) * i + Main.rand.NextVector2Unit() * 8f - Projectile.velocity;
                    _lineRotations[i] = Projectile.velocity.ToRotation();
                }
            }
        }
        catch (Exception ex) {
            Main.NewText(ex);
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        for (int i = 0; i < _lineSegments.Length; i += 2) {
            if (targetHitbox.Contains(_lineSegments[i].ToPoint())) {
                return true;
            }
        }
        return false;
    }

    public override bool PreDraw(ref Color lightColor) {
        if (_lineSegments.Length > 0) {
            DrawHelper.DrawBasicVertexLine(AequusTextures.Trail, _lineSegments, _lineRotations,
                (p) => new Color(100, 255, 255, 0) * MathF.Sin(p * MathHelper.Pi) * MathF.Pow(Projectile.Opacity, 2f) * (Projectile.damage == 0 ? 0.66f : 3f), (p) => (MathF.Sin(p * MathHelper.Pi) * 2f + 2f) * Projectile.Opacity * (Projectile.damage == 0 ? 1f : 2.5f), Projectile.Size / 2f - Main.screenPosition);
        }
        return false;
    }
}
