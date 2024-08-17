using Aequus.Common.ContentTemplates.Generic;
using Aequus.Content.Biomes.Meadows.Tiles;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Maps.CartographyTable;

public class CartographyTable : ModTile, IAddRecipes {
    private readonly ModItem Item;
    public CartographyTable() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.Origin = new(1, 3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.addTile(Type);
        AddMapEntry(Color.SaddleBrown);
        AddMapEntry(new Color(15, 29, 42)); // Empty
        AddMapEntry(new Color(35, 62, 97)); // Wall
        AddMapEntry(new Color(39, 71, 145)); // BG Object
        AddMapEntry(new Color(69, 95, 186)); // Block
        HitSound = SoundID.Dig;
        DustType = DustID.WoodFurniture;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    void IAddRecipes.AddRecipes(Aequus aequus) {
        Item.CreateRecipe()
            .AddIngredient(ModContent.GetInstance<MeadowWood>().Item, 25)
            .AddTile(TileID.Sawmill)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.GlassKiln);
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        int offset = Math.Clamp(WorldGen.GetWorldSize(), 0, 2);
        drawData.addFrX = offset * 72;
    }

    public override bool RightClick(int i, int j) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModContent.GetInstance<ServerMapRequestPacket>().SendReset(Main.myPlayer);
        }
        return true;
    }
}
