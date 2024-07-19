using ReLogic.Content;

namespace Aequus.Common.Structures.Pooling;

public class SpriteProvider(Asset<Texture2D> Texture) : ISpriteProvider {
    void ISpriteProvider.GetSpriteParams(out Texture2D texture, out Rectangle frame) {
        texture = Texture.Value;
        frame = Texture.Value.Bounds;
    }
}
