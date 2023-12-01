using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Assets;

public sealed partial class AequusShaders : AssetManager<Effect> {
    public static RequestCache<Effect> FadeToCenter { get; private set; } = new("Aequus/Assets/Shaders/FadeToCenter");
    public static RequestCache<Effect> StariteCore { get; private set; } = new("Aequus/Assets/Shaders/StariteCore");
}