using Aequus.Common;
using Aequus.Content.CarpenterBounties;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    internal class Tester : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
        }

        public override bool? UseItem(Player player)
        {
            Main.NewText(CarpenterSystem.BountiesByID[0].CheckConditions(Utils.CenteredRectangle(player.Center.ToTileCoordinates().ToVector2(), new Vector2(30f, 20f)).Fluffize(10), out string dnc, null));
            return true;
        }

        private void DebugCompressedTile(TileMapCache tileMap)
        {
            Main.NewText("================");
            var compressedTiles = tileMap.CompressTileArray();
            long max = compressedTiles.Length;
            using (var r = new BinaryReader(new MemoryStream(compressedTiles)))
            {
                for (int index = 0; index < compressedTiles.Length; index++)
                {
                    if (index <= 3)
                    {
                        if (index == 3)
                        {
                            Main.NewText(r.ReadInt32(), new Color(200, 200, 255, 255));
                        }
                        continue;
                    }
                    if (index <= 7)
                    {
                        if (index == 7)
                        {
                            int amt = r.ReadInt32();
                            for (int i = 0; i < amt; i++)
                            {
                                r.ReadInt32();
                                Main.NewText(r.ReadString(), Color.Green);
                            }
                            Main.NewText(amt, new Color(200, 200, 255, 255));
                        }
                        continue;
                    }
                    if (index <= 15)
                    {
                        if (index == 15)
                        {
                            max = r.ReadInt64();
                            Main.NewText(max, new Color(200, 200, 255, 255));
                        }
                        continue;
                    }
                    if (index <= 17)
                    {
                        if (index == 17)
                        {
                            var tileID = r.ReadUInt16();
                            if (tileID < Main.maxTileSets)
                            {
                                Main.NewText($"TileID.{TileID.Search.GetName(tileID)}: {tileID}", new Color(100, 255, 100, 255));
                            }
                            else
                            {
                                Main.NewText($"{TileID.Search.GetName(tileID)}: {tileID}", new Color(100, 255, 100, 255));
                            }
                        }
                        continue;
                    }
                    if (index <= 23)
                    {
                        if (index == 18)
                        {
                            Main.NewText(r.ReadByte(), new Color(0, 0, 255, 255));
                        }
                        else if (index == 19)
                        {
                            var bb2 = (BitsByte)r.ReadByte();
                            bb2[6] = false;
                            bb2[7] = false;
                            string name = "Unknown";
                            if (bb2 == LiquidID.Water)
                                name = nameof(LiquidID.Water);
                            if (bb2 == LiquidID.Lava)
                                name = nameof(LiquidID.Lava);
                            if (bb2 == LiquidID.Honey)
                                name = nameof(LiquidID.Honey);
                            Main.NewText($"{name}: {(byte)bb2}", new Color(0, 0, 255, 255));

                            // wall frames
                            Main.NewText(r.ReadInt16(), new Color(0, 0, 255, 255));
                            Main.NewText(r.ReadInt16(), new Color(0, 0, 255, 255));
                        }
                        continue;
                    }
                    byte val = compressedTiles[index];
                    var bb = (BitsByte)val;
                    string text = "";
                    for (int i = 0; i < 8; i++)
                    {
                        text += bb[i] ? "1" : "0";
                    }
                    Main.NewText($"{index}: {text}");
                    if (index > max)
                    {
                        break;
                    }
                }
            }
            Main.NewText(compressedTiles.Length, Color.Red);
        }
    }
}