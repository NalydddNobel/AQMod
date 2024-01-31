namespace Aequus.Old.Content.Materials.SoulGem;

[LegacyName("EmptySoulGemTile")]
public class SoulGemTile : ModTile {
    protected virtual Color MapColor => new Color(20, 105, 140);
    protected virtual int Item => ModContent.ItemType<SoulGem>();

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileObsidianKill[Type] = true;
        Main.tileNoFail[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        AddMapEntry(MapColor, Lang.GetItemName(Item));
        DustType = DustID.BlueCrystalShard;
    }

    public override bool CanPlace(int i, int j) {
        var top = Framing.GetTileSafely(i, j - 1);
        if (top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType]) {
            return true;
        }
        var bottom = Framing.GetTileSafely(i, j + 1);
        if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType])) {
            return true;
        }
        var left = Framing.GetTileSafely(i - 1, j);
        if (left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType]) {
            return true;
        }
        var right = Framing.GetTileSafely(i + 1, j);
        if (right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType]) {
            return true;
        }
        return false;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.01f;
        g = 0.1f;
        b = 0.2f;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        TileHelper.GemFraming(i, j);
        return false;
    }
}