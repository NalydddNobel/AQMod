using Aequus.Common.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    public class ZombieSceptreProj : SceptreProjectileBase {
        public override string Texture => AequusTextures.None.Path;

        public override void SetDefaults() {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = Aequus.NecromancyMagicClass;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            SetLine(3, 14);
        }

        public override void AI() {
            var owner = Main.player[Projectile.owner];
            owner.itemAnimation = Math.Max(owner.itemAnimation, 2);
            owner.itemTime = Math.Max(owner.itemTime, 2);
            if (Projectile.ai[1] > 0f) {
                Projectile.ai[1]++;
                Projectile.Opacity *= 0.9f;
                if (Projectile.ai[1] > 30f) {
                    Projectile.Kill();
                }
            }
            else {
                Helper.AddClamp(ref Projectile.alpha, -5, 0, 255);
                if (!Main.mouseLeft && Main.myPlayer == Projectile.owner) {
                    Projectile.ai[1]++;
                    Projectile.netUpdate = true;
                }
            }
            if (AttachedNPC == -1) {
                if (Main.myPlayer == Projectile.owner) {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * 20f, 0.2f);
                    Projectile.netUpdate = true;
                }
            }
            else {
                base.AI();
            }

            Projectile.CollideWithOthers();
            Projectile.timeLeft = 2;
            float ownerSpeed = owner.velocity.Length();
            var dir = (owner.Center + new Vector2(0f, owner.gfxOffY)) - Projectile.Center;
            var normalDir = Vector2.Normalize(dir);
            if (normalDir.HasNaNs()) {
                return;
            }
            float travelSpeed = dir.Length() / Segments;
            float lerpSubtract = 0.85f;
            if (ownerSpeed > 1f) {
                lerpSubtract = MathF.Pow(lerpSubtract, ownerSpeed / 4f);
            }

            Projectile.localAI[0] += 1f / 60f;
            for (int j = 0; j < Lines; j++) {
                for (int i = 0; i < Segments; i++) {
                    float offsetAmount = MathF.Pow(MathF.Sin(i * MathHelper.Pi / Segments), 2f);
                    var wantedPosition = (Projectile.Center + normalDir * travelSpeed * i)
                        + (dir.RotatedBy(MathHelper.PiOver2) * 0.3f * Helper.Wave(Projectile.localAI[0] * 5f + i * 0.7f + Projectile.whoAmI + j, -0.2f, 0.2f) * offsetAmount);
                    if (i == 0 || i >= Segments - 1) {
                        LineCoordinates[j][i] = wantedPosition;
                        LineRotations[j][i] = dir.ToRotation();
                    }
                    else {
                        if (LineCoordinates[j][i] == Vector2.Zero) {
                            LineCoordinates[j][i] = wantedPosition;
                        }
                        else {
                            LineCoordinates[j][i] = Vector2.Lerp(LineCoordinates[j][i], wantedPosition, 1f - lerpSubtract * offsetAmount);
                        }
                        var difference = LineCoordinates[j][i - 1] - LineCoordinates[j][i];
                        LineRotations[j][i] = difference.ToRotation();
                        if (Main.rand.NextBool(25 * Projectile.MaxUpdates)) {
                            Dust.NewDustPerfect(LineCoordinates[j][i] + Main.rand.NextVector2Square(-16f, 16f), DustID.MagicMirror, -difference / 24f, Scale: Main.rand.NextFloat(0.6f, 1.25f));
                        }
                    }
                }
                //LineCoordinates[j].CollideWithOthers(LineCoordinates[j], 16f, 4f);
            }
        }

        protected override void AIAttached(NPC npc) {
            Projectile.Center = npc.Center;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            AttachedNPC = target.whoAmI;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor) {
            AequusDrawing.ApplyBasicEffect(AequusTextures.Trail3);

            for (int j = 0; j < Lines; j++) {
                var colorStart = Color.Cyan with { A = 0 };
                var colorEnd = Color.Blue with { A = 0 };
                float opacity = Helper.Wave(Projectile.localAI[0] * 10f + Projectile.whoAmI + j, 0.66f, 1f) * Projectile.Opacity * 0.3f;
                colorStart *= opacity;
                colorEnd *= opacity;
                if (LineCoordinates[j].HasNaNs()) {
                    Main.NewText("Line #" + j + " has NaNs.");
                    continue;
                }
                AequusDrawing.VertexStrip.PrepareStripWithProceduralPadding(LineCoordinates[j], LineRotations[j],
                    p => Color.Lerp(colorEnd, colorStart, p),
                    p => 1f + MathF.Sin(p * MathHelper.Pi) * 24f,
                    -Main.screenPosition, true, true);

                AequusDrawing.VertexStrip.DrawTrail();
            }

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }
    }
}