using Aequus.Common.Tiles;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Statues;

internal class InstancedNPCStatue : InstancedModTile {
    private readonly ModNPC _modNPC;

    public InstancedNPCStatue(ModNPC modNPC) : base(modNPC.Name + "Statue", $"{modNPC.NamespaceFilePath()}/Tiles/{modNPC.Name}Statue") {
        _modNPC = modNPC;
    }

    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this, value: Item.sellPrice(copper: 60)));
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

    public override void HitWire(System.Int32 i, System.Int32 j) {
        TileObjectData objectData = TileObjectData.GetTileData(Main.tile[i, j]);
        System.Int32 width = objectData.Width;
        System.Int32 height = objectData.Height;
        System.Int32 coordinateWidth = objectData.CoordinateFullWidth;
        System.Int32 coordinateHeight = objectData.CoordinateFullHeight;

        System.Int32 x = i - Main.tile[i, j].TileFrameX % coordinateWidth / 18;
        System.Int32 y = j - Main.tile[i, j].TileFrameY % coordinateHeight / 18;

        for (System.Int32 k = y; k < y + width; k++) {
            for (System.Int32 l = x; l < x + height; l++) {
                Wiring.SkipWire(l, k);
            }
        }

        System.Single spawnX = (x + width * 0.5f) * 16;
        System.Single spawnY = (y + height * 0.65f) * 16;

        IEntitySource entitySource = new EntitySource_TileUpdate(x, y, context: Name);
        System.Int32 statueNPCId = _modNPC.Type;

        if (!Wiring.CheckMech(x, y, 30) || !NPC.MechSpawn(spawnX, spawnY, statueNPCId)) {
            return;
        }

        System.Int32 npcIndex = NPC.NewNPC(entitySource, (System.Int32)spawnX, (System.Int32)spawnY - 12, statueNPCId);
        if (npcIndex > -1) {
            NPC spawnedNPC = Main.npc[npcIndex];
            spawnedNPC.value = 0f;
            spawnedNPC.npcSlots = 0f;
            spawnedNPC.SpawnedFromStatue = true;
        }
    }
}
