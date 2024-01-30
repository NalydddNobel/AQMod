namespace Aequus.Old.Content.Particles;

public class VoidDust : MonoDust {
    public override System.Single VelocityMultiplier => 0.95f;
    public override System.Single ScaleSubtraction => 0.03f;

    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
        dust.noLight = false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

    public override System.Boolean Update(Dust dust) {
        dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
        dust.rotation += dust.velocity.Length() * 0.4f;
        return base.Update(dust);
    }
}