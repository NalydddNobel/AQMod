using Aequu2.Core.ContentGeneration;

namespace Aequu2.Old.Content.Tiles.Furniture.Oblivion;

[LegacyName("OblivionChestTile")]
public class OblivionChest : UnifiedModChest {
    public override void SafeSetStaticDefaults() {
        DustType = DustID.Ash;
        AddMapEntry(Color.Red.SaturationMultiply(0.7f), CreateMapEntryName(), MapChestName);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        DrawBasicGlowmask(i, j, spriteBatch, Aequu2Textures.OblivionChest_Glow, Color.White);
    }
}
