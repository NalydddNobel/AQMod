using System;
using System.Collections.Generic;

namespace AequusRemake.Core.Graphics;

internal class Paletter : IDisposable {
    private Texture2D _templatePalette;
    private Texture2D _templateTexture;
    private byte[] _textureData;

    private Dictionary<Texture2D, Texture2D> _finishedPalettes;
    private bool _queueLock;

    public Paletter(Texture2D TemplatePalette, Texture2D TemplateTexture) {
        _templatePalette = TemplatePalette;
        _templateTexture = TemplateTexture;
        _finishedPalettes = new();
    }

    public void Load() {
        if (Main.dedServ) {
            throw new Exception("Paletter cannot be loaded on the server.");
        }

        Dictionary<Color, byte> colorToIndexLookup = new();

        Color[] paletteColors = new Color[_templatePalette.Height];
        _templatePalette.GetData(paletteColors, 0, paletteColors.Length);

        for (byte i = 0; i < paletteColors.Length; i++) {
            colorToIndexLookup[paletteColors[i]] = i;
        }

        int size = _templateTexture.Width * _templateTexture.Height;
        Color[] templateTextureColors = new Color[size];
        _textureData = new byte[size];
        _templateTexture.GetData(templateTextureColors, 0, size);

        for (int i = 0; i < size; i++) {
            Color color = templateTextureColors[i];
            if (color.A == 0) {
                continue;
            }

            if (colorToIndexLookup.TryGetValue(color, out byte index)) {
                _textureData[i] = (byte)(index + 1);
            }
            else {
                throw new Exception("Template texture contains a color not found in the template palette.");
            }
        }
    }

    public void Request(Texture2D palette) {
        if (_finishedPalettes.ContainsKey(palette) || _queueLock) {
            return;
        }

        if (palette.Height != _templatePalette.Height) {
            throw new Exception("Provided palette does not match the height of the template palette.");
        }

        _queueLock = true;
        Main.QueueMainThreadAction(() => {
            lock (this) {
                _queueLock = false;
                ApplyPalette(palette);
            }
        });
    }

    public void ApplyPalette(Texture2D palette) {
        try {
            Texture2D resultTexture = new Texture2D(Main.instance.GraphicsDevice, _templateTexture.Width, _templateTexture.Height);

            Color[] paletteColors = new Color[palette.Height];
            palette.GetData(paletteColors, 0, paletteColors.Length);

            int size = _templateTexture.Width * _templateTexture.Height;
            Color[] resultColors = new Color[size];

            for (int i = 0; i < size; i++) {
                int index = _textureData[i] - 1;
                if (index >= 0) {
                    resultColors[i] = paletteColors[index];
                }
            }

            resultTexture.SetData(resultColors, 0, size);
            _finishedPalettes[palette] = resultTexture;
        }
        catch (Exception ex) {
            Log.Error(ex);
        }
    }

    public Texture2D GetValue(Texture2D palette) {
        if (_finishedPalettes.TryGetValue(palette, out Texture2D result)) {
            return result;
        }

        return _templateTexture;
    }

    public bool TryGetValueOrRequest(Texture2D palette, out Texture2D result) {
        _finishedPalettes ??= new();
        if (_finishedPalettes.TryGetValue(palette, out result)) {
            return true;
        }

        Request(palette);
        return false;
    }

    public void Dispose() {
        foreach (var pair in _finishedPalettes) {
            pair.Value?.Dispose();
        }

        _finishedPalettes.Clear();
    }
}
