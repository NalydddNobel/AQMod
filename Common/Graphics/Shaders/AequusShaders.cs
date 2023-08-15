namespace Aequus.Common.Graphics.Shaders;

public class AequusShaders {
    private static ShaderAsset GetShader(string name) {
        return new($"{Aequus.AssetsPath}Effects/{name}");
    }

    public static ShaderAsset GlintMiscShader = GetShader("GlintMiscShader");
}