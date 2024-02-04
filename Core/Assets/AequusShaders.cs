namespace Aequus.Core.Assets;

public sealed partial class AequusShaders : AssetManager<Effect> {
    public static readonly RequestCache<Effect> FadeToCenter = new("Aequus/Assets/Shaders/FadeToCenter");
    public static readonly RequestCache<Effect> UVVertexShader = new("Aequus/Assets/Shaders/UVVertexShader");
    public static readonly RequestCache<Effect> BubbleMerge = new("Aequus/Assets/Shaders/BubbleMerge");
    public static readonly RequestCache<Effect> Multiply = new("Aequus/Assets/Shaders/Multiply");
    public static readonly RequestCache<Effect> LuminentMultiply = new("Aequus/Assets/Shaders/LuminentMultiply");
}