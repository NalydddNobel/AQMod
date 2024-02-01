using Aequus.Common.Tiles;

namespace Aequus.Old.Content.Tiles.Furniture.Oblivion;

[LegacyName("OblivionChestTile")]
public class OblivionChest : BaseChest {
    public override void SafeSetStaticDefaults() {
        DustType = DustID.Ash;
        AddMapEntry(Color.Red.SaturationMultiply(0.7f), CreateMapEntryName(), MapChestName);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.OblivionChest_Glow, Color.White);
    }
}
