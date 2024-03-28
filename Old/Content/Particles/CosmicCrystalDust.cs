namespace Aequus.Old.Content.Particles;

public class CosmicCrystalDust : ModDust {
    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
        dust.noLight = true;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        return new Color(255 - dust.alpha, 255 - dust.alpha, 255 - dust.alpha, 255 - dust.alpha);
    }
}