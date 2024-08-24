using Aequus.Common.Utilities.IO;
using System;
using System.Diagnostics;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Maps.CartographyTable;

public class ServerMap {
    public const int ChunkWidth = 200;
    public const int ChunkHeight = 100;

    public int ChunkRows => Helper.DivCeiling(Width, ChunkWidth);
    public int ChunkColumns => Helper.DivCeiling(Height, ChunkHeight);
    public int MaxChunks => ChunkRows * ChunkColumns;

    public readonly int Width;
    public readonly int Height;
    public readonly bool[] NoDownload;
    public readonly bool[] NoUpload;

    private readonly ServerMapTile[] _map;

    public ServerMap(int Width, int Height) {
        this.Width = Width;
        this.Height = Height;
        _map = new ServerMapTile[Width * Height];
        NoDownload = new bool[MaxChunks];
        NoUpload = new bool[MaxChunks];
    }

    public ServerMapTile this[int x, int y] {
        get => _map[Math.Clamp(x + y * Width, 0, _map.Length)];
        set => _map[Math.Clamp(x + y * Width, 0, _map.Length)] = value;
    }

    public void ScanType(int x, int y) {
        byte type = MapTypeConvert.Instance.ToServerType(Main.tile[x, y], 0);
        this[x, y] = this[x, y] with { Type = type };
    }

    public void SetLight(int x, int y, byte light) {
        this[x, y] = this[x, y] with { Light = light };
    }

    public void GetChunkDimensions(int chunk, out int x, out int y, out int w, out int h) {
        int row = Main.maxTilesX / ChunkWidth;
        if (ChunkWidth * row < Main.maxTilesX) {
            row++;
        }

        x = chunk % row * ChunkWidth;
        y = chunk / row * ChunkHeight;
        w = Math.Min(ChunkWidth, Main.maxTilesX - x);
        h = Math.Min(ChunkHeight, Main.maxTilesY - y);
    }

    public TagCompound Save() {
        return new TagCompound() {
            ["Width"] = Width,
            ["Height"] = Height,
            ["Light"] = _map.Select(i => i.Light).ToArray(),
            ["Type"] = _map.Select(i => i.Type).ToArray(),
        };
    }

    public bool Load(TagCompound tag) {
        if (!tag.TryGet("Width", out int w) || Width != w || !tag.TryGet("Height", out int h) || Height != h) {
            return false;
        }

        if (tag.TryGet("Light", out byte[] lights)) {
            for (int i = 0; i < lights.Length; i++) {
                _map[i].Light = lights[i];
            }
        }
        if (tag.TryGet("Type", out byte[] types)) {
            for (int i = 0; i < types.Length; i++) {
                _map[i].Type = types[i];
            }
        }

        return true;
    }

    [Conditional("DEBUG")]
    internal void CreateDebugMap() {
        using (var stream = FileServices.CreateTempFileStream("map.server.png")) {
            var img = new System.Drawing.Bitmap(Width, Height);
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    byte light = this[i, j].Light;

                    Color xnaColor = this[i, j].Type switch {
                        MapTypeConvert.Block => new Color(69, 95, 186),
                        MapTypeConvert.BGObject => new Color(39, 71, 145),
                        MapTypeConvert.Wall => new Color(35, 62, 97),
                        _ => new Color(15, 29, 42)
                    };
                    xnaColor = xnaColor.MultiplyRGB(new Color(light, light, light));

                    var rgb = System.Drawing.Color.FromArgb(255, xnaColor.R, xnaColor.G, xnaColor.B);
                    img.SetPixel(i, j, rgb);
                }
            }
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public float GetChunkProgress(int chunk) {
        return chunk / (float)MaxChunks;
    }

    public void ResetNoUploadList() {
        for (int i = 0; i < NoUpload.Length; i++) {
            NoUpload[i] = false;
        }
    }

    public void SetNoUpload(int chunk, bool value) {
        if (chunk <= 0 || chunk >= MaxChunks) {
            return;
        }

        NoUpload[chunk] = value;
    }
}
