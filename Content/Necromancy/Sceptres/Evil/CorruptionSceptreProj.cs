using Aequus.Common.Particles.New;
using Aequus.Common.Utilities;
using Aequus.Particles.Dusts;
using Terraria.GameContent;

namespace Aequus.Content.Necromancy.Sceptres.Evil;

[LegacyName("ZombieBolt")]
public class CorruptionSceptreProj : ModProjectile {
    protected float trailSpeed = 5f;
    protected float primDistance = 4f;
    protected float primScale;
    protected Color primColor;

    protected Vector2[][] _trail;

    public override void SetStaticDefaults() {
        PushableEntities.ProjectileIDs.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.scale = 0.8f;
        Projectile.alpha = 10;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
        Projectile.DamageType = Aequus.NecromancyClass;
        Projectile.alpha = 255;
        InitTrail();
    }

    protected void InitTrail() {
        _trail = new Vector2[3][];
        for (int i = 0; i < 3; i++) {
            _trail[i] = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
        }
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(100, 140, 255, 255 - Projectile.alpha);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            UpdateTrail();

            int trailCount = _trail.GetLength(0);

            for (int i = 0; i < trailCount; i++) {
                var p = ModContent.GetInstance<LegacyBloomParticle>().New();
                p.Location = _trail[i][0];
                p.Velocity = Projectile.velocity * -0.125f;
                p.Color = new Color(40, 30, 150, 10) * Projectile.Opacity;
                p.BloomColor = Color.Blue with { A = 0 } * 0.015f * Projectile.Opacity;
                p.Scale = 1.5f;
                p.BloomScale = 0.5f;
            }
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        int target = Projectile.FindTargetWithLineOfSight(300f);
        if (target != -1) {
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.075f)) * speed;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    protected void UpdateTrail() {
        int trailCount = _trail.Length;
        int trailLength = _trail[0].Length;

        for (int i = 0; i < trailCount; i++) {
            for (int j = trailLength - 1; j > 0; j--) {
                _trail[i][j] = _trail[i][j - 1];
            }

            _trail[i][0] = Projectile.Center + Projectile.velocity + (i / (float)trailCount * MathHelper.TwoPi + Main.GameUpdateCount / 4f).ToRotationVector2() * primDistance;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        NecromancyDebuff.Apply<NecromancyDebuff>(target, 600, Projectile.owner);
    }

    public override void OnKill(int timeLeft) {
        var center = Projectile.Center;
        for (int i = 0; i < 12; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(222, 210, 255, 150));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var origin = frame.Size() / 2f;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(10, 40, 255, 100), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
        return false;
    }

    protected void DrawTrail(float waveSize = 8f, int maxLength = -1) {
        int trailCount = _trail.Length;
        for (int i = 0; i < trailCount; i++) {
            DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.Trail0, _trail[i], Projectile.oldRot,
                (p) => primColor * (1f - p),
                (p) => primScale * (1f - p),
                -Main.screenPosition);
        }
        //int trailLength = maxLength > 0 ? maxLength : ProjectileID.Sets.TrailCacheLength[Type];
        //var trail = new Vector2[trailLength];
        //var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        //for (int i = 0; i < trailLength; i++) {
        //    if (Projectile.oldPos[i] == Vector2.Zero) {
        //        trailLength = i;
        //        continue;
        //    }
        //    trail[i] = Projectile.oldPos[i] + offset;
        //}
        //foreach (var f in Helper.Circular(3, Main.GlobalTimeWrappedHourly * timeMultiplier)) {
        //    var renderTrail = new Vector2[trailLength];
        //    Array.Copy(trail, renderTrail, trailLength);

        //    for (int i = 0; i < trailLength; i++) {
        //        renderTrail[i] += new Vector2(0f, (float)Math.Sin(f + i * 0.33f) * waveSize).RotatedBy(Projectile.oldRot[i]);
        //    }

        //    prim.Draw(renderTrail);
        //}
    }
}