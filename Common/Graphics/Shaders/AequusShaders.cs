namespace Aequus.Common.Graphics.Shaders;

public class AequusShaders {
    private static ShaderAsset GetShader(string name) {
        return new($"{Aequus.AssetsPath}Effects/{name}");
    }

    public static readonly ShaderAsset GlintMiscShader = GetShader("GlintMiscShader");
    public static readonly ShaderAsset Trailshader = GetShader("Prims/Trailshader");
}