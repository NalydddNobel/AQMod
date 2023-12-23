namespace Aequus.Core.Assets;

public sealed partial class AequusShaders : AssetManager<Effect> {
    public static readonly RequestCache<Effect> FadeToCenter = new("Aequus/Assets/Shaders/FadeToCenter");
    public static readonly RequestCache<Effect> UVVertexShader = new("Aequus/Assets/Shaders/UVVertexShader");
}