using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Entities.Tiles;
using Aequu2.Old.Content.Items.Potions.Prefixes.BoundedPotions;
using Aequu2.Old.Content.Items.Potions.Prefixes.EmpoweredPotions;
using Aequu2.Old.Content.Items.Potions.Prefixes.SplashPotions;
using Aequu2.Old.Content.Items.Potions.Prefixes.StuffedPotions;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequu2.Old.Content.Tiles.Herbs.HangingPots;

internal class HangingPot : ModTile {
    public override void Load() {
        int style = 0;
        AddItem("Moray", ItemID.PotSuspendedBlinkroot, () => ModContent.GetInstance<SplashPrefix>().Item.Type);
        AddItem("Mistral", ItemID.PotSuspendedFireblossom, () => ModContent.GetInstance<EmpoweredPrefix>().Item.Type);
        AddItem("Manacle", ItemID.PotSuspendedBlinkroot, () => ModContent.GetInstance<BoundedPrefix>().Item.Type);
        AddItem("Moonflower", ItemID.PotSuspendedDeathweedCrimson, () => ModContent.GetInstance<StuffedPrefix>().Item.Type);

        void AddItem(string name, int recipeSortIdTarget, Func<int> getIngredientId) {
            InstancedTileItem hangingPlantItem = new InstancedTileItem(this, style, name, rarity: ItemRarityID.White, value: Item.silver, researchSacrificeCount: 1, journeyOverride: new JourneySortByTileId(TileID.PotsSuspended));

            Mod.AddContent(hangingPlantItem);

            style++;
            Aequu2.OnAddRecipes += () => {
                hangingPlantItem.CreateRecipe()
                    .AddIngredient(ItemID.PotSuspended)
                    .AddIngredient(getIngredientId())
                    .Register()
                    .SortAfterFirstRecipesOf(recipeSortIdTarget);
            };
        }
    }

    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.DrawYOffset = -2;
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.CoordinateHeights = new int[3] { 16, 16, 16 };
        TileObjectData.newTile.StyleWrapLimit = 111;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.PlatformNonHammered, TileObjectData.newTile.Width, 0);
        TileObjectData.newAlternate.DrawYOffset = -10;
        TileObjectData.addAlternate(0);
        TileObjectData.addTile(Type);

        VineDrawing.VineLength[Type] = new(2, 3);

        AddMapEntry(Color.White);
        DustType = -1;
    }

    //public override ushort GetMapOption(int i, int j) {
    //    return (ushort)(Main.tile[i, j].TileFrameY / 54);
    //}

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        Tile tile = Main.tile[i, j];
        TileObjectData data = TileObjectData.GetTileData(tile);
        int x = i - tile.TileFrameX / 18 % data.Width;
        int topLeftY = j - tile.TileFrameY / 18 % data.Height;
        if (WorldGen.IsBelowANonHammeredPlatform(x, topLeftY)) {
            offsetY -= 8;
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
            VineDrawing.DrawVine(i, j);
        }
        return false;
    }
}
