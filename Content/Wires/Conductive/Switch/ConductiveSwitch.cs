using Aequus.Common.Tiles;
using Aequus.Common.Wires;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Wires.Conductive.Switch;

internal class ConductiveSwitch : InstancedTile {
    public ConductiveSwitch(string name) : base($"ConductiveSwitch{name}", $"{typeof(ConductiveSwitch).NamespaceFilePath()}/ConductiveSwitch{name}") {
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

    public override bool CanPlace(int i, int j) {
        return TileHelper.GetGemFramingAnchor(i, j).IsSolidTileAnchor();
    }

    public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
        width = 1;
        height = 1;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

    public override bool RightClick(int i, int j) {
        Main.tile[i, j].TileFrameX = (short)(Main.tile[i, j].TileFrameX == 22 ? 0 : 22);
        SoundEngine.PlaySound(SoundID.Mech, new Vector2(i, j).ToWorldCoordinates());

        var circuitSystem = ModContent.GetInstance<CircuitSystem>();
        circuitSystem.HitCircuit(i, j - 1);
        circuitSystem.HitCircuit(i, j + 1);
        circuitSystem.HitCircuit(i + 1, j);
        circuitSystem.HitCircuit(i - 1, j);
        return true;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        var gemFrameAnchor = TileHelper.GetGemFramingAnchor(i, j);
        if (gemFrameAnchor == TileAnchorDirection.Invalid) {
            if (Main.tile[i, j].WallType > WallID.None) {
                Main.tile[i, j].TileFrameY = 88;
            }
            else {
                WorldGen.KillTile(i, j);
            }
        }
        else {
            Main.tile[i, j].TileFrameY = (short)(TileHelper.GemFrame(gemFrameAnchor) * 22);
        }
        return false;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int item = TileLoader.GetItemDropFromTypeAndStyle(Type, Main.tile[i, j].TileFrameX / 36);
        if (item > 0) {
            return new Item[1] {
                new Item(item)
            };
        }

        return null;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 22, 22);
        var lightColor = Lighting.GetColor(i, j);
        var drawCoordinates = new Vector2(i * 16f - 2f, j * 16f - 2f) - Main.screenPosition + TileHelper.DrawOffset;

        spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        if (Main.InSmartCursorHighlightArea(i, j, out var actuallySelected)) {
            spriteBatch.Draw(TextureAssets.HighlightMask[Type].Value, drawCoordinates, frame, Colors.GetSelectionGlowColor(actuallySelected, (lightColor.R + lightColor.G + lightColor.B) / 3), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        return false;
    }
}
