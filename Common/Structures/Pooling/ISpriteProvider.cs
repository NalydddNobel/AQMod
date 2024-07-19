namespace Aequus.Common.Structures.Pooling;

public interface ISpriteProvider {
    void GetSpriteParams(out Texture2D texture, out Rectangle frame);
}
