namespace Aequu2.Old.Content.Necromancy.Sceptres.Forbidden;

public class LocustLarge : LocustSmall {
    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 8;
        Projectile.height = 8;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        target.netUpdate = true;
        target.AddBuff(ModContent.BuffType<LocustDebuff>(), 120);
    }
}