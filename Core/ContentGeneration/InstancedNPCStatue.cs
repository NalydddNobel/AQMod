using Terraria.DataStructures;
using Terraria.Enums;
using tModLoaderExtended.Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Core.ContentGeneration;

internal class InstancedNPCStatue : InstancedModTile {
    private readonly ModNPC _modNPC;

    public InstancedNPCStatue(ModNPC modNPC) : base(modNPC.Name + "Statue", $"{modNPC.NamespaceFilePath()}/Tiles/{modNPC.Name}Statue") {
        _modNPC = modNPC;
    }

    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this, value: Item.sellPrice(copper: 60), journeyOverride: new JourneySortByTileId(TileID.Statues)));
    }

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleHorizontal = true;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);

        TileObjectData.addTile(Type);

        DustType = DustID.Silver;

        AddMapEntry(CommonColor.MapStatue, Language.GetText("MapObject.Statue"));
    }

    public override void HitWire(int i, int j) {
        TileObjectData objectData = TileObjectData.GetTileData(Main.tile[i, j]);
        int width = objectData.Width;
        int height = objectData.Height;
        int coordinateWidth = objectData.CoordinateFullWidth;
        int coordinateHeight = objectData.CoordinateFullHeight;

        int x = i - Main.tile[i, j].TileFrameX % coordinateWidth / 18;
        int y = j - Main.tile[i, j].TileFrameY % coordinateHeight / 18;

        for (int k = y; k < y + width; k++) {
            for (int l = x; l < x + height; l++) {
                Wiring.SkipWire(l, k);
            }
        }

        float spawnX = (x + width * 0.5f) * 16;
        float spawnY = (y + height * 0.65f) * 16;

        IEntitySource entitySource = new EntitySource_TileUpdate(x, y, context: Name);
        int statueNPCId = _modNPC.Type;

        if (!Wiring.CheckMech(x, y, 30) || !NPC.MechSpawn(spawnX, spawnY, statueNPCId)) {
            return;
        }

        int npcIndex = NPC.NewNPC(entitySource, (int)spawnX, (int)spawnY - 12, statueNPCId);
        if (npcIndex > -1) {
            NPC spawnedNPC = Main.npc[npcIndex];
            spawnedNPC.value = 0f;
            spawnedNPC.npcSlots = 0f;
            spawnedNPC.SpawnedFromStatue = true;
        }
    }
}
