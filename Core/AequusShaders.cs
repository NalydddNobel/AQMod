using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core;

public partial class AequusShaders : AssetLoader<ShaderAsset, Effect> {
    public static readonly ShaderAsset FadeToCenter = new("Aequus/Assets/Shaders/FadeToCenter");
}