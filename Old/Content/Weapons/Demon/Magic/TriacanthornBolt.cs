using Aequus.DataSets;

namespace Aequus.Old.Content.Weapons.Demon.Magic;

public class TriacanthornBolt : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 15;
        ProjectileDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.extraUpdates = 3;
        Projectile.timeLeft = 80;
    }

    public override void AI() {
        int target = Projectile.FindTargetWithLineOfSight(400f);
        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptionThorns, Projectile.velocity.Y * 0.3f, Projectile.velocity.Y * 0.3f);
        d.velocity *= 0.2f;
        d.noGravity = true;
        d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, Projectile.velocity.Y * 0.3f, Projectile.velocity.Y * 0.3f, newColor: Color.BlueViolet with { A = 0 } * 0.6f);
        d.velocity *= 0.5f;
        d.noGravity = true;
        if (target != -1) {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 5f, 0.04f);
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
}