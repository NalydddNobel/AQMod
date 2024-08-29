namespace Aequus.Content.Tiles.Tombstones;

public class HellTombstones : UnifiedTombstones {
    public const int Tombstone = 0;
    public const int Grave_Marker = 1;
    public const int Cross_Marker = 2;
    public const int Headstone = 3;
    public const int Gravestone = 4;
    public const int Obelisk = 5;

    public const int Gold_Yin = 6;
    public const int Gold_Tombstone = 7;
    public const int Gold_Imp = 8;
    public const int Gold_Quartz = 9;
    public const int Gold_Fist = 10;

    public override int StyleCount => 11;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DustType = DustID.Ash;
        AddMapEntry(new Color(101, 90, 102));
        AddMapEntry(new Color(214, 36, 36));
    }

    public override TombstoneOverride? OverrideTombstoneDrop(Player player, bool gold, long coinsOwned) {
        if (player.position.Y < Main.UnderworldLayer * 16) {
            return null;
        }

        return null;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        switch (Main.tile[i, j].TileFrameX / 36) {
            // Gold Ash Tombstones color
            case > Gold_Yin: {
                    r = 0.5f;
                    g = 0.1f;
                    b = 0.1f;
                }
                break;

            // Ash Tombstones color
            default: {
                    r = 0.6f;
                    g = 0.3f;
                    b = 0.1f;
                }
                break;
        }
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX >= (36 * Gold_Yin) ? 1 : 0);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        Vector2 drawCoordinates = new Vector2(i, j) * 16f - Main.screenPosition + Helper.TileDrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        spriteBatch.Draw(AequusTextures.HellTombstones_Glow.Value, drawCoordinates, frame, Color.White * Helper.Wave(Main.GlobalTimeWrappedHourly, 0.5f, 0.8f));
    }
}
