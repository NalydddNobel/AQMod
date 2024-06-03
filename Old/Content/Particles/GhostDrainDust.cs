using System;

namespace Aequus.Old.Content.Particles;

public class GhostDrainDust : ModDust {
    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
        dust.noLight = false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        return new Color(dust.color.R - dust.alpha, dust.color.G - dust.alpha, dust.color.B - dust.alpha, dust.color.A - dust.alpha);
    }

    public override bool Update(Dust dust) {
        if (dust.customData == null) {
            dust.active = false;
            return false;
        }
        if (!dust.noLight) {
            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
        }
        var player = (Player)dust.customData;
        var diff = player.MountedCenter - dust.position;
        float lerpValue = Math.Clamp(1f - 1f * dust.scale, 0.05f, 0.9f);
        float playerM = 0f;
        if (diff.Length() < 120f) {
            lerpValue = Math.Min(lerpValue * 1.25f, 1f);
            playerM += 0.2f;
        }
        if (diff.Length() < 60f) {
            lerpValue = Math.Min(lerpValue * 2f, 1f);
            playerM += 0.8f;
        }
        dust.position += player.velocity * playerM;
        diff = player.MountedCenter - dust.position;
        dust.velocity = Vector2.Lerp(dust.velocity, Vector2.Normalize(diff) * Math.Max(diff.Length() / 15f * Math.Clamp(lerpValue * 3f, 1f, 15f), 2f), lerpValue);
        dust.position += dust.velocity;
        dust.scale -= 0.01f;
        if (diff.Length() < 20f) {
            dust.scale -= 0.02f;
        }
        if (dust.alpha > 0) {
            dust.alpha -= 10;
            if (dust.alpha < 0)
                dust.alpha = 0;
        }
        if (dust.scale <= 0.1f) {
            dust.active = false;
        }
        return false;
    }
}