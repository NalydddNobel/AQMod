

namespace Aequus.Common.Drawing.Generative;

public class EffectAtlasMerge(int X, int Y, int W, int H, AtlasInfo Dimensions) : ITextureGenerator {
    Color[] ITextureGenerator.GenerateData(ref TextureGenContext context) {
        int calcuatedResultWidth = W * Dimensions.Width;
        int calcuatedResultHeight = 0;
        for (int l = 0; l < H; l++) {
            calcuatedResultHeight += Dimensions.Heights[l % Dimensions.Heights.Length];
        }

        Color[] canvas = new Color[calcuatedResultWidth * calcuatedResultHeight];

        int heightCollected = 0;
        for (int n = 0; n < H; n++) {
            int frameHeight = Dimensions.Heights[n % Dimensions.Heights.Length];

            for (int m = 0; m < W; m++) {
                for (int i = 0; i < Dimensions.Width; i++) {
                    for (int j = 0; j < frameHeight; j++) {
                        int copyX = X + i + (Dimensions.Width + Dimensions.PadX) * m;
                        int copyY = Y + j + heightCollected;
                        int pasteX = i + (Dimensions.Width * m);
                        int pasteY = j + (heightCollected - Dimensions.PadY * n);

                        canvas[pasteX + pasteY * calcuatedResultWidth] = context[copyX + copyY * context.TextureWidth];
                    }
                }
            }

            heightCollected += frameHeight + Dimensions.PadY;
        }

        context.TextureWidth = calcuatedResultWidth;
        context.TextureHeight = calcuatedResultHeight;

        return canvas;
    }
}

public record struct AtlasInfo(int Width, int PadX, int[] Heights, int PadY);
