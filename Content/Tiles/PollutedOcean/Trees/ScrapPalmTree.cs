using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Entities.Tiles.Components;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Content.Tiles.PollutedOcean.Trees;

public class ScrapPalmTree : ModTile, IOverridePlacement, IAddRecipes {
    public ModItem DropItem { get; protected set; }
    public ModTile TreeTop { get; protected set; }
    public ModItem TreeTopItem { get; protected set; }

    public override void Load() {
        DropItem = new InstancedTileItem(this);
        Mod.AddContent(DropItem);

        TreeTop = new ScrapPalmTreeTop(this);
        Mod.AddContent(TreeTop);
        TreeTopItem = (TreeTop as ScrapPalmTreeTop).DropItem;
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.CoordinateWidth = 20;
        TileObjectData.newTile.CoordinateHeights = [20];
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.AnchorAlternateTiles = [Type];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 7;
        TileObjectData.newTile.RandomStyleRange = 6;

        TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
        TileObjectData.newSubTile.RandomStyleRange = 0;
        TileObjectData.newSubTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
        TileObjectData.addSubTile(0, 7);

        TileObjectData.addTile(Type);

        MineResist = 8f;
        DustType = DustID.Iron;
        AddMapEntry(new Color(192, 187, 151), Language.GetText("MapObject.PalmTree"));
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        Tile tile = Framing.GetTileSafely(i, j);
        Tile below = Framing.GetTileSafely(i, j + 1);
        Tile above = Framing.GetTileSafely(i, j - 1);
        if (above.HasTile) {
            tile.TileFrameY = 22;
        }
        if (below.IsSolid()) {
            tile.TileFrameX = 0;
        }
        else {
            tile.TileFrameX = (short)(22 * WorldGen.genRand.Next(1, 7));
        }
        return true;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        yield return new Item(DropItem.Type);
    }

    void IOverridePlacement.OverridePlacementCheck(Player player, Tile targetTile, Item item, ref int tileToCreate, ref int previewPlaceStyle, ref bool? overrideCanPlace, ref int? forcedRandom) {
        int x = Player.tileTargetX;
        int y = Player.tileTargetY;

        Tile belowTile = Framing.GetTileSafely(x, y + 1);
        previewPlaceStyle = belowTile.IsSolid() ? 0 : 1;
    }

    void IAddRecipes.AddRecipes(Mod mod) { AddRecipes(); }
    public virtual void AddRecipes() {
        DropItem.CreateRecipe()
            .AddIngredient(ModContent.GetInstance<ScrapTree>().DropItem.Type)
            .AddIngredient(ItemID.PalmWood)
            .Register();

        TreeTopItem.CreateRecipe()
            .AddIngredient(DropItem.Type, 15)
            .Register();
    }
}
