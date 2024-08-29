using Aequus.Common.Utilities.Helpers;
using ReLogic.Content;
using System;
using System.Reflection;

namespace Aequus.Common.Drawing.Generative;

public static class TextureGen {
    public static Asset<Texture2D> New(ITextureGenerator generator, Asset<Texture2D> texture) {
        texture.Request();
        Texture2D? next = New(generator, AssetTools.ForceLoad(ref texture));

        if (next == null) {
            Aequus.Instance.Logger.Error("Texture generation error. Effect returned null.");
            return texture;
        }

        string name = $"Aequus/Generated/{texture.Name}";
        return NewAsset(next, name, texture);
    }

    public static Texture2D? New(ITextureGenerator generator, Texture2D texture) {
        // Get context data
        TextureGenContext context = TextureGenContext.FromTexture(texture);

        // Populate color array
        Color[] next = New(generator, ref context);

        // Create and set new texture data.
        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, context.TextureWidth, context.TextureHeight);
            resultTexture.SetData(next);
            return resultTexture;
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);

            // return null if error occurs.
            return null;
        }
    }

    public static Color[] New(ITextureGenerator generator, ref TextureGenContext context) {
        generator.Prepare(in context);
        return generator.GenerateData(ref context);
    }

    public static Asset<Texture2D> PerPixel(IColorEffect color, Asset<Texture2D> texture) {
        texture.Request();
        Texture2D? next = PerPixel(color, texture).Value;

        if (next == null) {
            Aequus.Instance.Logger.Error("Texture generation error. Effect returned null.");
            return texture;
        }

        string name = $"Aequus/PerPixel/{texture.Name}";

        return NewAsset(next, name, errorAsset: texture);
    }

    public static Texture2D? PerPixel(IColorEffect color, Texture2D texture) {
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
            Aequus.Instance.Logger.Error(ex);

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

    private static Asset<Texture2D> NewAsset(Texture2D texture, string name, Asset<Texture2D> errorAsset) {
        try {
            object? activated = Activator.CreateInstance(typeof(Asset<Texture2D>), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, [name], null, null);
            if (activated is not Asset<Texture2D> nextAsset) {
                Aequus.Instance.Logger.Error("Texture generation error. Activated instance is not asset.");
                return errorAsset;
            }

            SetAsset(nextAsset, texture);
            return nextAsset;
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }

        return errorAsset;
    }
    private static void SetAsset(Asset<Texture2D> asset, Texture2D newValue) {
        typeof(Asset<Texture2D>).GetMethod("SubmitLoadedContent", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            ?.Invoke(asset, [newValue, null]);
    }
}
