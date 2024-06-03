using ReLogic.Content;
using System;
using System.Reflection;

namespace Aequus.Core.Graphics.Textures;

public static class TextureGen {
    public static Asset<Texture2D> PerPixel(IColorEffect color, Asset<Texture2D> texture) {
        Texture2D next = PerPixel(color, ExtendTexture.Wait(texture));

        string name = $"Aequus/Gen/{texture.Name}";
        Asset<Texture2D> nextAsset = (Asset<Texture2D>)Activator.CreateInstance(typeof(Asset<Texture2D>), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, [name], null, null);
        SetAsset(nextAsset, next);
        return nextAsset;
    }

    public static Texture2D PerPixel(IColorEffect color, Texture2D texture) {
        // Get context data
        ColorEffectContext context = ColorEffectContext.FromTexture(texture);

        // Populate color array
        PerPixel(color, ref context, context.Colors);

        // Create and set new texture data.
        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, texture.Width, texture.Height);
            resultTexture.SetData(context.Colors);
            return resultTexture;
        }
        catch (Exception ex) {
            Log.Error(ex);

            // return null if error occurs.
            return null;
        }
    }

    public static void PerPixel(IColorEffect color, ref ColorEffectContext context, Color[] outputColorArr) {
        color.Prepare(in context);
        for (int i = 0; i < outputColorArr.Length; i++) {
            context.index = i;
            outputColorArr[i] = color.GetColor(in context);
        }
    }

    private static void SetAsset(Asset<Texture2D> asset, Texture2D newValue) {
        typeof(Asset<Texture2D>).GetMethod("SubmitLoadedContent", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Invoke(asset, [newValue, null]);
    }
}
