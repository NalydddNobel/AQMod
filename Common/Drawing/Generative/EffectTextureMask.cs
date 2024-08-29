using Aequus.Common.Utilities.Helpers;
using ReLogic.Content;
using System;

namespace Aequus.Common.Drawing.Generative;

public class EffectTextureMask : IColorEffect {
    private Texture2D _maskTexture;
    private Color[] _sampleColors;
    private Color[] _workingColors;
    public IColorEffect? MaskEffect;

    public int MaskWidth => _maskTexture.Width;
    public int MaskHeight => _maskTexture.Height;

    public EffectTextureMask(Texture2D Texture) {
        _maskTexture = Texture;
        _sampleColors = TextureTools.AllocColorData(Texture);
        _workingColors = _sampleColors;
        MaskEffect = null;
    }
    public EffectTextureMask(Asset<Texture2D> Asset) : this(AssetTools.ForceLoad(ref Asset)) { }

    public EffectTextureMask WithEffect(IColorEffect effect) {
        MaskEffect = effect;
        return this;
    }

    void IColorEffect.Prepare(in ColorEffectContext context) {
        if (context.TextureWidth != MaskWidth || context.TextureHeight != MaskHeight) {
            throw new ArgumentException($"Width and Height in \"{nameof(context)}\" does not match the mask texture's Width and Height.", nameof(context));
        }

        if (MaskEffect != null) {
            _workingColors = new Color[MaskWidth * MaskHeight];
            ColorEffectContext maskContext = new ColorEffectContext(_maskTexture, _sampleColors);
            TextureGen.PerPixel(MaskEffect, ref maskContext, _workingColors);
        }
        else {
            _workingColors = _sampleColors;
        }
    }

    Color IColorEffect.GetColor(in ColorEffectContext context) {
        if (_workingColors[context.index].A > 0) {
            return _workingColors[context.index];
        }
        return context.Color;
    }
}
