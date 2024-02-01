using Aequus.Old.Content.Necromancy.Sceptres.Evil;
using Aequus.Old.Content.Particles;
using Terraria.GameContent;

namespace Aequus.Old.Content.Necromancy.Sceptres.Dungeon;

public class RevenantProj : CorruptionSceptreProj {
    public override string Texture => AequusTextures.CorruptionSceptreProj.Path;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        base.SetStaticDefaults();
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.scale = 0.8f;
        Projectile.alpha = 10;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
        Projectile.alpha = 250;
        Projectile.scale = 1.1f;
        Projectile.DamageType = Aequus.NecromancyClass;
        Projectile.extraUpdates = 1;
        InitTrail();
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(100, 222, 255, 255 - Projectile.alpha);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            UpdateTrail();

            int trailCount = _trail.GetLength(0);

            for (int i = 0; i < trailCount; i++) {
                if (Main.rand.NextBool(4)) {
                    continue;
                }

                var p = ModContent.GetInstance<RevenantParticle>().New();
                p.Location = _trail[i][0];
                p.Velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 0.1f * Projectile.MaxUpdates;
                p.Color = new Color(55, 55, 255, 200) * Projectile.Opacity;
                p.BloomColor = Color.Blue with { A = 0 } * 0.02f * Projectile.Opacity;
                p.Scale = 1f;
                p.BloomScale = 0.35f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.Opacity = 0f;
            }
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        int target = Projectile.FindTargetWithLineOfSight(400f);
        if (target != -1) {
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.125f)) * speed;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        NecromancyDebuff.Apply<RevenantDebuff>(target, 600, Projectile.owner);
    }

    public override bool PreDraw(ref Color lightColor) {
        //primColor = new Color(40, 100, 255, 100) * Projectile.Opacity;
        //primScale = 6f;

        //var texture = TextureAssets.Projectile[Type].Value;
        //var frame = Projectile.Frame();
        //var origin = frame.Size() / 2f;

        //int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        //var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        //float scale = Projectile.scale * 1.5f;
        //for (int i = 0; i < trailLength; i++) {
        //    float progress = 1f - i / (float)trailLength;
        //    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity * progress, Projectile.rotation, origin, scale * progress, SpriteEffects.None, 0f);
        //}

        //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(10, 40, 255, 100) * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
        //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) {
        var center = Projectile.Center;
        for (int i = 0; i < 4; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, newColor: new Color(255, 255, 255, 0));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
            d.scale += Main.rand.NextFloat(-0.5f, 0f);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.2f, 0.5f);
        }
        for (int i = 0; i < 12; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(222, 222, 255, 150));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
        }
    }
}