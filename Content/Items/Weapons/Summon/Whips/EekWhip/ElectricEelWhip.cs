using Aequus.Common;
using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Items.Weapons.Summon.Whips.EekWhip;

[WorkInProgress]
public class ElectricEelWhip : UnifiedWhipItem {
    public override string Texture => AequusTextures.DemonVasculash.Path;

    public override void SetDefaults() {
        Item.DefaultToWhip(WhipProjectile.Type, 10, 2f, 3.2f, animationTotalTime: 42);
        Item.rare = Commons.Rare.PollutedOceanLoot;
        Item.value = Commons.Cost.PollutedOceanLoot;
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

    public override void SetWhipSettings(Projectile projectile, ref WhipSettings settings) {

    }
}
