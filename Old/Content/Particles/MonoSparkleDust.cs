﻿namespace Aequus.Old.Content.Particles;

public class MonoSparkleDust : ModDust {
    public override void OnSpawn(Dust dust) {
        dust.noGravity = true;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        var color = new Color(lightColor.ToVector3() * dust.color.ToVector3()) with { A = dust.color.A } * (1f - dust.alpha / 255f);
        return color;
    }

    public override bool Update(Dust dust) {
        if (dust.scale > 10f) {
            dust.active = false;
        }
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.5f;
        if (!dust.noLightEmittence) {
            Lighting.AddLight(dust.position, dust.color.ToVector3() * (dust.scale > 1f ? 1f : dust.scale));
        }
        if (dust.noGravity) {
            dust.velocity *= 0.93f;
            if (dust.velocity.Length() < 1f) {
                dust.noGravity = false;
            }
            if (dust.fadeIn == 0f) {
                dust.scale += 0.0025f;
            }
        }
        else {
            dust.velocity *= 0.95f;
            dust.scale -= 0.01f;
        }
        var tileCoords = dust.position.ToTileCoordinates();
        if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10) &&
            WorldGen.SolidTile(Framing.GetTileSafely(tileCoords.X, tileCoords.Y)) && dust.fadeIn == 0f && !dust.noGravity) {
            dust.scale *= 0.9f;
            dust.velocity *= 0.25f;
        }
        if (dust.scale < 0.1f) {
            dust.active = false;
        }
        if (dust.alpha > 0) {
            dust.alpha -= 10;
            if (dust.alpha < 0) {
                dust.alpha = 0;
            }
        }
        return false;
    }
}