using Aequus.Common.UI;
using Aequus.Core.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Text;
using System;
using System.Threading;
using Terraria;
using Terraria.Utilities;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

public class EtOmniaVanitasBar : UILayer {
    public static float Opacity { get; set; }
    public static float HitAnimation { get; set; }

    public override string Layer => InterfaceLayers.EntityHealthBars_16;

    public override void OnGameUpdate() {
        HitAnimation *= 0.8f;
        var player = Main.LocalPlayer;
        if (player.HeldItemFixed().ModItem is not EtOmniaVanitas || !player.TryGetModPlayer<EtOmniaVanitasPlayer>(out var modPlayer) || modPlayer.chargeProgress <= 0) {
            if (Opacity > 0f) {
                Opacity -= 0.033f;
                if (Opacity < 0f) {
                    Opacity = 0f;
                }
            }
            return;
        }

        if (Opacity < 1f) {
            Opacity += 0.1f;
            if (Opacity > 1f) {
                Opacity = 1f;
            }
        }
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        if (Opacity <= 0f) {
            return true;
        }

        var player = Main.LocalPlayer;
        var modPlayer = player.GetModPlayer<EtOmniaVanitasPlayer>();
        float progress = 1f - MathF.Pow(1f - modPlayer.chargeProgress / (float)EtOmniaVanitas.MaxChargeProgress, 1.5f);

        spriteBatch.End();
        spriteBatch.BeginUI(immediate: true);

        AequusShaders.CircleBar.Value.Parameters["uOpacity"].SetValue(progress);
        AequusShaders.CircleBar.Value.Techniques[0].Passes["Main"].Apply();

        var texture = AequusTextures.EtOmniaVanitasBar.Value;
        var drawCoordinates = (player.Center - Main.screenPosition) / Main.UIScale;
        float scale = 0.6f + HitAnimation * 0.05f;
        var origin = texture.Size() / 2f;
        float rotation = MathHelper.PiOver2;
        var barColor = Color.White * Opacity * 0.8f;
        spriteBatch.Draw(texture, drawCoordinates, null, barColor, rotation, origin, scale, SpriteEffects.None, 0f);
        if (HitAnimation > 0f) {
            spriteBatch.Draw(texture, drawCoordinates, null, barColor with { A = 0 } * HitAnimation, rotation, origin, scale + HitAnimation * 0.05f, SpriteEffects.None, 0f);
        }

        spriteBatch.End();
        spriteBatch.BeginUI(immediate: false);

        float radius = 120f;
        if (player.TryGetTimer(EtOmniaVanitasPlayer.TimerId, out var timer) && timer.Active) {
            const int SparklesToRender = 10;
            float time = Main.GlobalTimeWrappedHourly * 24f;
            var sparkleTexture = AequusTextures.Flare.Value;
            var sparkleOrigin = sparkleTexture.Size() / 2f;
            float timerProgress = 1f - timer.TimePassed / timer.MaxTime;
            var verticalScale = new Vector2(0.5f, 1f);
            var horizontalScale = new Vector2(0.5f, 1.5f);
            float timerScale = MathF.Sin((1f - MathF.Pow(1f - timerProgress, 2f)) * MathHelper.Pi);
            for (int i = 0; i < (int)(progress * 30); i++) {
                var fastRandom = new FastRandom((int)time + i).WithModifier((ulong)(time + i));
                float sparkleProgress = 1f / SparklesToRender * (i + 1) - time / SparklesToRender % (1f / SparklesToRender);
                var color = Color.LightSkyBlue.HueAdd(fastRandom.NextFloat(-0.2f, 0.2f)) with { A = 0 } * MathF.Sin(sparkleProgress * MathHelper.Pi) * fastRandom.NextFloat(0.3f, 2f) * timerProgress;
                float sparkleScale = MathF.Pow(MathF.Sin(sparkleProgress * MathHelper.Pi), fastRandom.NextFloat(1f, 3f)) * fastRandom.NextFloat(0.3f, 0.8f) * timerProgress;
                float sparkleRotation = fastRandom.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                var textureUVX = Math.Abs(Math.Sin(sparkleRotation / 2));
                if (textureUVX > progress) {
                    continue;
                }
                var position = drawCoordinates + (sparkleRotation + MathHelper.PiOver2).ToRotationVector2() * (radius + (1f - sparkleProgress) * fastRandom.NextFloat(20f)) * scale;
                spriteBatch.Draw(sparkleTexture, position, null, color, 0f, sparkleOrigin, verticalScale* sparkleScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkleTexture, position, null, color, MathHelper.PiOver2, sparkleOrigin, horizontalScale *  sparkleScale, SpriteEffects.None, 0f);
            }
        }
        if (player.TryGetTimer(EtOmniaVanitasPlayer.TimerId + "Complete", out var completeTimer) && completeTimer.Active) {
            var sparkleTexture = AequusTextures.Flare.Value;
            var sparkleOrigin = sparkleTexture.Size() / 2f;
            float sparkleProgress = MathF.Pow(completeTimer.TimePassed / completeTimer.MaxTime, 2f);
            var color = Color.Lerp(Color.White, Color.Blue, sparkleProgress) with { A = 0 } * 0.66f;
            float sparkleScale = MathF.Sin(sparkleProgress * MathHelper.Pi);
            float sparkleRotationProgress = 1f - MathF.Pow(1f - completeTimer.TimePassed / completeTimer.MaxTime, 2f);
            int sparkleCount = 3;
            for (int i = -sparkleCount; i <= sparkleCount; i++) {
                float scaleMultiplier = 1f - Math.Abs(i) / (float)(sparkleCount + 1);
                var position = drawCoordinates + new Vector2(0f, -radius).RotatedBy(i * sparkleRotationProgress * 0.4f) * scale;

                spriteBatch.Draw(sparkleTexture, position, null, color, i * 0.2f, sparkleOrigin, new Vector2(0.5f, 1f) * sparkleScale * scaleMultiplier, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkleTexture, position, null, color, MathHelper.PiOver2, sparkleOrigin, new Vector2(0.5f, 2f) * sparkleScale * scaleMultiplier, SpriteEffects.None, 0f);
            }
        }
        return true;
    }
}