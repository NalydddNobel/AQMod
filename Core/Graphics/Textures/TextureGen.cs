using ReLogic.Content;
using System;
using System.Reflection;

namespace Aequus.Core.Graphics.Textures;

public static class TextureGen {
    public static Asset<Texture2D> PerPixel(IColorEffect color, Asset<Texture2D> texture) {
        texture.Wait?.Invoke();
        Texture2D next = PerPixel(color, texture.Value);

        string name = $"Aequus/Gen/{texture.Name}";
        Asset<Texture2D> nextAsset = (Asset<Texture2D>)Activator.CreateInstance(typeof(Asset<Texture2D>), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, [name], null, null);
        SetAsset(nextAsset, next);
        return nextAsset;
    }
    public static Texture2D PerPixel(IColorEffect color, Texture2D texture) {
        Color[] resultColors = new Color[texture.Width * texture.Height];
        texture.GetData(resultColors);
        ColorEffectContext context = new ColorEffectContext(resultColors, texture.Width, texture.Height);
        for (int i = 0; i < resultColors.Length; i++) {
            context.index = i;
            resultColors[i] = color.GetColor(in context);
        }

        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, texture.Width, texture.Height);
            resultTexture.SetData(resultColors);
            return resultTexture;
        }
        catch (Exception ex) {
            Log.Error(ex);

            // return null if error occurs.
            return null;
        }
    }

    private static void SetAsset(Asset<Texture2D> asset, Texture2D newValue) {
        typeof(Asset<Texture2D>).GetMethod("SubmitLoadedContent", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Invoke(asset, [newValue, null]);
    }

    public static Texture2D AddHue(Texture2D baseTexture, float hue) {
        Color[] textureColorsExtracted = new Color[baseTexture.Width * baseTexture.Height];
        baseTexture.GetData(textureColorsExtracted);

        for (int k = 0; k < textureColorsExtracted.Length; k++) {
            Color color = textureColorsExtracted[k];
            byte velocity = Math.Max(Math.Max(color.R, color.G), color.B);
            textureColorsExtracted[k] = color.HueAdd(hue) with { A = color.A };
        }

        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, baseTexture.Width, baseTexture.Height);
            resultTexture.SetData(textureColorsExtracted);
            return resultTexture;
        }
        catch (Exception ex) {
            Log.Error(ex);

            // return null if error occurs, this will retry rendering the textures next frame
            return null;
        }
    }
}
