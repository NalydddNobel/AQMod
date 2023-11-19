using Aequus.Common.Tiles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CrabPots;
public class CrabPot : ModTile, IModifyPlacementPreview {
    public const int CopperPot = 0;
    public const int TinPot = 1;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
        TileObjectData.newTile.DrawYOffset = -12;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 22 };
        TileObjectData.newTile.AnchorTop = AnchorData.Empty;
        TileObjectData.newTile.AnchorRight = AnchorData.Empty;
        TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.addTile(Type);
        DustType = DustID.Iron;
        AddMapEntry(new(105, 186, 181), TextHelper.GetDisplayName<CrabPotCopperItem>());
        AddMapEntry(new(152, 186, 188), TextHelper.GetDisplayName<CrabPotTinItem>());
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX / 36);
    }

    public override void RandomUpdate(int i, int j) {
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var data = TileObjectData.GetTileData(Main.tile[i, j]);
        int yFrame = Main.tile[i, j].TileFrameY % 42 / 18;
        int left = i - Main.tile[i, j].TileFrameX % 36 / 18;
        int top = j - yFrame;
        data.StyleHorizontal = true;
        int waterYOffset = 16 - Main.tile[left, top].LiquidAmount / 16 + (int)(MathF.Sin(Main.GameUpdateCount / 40f) * 2.5f);
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, new Vector2(i, j).ToWorldCoordinates(0f, 0f) + new Vector2(data.DrawXOffset, data.DrawYOffset + waterYOffset) + DrawHelper.TileDrawOffset - Main.screenPosition, new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, data.CoordinateWidth, data.CoordinateHeights[yFrame]), Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        return false;
    }
}