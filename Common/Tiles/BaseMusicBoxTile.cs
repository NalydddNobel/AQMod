﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public abstract class BaseMusicBoxTile : ModTile {
    public abstract int MusicBoxItemId { get; }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        AddMapEntry(new(200, 200, 200), TextHelper.GetDisplayName(ModContent.GetModItem(MusicBoxItemId)));
    }

    public override void MouseOver(int i, int j) {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = MusicBoxItemId;
    }
}