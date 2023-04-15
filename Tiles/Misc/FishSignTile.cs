using Aequus.Items.Placeable.Furniture.Interactable;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Misc {
    public class FishSignTile : ModTile {
        public static int maxWater = 1000;

        public override void SetStaticDefaults() {
            Main.tileSign[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.AnchorLeft = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(2);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(3);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(4);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.addAlternate(5);
            TileObjectData.addTile(Type);

            AddMapEntry(Helper.ColorFurniture, CreateMapEntryName());
            DustType = DustID.WoodFurniture;
            AdjTiles = new int[] { TileID.Signs, };
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
            return true;
        }

        public override void PlaceInWorld(int i, int j, Item item) {
            var player = Main.LocalPlayer;
            int signID = Sign.ReadSign(i, j, true);
            var sign = Main.sign[signID];
            string text = WriteWater(player, i, j);
            if (string.IsNullOrEmpty(text)) {
                sign.text += Language.GetTextValue("Mods.Aequus.MapObject.FishSignTile.NoLiquid");
            }
            else {
                sign.text += text;
            }
            text = WriteLayer(player, i, j);
            if (!string.IsNullOrEmpty(text)) {
                sign.text += "\n" + text;
            }
            text = WriteBiomes(player, i, j);
            if (!string.IsNullOrEmpty(text)) {
                sign.text += "\n" + text;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.ReadSign, number: signID);
            }
        }

        public static string WriteWater(Player player, int i, int j) {
            int waterX = 0;
            int waterY = 0;
            int liquidID = 0;
            for (int n = 2; n < 15; n++) {
                for (int k = 0; k < 30; k++) {
                    int m = k / 2 * (k % 2 == 0 ? 1 : -1);
                    var t = Main.tile[i + m, j + n];
                    if (!t.IsFullySolid() && t.LiquidAmount > 0) {
                        waterX = i + m;
                        waterY = j + n;
                        liquidID = t.LiquidType;
                        goto CheckWater;
                    }
                }
            }
            if (waterX == 0 || waterY == 0)
                return null;
            CheckWater:
            int waterAmt = 0;
            var l = new List<Point>() { new Point(waterX, waterY), };
            var alreadyChecked = new List<Point>();
            while (waterAmt <= maxWater && l.Count > 0) {
                var p = l[0];
                if (!WorldGen.InWorld(p.X, p.Y, 10)) {
                    alreadyChecked.Add(l[0]);
                    l.RemoveAt(0);
                    continue;
                }
                var t = Main.tile[p];
                if (!t.IsFullySolid() && t.LiquidAmount > 0 && t.LiquidType == liquidID && !alreadyChecked.Contains(p)) {
                    waterAmt++;
                    l.Add(new Point(p.X + 1, p.Y));
                    l.Add(new Point(p.X - 1, p.Y));
                    l.Add(new Point(p.X, p.Y + 1));
                    l.Add(new Point(p.X, p.Y - 1));
                }
                alreadyChecked.Add(l[0]);
                l.RemoveAt(0);
            }
            return TextHelper.GetTextValueWith("MapObject.FishSignTile.LiquidAmount", new { LiquidName = TextHelper.LiquidName(liquidID), WaterAmount = waterAmt >= maxWater ? $"{maxWater}+" : waterAmt.ToString(), });
        }
        public static string WriteLayer(Player player, int i, int j) {
            string layer = "Bestiary_Biomes.Surface";
            if (player.ZoneUnderworldHeight) {
                layer = "Bestiary_Biomes.TheUnderworld";
            }
            else if (player.ZoneRockLayerHeight) {
                layer = "Bestiary_Biomes.Caverns";
            }
            else if (player.ZoneDirtLayerHeight) {
                layer = "Bestiary_Biomes.Underground";
            }
            else if (player.ZoneSkyHeight) {
                layer = "Bestiary_Biomes.Sky";
            }
            return TextHelper.GetTextValueWith("MapObject.FishSignTile.DepthLayer", new { Layer = Language.GetTextValue(layer), });
        }
        public static string WriteBiomes(Player player, int i, int j) {
            var biomes = player.GetStringListOfBiomes();
            if (biomes.Count > 0) {
                string biomesText = Language.GetTextValue(biomes[0]);
                for (int k = 1; k < biomes.Count; k++) {
                    biomesText += $", {Language.GetTextValue(biomes[k])}";
                }
                return TextHelper.GetTextValueWith("MapObject.FishSignTile.Biomes", new { Biomes = biomesText, });
            }
            return null;
        }

        public override bool RightClick(int i, int j) {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            Sign.KillSign(i, j);
        }
    }
}