using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequu2.Old.Content.Items.Accessories.HighSteaks;

public class HighSteaksMoneyConsumeEffect : PlayerDrawLayer {
    public static readonly List<float> CoinAnimations = new();

    public override void Unload() {
        CoinAnimations?.Clear();
    }

    public override Position GetDefaultPosition() {
        return new BeforeParent(PlayerDrawLayers.WaistAcc);
    }

    protected override void Draw(ref PlayerDrawSet drawInfo) {
        if (!drawInfo.drawPlayer.active) {
            return;
        }

        //Main.NewText(drawInfo.drawPlayer.waist +"|"+ HighSteaks.WaistSlot);
        if (Main.gameMenu || drawInfo.drawPlayer.GetModPlayer<HighSteaksPlayer>().highSteaksHidden) {
            CoinAnimations.Clear();
            return;
        }

        for (int i = 0; i < CoinAnimations.Count; i++) {
            float animationTime = CoinAnimations[i] % 100f;
            float rotation = CoinAnimations[i] / 100f;
            ulong seed = (ulong)rotation;
            var drawLocation = drawInfo.Position + new Vector2(drawInfo.drawPlayer.width / 2f, drawInfo.drawPlayer.height / 2f) + rotation.ToRotationVector2() * animationTime * Utils.RandomInt(ref seed, 40, 64) / 10f;
            float opacity = (float)Math.Pow(animationTime > 8 ? 1f - (animationTime - 8f) / 16f : 1f, 2f);
            var texture = TextureAssets.Coin[1];
            var frame = texture.Value.Frame(verticalFrames: 8, frameY: (int)((Main.GameUpdateCount / 10 + CoinAnimations[i] / 5) % 8));
            drawInfo.DrawDataCache.Add(
                new DrawData(texture.Value, (drawLocation - Main.screenPosition).Floor(), frame, ExtendLight.Get(drawLocation) * opacity, 0f, frame.Size() / 2f, 1f, drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0) { shader = 0, });
            CoinAnimations[i]++;
            if (animationTime > 24) {
                CoinAnimations.RemoveAt(i);
                i--;
            }
        }
    }
}