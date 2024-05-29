using Aequus.Common;
using Aequus.Core.ContentGeneration;
using System;

namespace Aequus.Content.Items.Weapons.Summon.Whips.EekWhip;

[WorkInProgress]
public class ElectricEelWhip : UnifiedWhipItem {
    public override void SetDefaults() {
        Item.DefaultToWhip(WhipProjectile.Type, 10, 5f, 5f, animationTotalTime: 26);
        Item.rare = Commons.Rare.BiomeOcean;
        Item.value = Commons.Cost.BiomeOcean;
    }

    public override void DrawWhip(IWhipController.WhipDrawParams drawInfo) {
        Texture2D texture = drawInfo.Texture;
        int i = drawInfo.SegmentIndex;
        int count = drawInfo.SegmentCount;

        Vector2 originOffset = Vector2.Zero;
        int frameIndex;
        bool head = i == count - 2;
        if (head) { frameIndex = 0; }
        else if (i > 0) { frameIndex = i % 3 + 1; }
        else {
            frameIndex = 4;
            originOffset = new Vector2(0f, 8f);
        }

        Rectangle frame = texture.Frame(2, 5, 0, frameIndex);

        Color lightColor = ExtendLight.Get(drawInfo.Position);
        float rotation = (drawInfo.Position - drawInfo.Next).ToRotation() - MathHelper.PiOver2;
        Vector2 origin = frame.Size() / 2f + originOffset;
        float scale = drawInfo.Projectile.scale;
        SpriteEffects effects = drawInfo.SpriteEffects;

        Vector2 drawCoordinates = drawInfo.Position - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, rotation, origin, scale, effects, 0);
        Main.EntitySpriteDraw(texture, drawCoordinates, frame with { X = frame.Width }, Color.White, rotation, origin, scale, effects, 0);

        if (head) {
            Projectile.GetWhipSettings(drawInfo.Projectile, out float timeToFlyOut, out int _, out float _);
            Texture2D flareTexture = AequusTextures.FlareSoft;
            float progress = drawInfo.Projectile.ai[0] / timeToFlyOut;
            float opacity = Math.Clamp(Vector2.Distance(drawInfo.Position, DrawHelper.ScreenCenter) / 240f, 0f, 1f);
            Color glowColor = new Color(180, 180, 60, 0) * opacity;
            Vector2 flareOrigin = flareTexture.Size() / 2f;
            Vector2 flareScale = new Vector2(1f, 0.5f) * scale;
            Main.EntitySpriteDraw(AequusTextures.FlareSoft, drawCoordinates, null, glowColor, 0f, flareOrigin, flareScale, SpriteEffects.None, 0);
        }
    }

    public override void SetWhipSettings(Projectile projectile, ref WhipSettings settings) {
        settings.Segments = 18;
    }

    public override Color GetWhipStringColor(Vector2 position) {
        return base.GetWhipStringColor(position);
    }
}
