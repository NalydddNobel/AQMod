using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Calendar;

public class CalendarTile : ModTile {
    public static bool Nearby;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = DustID.BubbleBurst_White;
        AddMapEntry(new Color(85, 85, 85), Lang.GetItemName(ModContent.ItemType<Calendar>()));
    }

    public override void NearbyEffects(int i, int j, bool closer) {
        Nearby = true;
    }
}