using ReLogic.Content;

namespace AequusRemake.Core.Structures;

public interface ISpriteProvider {
    void GetSpriteParams(out Texture2D texture, out Rectangle frame);
}

public class SpriteProvider(Asset<Texture2D> Texture) : ISpriteProvider {
    void ISpriteProvider.GetSpriteParams(out Texture2D texture, out Rectangle frame) {
        texture = Texture.Value;
        frame = Texture.Value.Bounds;
    }
}

public class FramedSpriteProvider(Asset<Texture2D> Texture, Rectangle Frame) : ISpriteProvider {
    public FramedSpriteProvider(Asset<Texture2D> texture, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0, int spritePaddingX = 0, int spritePaddingY = 0) : this(texture, texture.Frame(horizontalFrames, verticalFrames, frameX, frameY, spritePaddingX, spritePaddingY)) { }

    void ISpriteProvider.GetSpriteParams(out Texture2D texture, out Rectangle frame) {
        texture = Texture.Value;
        frame = Frame;
    }
}