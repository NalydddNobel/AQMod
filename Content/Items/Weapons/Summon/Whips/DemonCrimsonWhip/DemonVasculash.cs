using Aequus.Common;
using Aequus.Core.ContentGeneration;
using System.Collections.Generic;

namespace Aequus.Content.Items.Weapons.Summon.Whips.DemonCrimsonWhip;

[WorkInProgress]
public class DemonVasculash : UnifiedWhipItem {
    public override void SetDefaults() {
        Item.DefaultToWhip(WhipProjectile.Type, 90, 2f, 2.8f, animationTotalTime: 42);
        Item.rare = Commons.Rare.DemonSiegeLoot;
        Item.value = Commons.Cost.DemonSiegeLoot;
    }

    public override void SetWhipSettings(Projectile projectile, ref WhipSettings settings) {
        settings.Segments = 48;
        settings.RangeMultiplier = 1f;
    }

    public override Color GetWhipStringColor(Vector2 position) {
        return Color.Red with { A = 0 };
    }

    public override void DrawWhip(IWhipController.WhipDrawParams drawInfo) {
        Texture2D texture = drawInfo.Texture;
        int i = drawInfo.SegmentIndex;
        int count = drawInfo.SegmentCount;

        Vector2 originOffset = Vector2.Zero;
        int frameIndex;
        if (i == count - 2) { frameIndex = 0; }
        else if (i > 0) {
            if (i < 3) {
                return;
            }
            frameIndex = i % 2 + 1;
        }
        else {
            frameIndex = 3;
            originOffset = new Vector2(0f, 8f);
        }

        Rectangle frame = texture.Frame(1, 4, 0, frameIndex);

        Color lightColor = ExtendLight.Get(drawInfo.Position);
        Color drawColor = Color.Lerp(Color.White, lightColor, 0.5f);
        float rotation = (drawInfo.Position - drawInfo.Next).ToRotation() - MathHelper.PiOver2;
        Vector2 origin = frame.Size() / 2f + originOffset;
        float scale = drawInfo.Projectile.scale;
        SpriteEffects effects = drawInfo.SpriteEffects;

        Main.EntitySpriteDraw(texture, drawInfo.Position - Main.screenPosition, frame, drawColor, rotation, origin, scale, effects, 0);
    }

    public override void WhipAI(Projectile projectile, List<Vector2> WhipPoints, float Progress) {
        Vector2 endPoint = WhipPoints[^1];
        if (Vector2.Distance(endPoint, Main.player[projectile.owner].Center) > 32f && projectile.localAI[0] > 0f) {
            Vector2 velocity = (endPoint - new Vector2(projectile.localAI[0], projectile.localAI[1])) * 0.3f;

            for (int i = 0; i < 3; i++) {
                Dust d = Dust.NewDustDirect(endPoint - new Vector2(16f), 32, 32, DustID.Torch, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.6f, 2.6f));
                d.noGravity = true;

                if (Main.rand.NextBool()) {
                    d = Dust.NewDustDirect(Main.rand.Next(WhipPoints) - new Vector2(16f), 32, 32, DustID.Torch, Scale: 1.5f);
                    d.noGravity = true;
                }
            }
        }

        projectile.localAI[0] = endPoint.X;
        projectile.localAI[1] = endPoint.Y;
    }
}
