using Aequus.Old.Content.Necromancy.Sceptres.Evil;
using Aequus.Old.Content.Particles;
using Terraria.GameContent;

namespace Aequus.Old.Content.Necromancy.Sceptres.Forbidden;
public class OsirisProj : CorruptionSceptreProj {
    public override string Texture => AequusTextures.CorruptionSceptreProj.Path;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
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
        Projectile.extraUpdates = 1;
        Projectile.scale = 1.33f;
        Projectile.DamageType = Aequus.NecromancyClass;
        InitTrail();
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 222, 100, 255 - Projectile.alpha);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            UpdateTrail();

            int trailCount = _trail.GetLength(0);

            for (int i = 0; i < trailCount; i++) {
                if (!Main.rand.NextBool(3)) {
                    continue;
                }

                var p = ModContent.GetInstance<LegacyBloomParticle>().New();
                p.Location = _trail[i][0];
                p.Velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * -0.125f;
                p.Color = new Color(255, 160, 100, 100);
                p.BloomColor = Color.OrangeRed with { A = 0 } * 0.1f;
                p.Scale = 1.1f;
                p.BloomScale = 0.35f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 13;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        int target = Projectile.FindTargetWithLineOfSight(500f);
        if (target != -1) {
            float speed = Projectile.velocity.Length();
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.1f)) * speed;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        //NecromancyDebuff.ReduceDamageForDebuffApplication<OsirisDebuff>(Tier, target, ref modifiers);
    }

    public void SpawnLocusts(Entity target) {
        var source = Projectile.GetSource_OnHit(target, "Aequus:Osiris");
        int distance = (int)(target.Size.Length() / 2f);
        for (int i = 0; i < 3; i++) {
            var normal = Main.rand.NextVector2Unit();
            var p = Projectile.NewProjectile(source, target.Center + normal * distance, normal * 3f, LocustType(Main.player[Projectile.owner]), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f, target.whoAmI);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

        NecromancyDebuff.Apply<OsirisDebuff>(target, 600, Projectile.owner);
        SpawnLocusts(target);
    }
    public int LocustType(Player player) {
        if (player.strongBees && Main.rand.NextBool(3)) {
            return ModContent.ProjectileType<LocustLarge>();
        }
        return ModContent.ProjectileType<LocustSmall>();
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        SpawnLocusts(target);
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var origin = frame.Size() / 2f;

        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        for (int i = 0; i < trailLength; i++) {
            float progress = 1f - i / (float)trailLength;
            Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity * progress, Projectile.rotation, origin, Projectile.scale * 0.75f * progress, SpriteEffects.None, 0f);
        }

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(255, 128, 10, 100) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) {
        var center = Projectile.Center;
        for (int i = 0; i < 7; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, newColor: new Color(255, 255, 255, 0));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
            d.scale += Main.rand.NextFloat(-0.5f, 0f);
            d.fadeIn = d.scale + Main.rand.NextFloat(0.2f, 0.5f);
        }
        for (int i = 0; i < 20; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(255, 222, 222, 150));
            d.velocity *= 0.2f;
            d.velocity += (d.position - center) / 8f;
        }
    }
}