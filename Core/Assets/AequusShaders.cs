using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Assets;

public sealed partial class AequusShaders : AssetManager<Effect> {
    public static readonly RequestCache<Effect> FadeToCenter = new("Aequus/Assets/Shaders/FadeToCenter");
}