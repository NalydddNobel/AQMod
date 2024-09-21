using Aequus.Common.Assets;

namespace Aequus.Common.Graphics.Shaders;

public class AequusShaders {
    public static readonly ShaderAsset GlintMiscShader = getLegacyShader("GlintMiscShader");
    public static readonly ShaderAsset Trailshader = getLegacyShader("Prims/Trailshader");
    public static readonly RequestCache<Effect> BubbleMerge = Get("BubbleMerge");
    public static readonly RequestCache<Effect> UVVertexShader = Get("UVVertexShader");
    public static readonly RequestCache<Effect> LuminentMultiply = Get("LuminentMultiply");

    private static RequestCache<Effect> Get(string name) {
        return new($"Effects/{name}");
    }

    private static ShaderAsset getLegacyShader(string name) {
        return new($"{Aequus.AssetsPath}Effects/{name}");
    }
}