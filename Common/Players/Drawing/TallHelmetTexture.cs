using ReLogic.Content;
using Terraria.DataStructures;

namespace Aequus.Common.Players.Drawing;

internal class TallHelmetTexture : EquipTexture, IEquipTextureDraw {
    private readonly string _texture;
    private Asset<Texture2D> _asset;

    private TallHelmetTexture(string texturePath) {
        _texture = texturePath;
    }

    public void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader) {
        Texture2D texture = (_asset ??= ModContent.Request<Texture2D>(_texture)).Value;

        int frameY = frame.Y / frame.Height;

        Rectangle realFrame = texture.Frame(verticalFrames: 20, frameY: frameY);
        int difference = realFrame.Height - frame.Height;

        drawInfo.Draw(texture, position - new Vector2(0f, difference), realFrame, color, rotation, origin, 1f, effects, shader);
    }

    public static int RegisterTo(ModItem modItem) {
        return EquipLoader.AddEquipTexture(modItem.Mod, AequusTextures.None.Path, EquipType.Head, modItem, null,
            new TallHelmetTexture(modItem.Texture + "_Head"));
    }
}

