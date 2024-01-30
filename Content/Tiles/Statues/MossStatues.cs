﻿using Aequus.Common.Tiles;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Statues;

public class MossStatues : ModTile {
    public const System.Int32 STYLE_ARGON = 0;
    public const System.Int32 STYLE_KRYPTON = 1;
    public const System.Int32 STYLE_NEON = 2;
    public const System.Int32 STYLE_XENON = 3;

    public override void Load() {
        AddItem(STYLE_ARGON, "Argon");
        AddItem(STYLE_KRYPTON, "Krypton");
        AddItem(STYLE_NEON, "Neon");
        AddItem(STYLE_XENON, "Xenon");

        void AddItem(System.Int32 style, System.String name) {
            Mod.AddContent(new InstancedTileItem(this, style, name, value: Item.sellPrice(copper: 60)));
        }
    }

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new System.Int32[] { 16, 16, 18 };
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleHorizontal = true;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);

        TileObjectData.addTile(Type);

        DustType = DustID.Silver;

        AddMapEntry(CommonColor.TILE_STATUE, Language.GetText("MapObject.Statue"));
    }
}
