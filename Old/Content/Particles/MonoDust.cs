using System;

namespace Aequus.Old.Content.Particles;

public class MonoDust : ModDust {
    public virtual Single VelocityMultiplier => 0.9f;
    public virtual Single ScaleSubtraction => 0.05f;

    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
        dust.noLight = false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

    public override Boolean Update(Dust dust) {
        dust.velocity *= VelocityMultiplier;
        Single speed = dust.velocity.Length();
        dust.rotation += speed * 0.0314f;
        dust.scale -= ScaleSubtraction - speed / 1000f;
        if (dust.scale <= 0.1f) {
            dust.active = false;
        }

        if (!dust.noLight) {
            Vector3 lightColor;
            lightColor = dust.color.ToVector3() * 0.5f;
            Lighting.AddLight(dust.position, lightColor);
        }
        if (dust.customData != null) {
            if (dust.customData is Player player) {
                dust.position += player.velocity * Math.Min(dust.scale * 2f, 1f);
            }
        }
        dust.position += dust.velocity;
        return false;
    }
}