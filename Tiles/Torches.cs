using AQMod.Assets.Textures;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Items.Placeable.Torches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class Torches : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;

            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124 };
            TileObjectData.addAlternate(1);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124 };
            TileObjectData.addAlternate(2);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);

            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(3);

            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(4);

            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(5);

            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Torch");
            AddMapEntry(new Color(0, 0, 255), name);
            dustType = ModContent.DustType<ArgonDust>();
            drop = ModContent.ItemType<UltrabrightRedTorch>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Torches };
            torch = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = Main.rand.Next(1, 3);
        }

        private const int TorchIntensityDistance = 300;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 66)
            {
                switch (tile.frameY / 22)
                {
                    case 0:
                    {
                        r = 1f;
                        g = 0f;
                        b = 0f;
                    }
                    break;

                    case 1:
                    {
                        r = 0f;
                        g = 1f;
                        b = 0f;
                    }
                    break;

                    case 2:
                    {
                        r = 0f;
                        g = 0f;
                        b = 1f;
                    }
                    break;

                    case 3:
                    {
                        float time = Main.GlobalTime;
                        r = ((float)Math.Sin(time) + 1f) / 4f;
                        g = ((float)Math.Cos(time) + 1f) / 16f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;

                        if (Main.tile[i, j].liquid > 0)
                        {
                            float intensityMult = 0.1f;
                            var screenCenter = DrawUtils.ScreenCenter;
                            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
                            var distance = (screenCenter - screenPosition).Length();
                            if (distance < TorchIntensityDistance)
                            {
                                intensityMult += 1f - distance / TorchIntensityDistance;
                            }
                            r *= intensityMult;
                            g *= intensityMult;
                            b *= intensityMult;
                        }
                    }
                    break;

                    case 4:
                    {
                        float time = Main.GlobalTime;
                        r = ((float)Math.Sin(time) + 1f) / 16f;
                        g = ((float)Math.Cos(time) + 1f) / 4f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;

                        if (Main.tile[i, j].liquid > 0)
                        {
                            float intensityMult = 0.1f;
                            var screenCenter = DrawUtils.ScreenCenter;
                            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
                            var distance = (screenCenter - screenPosition).Length();
                            if (distance < TorchIntensityDistance)
                            {
                                intensityMult += 1f - distance / TorchIntensityDistance;
                            }
                            r *= intensityMult;
                            g *= intensityMult;
                            b *= intensityMult;
                        }
                    }
                    break;

                    case 5:
                    {
                        float time = Main.GlobalTime;
                        r = ((float)Math.Sin(time) + 1f) / 16f;
                        g = ((float)Math.Cos(time) + 1f) / 16f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 4f;

                        if (Main.tile[i, j].liquid > 0)
                        {
                            float intensityMult = 0.1f;
                            var screenCenter = DrawUtils.ScreenCenter;
                            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
                            var distance = (screenCenter - screenPosition).Length();
                            if (distance < TorchIntensityDistance)
                            {
                                intensityMult += 1f - distance / TorchIntensityDistance;
                            }
                            r *= intensityMult;
                            g *= intensityMult;
                            b *= intensityMult;
                        }
                    }
                    break;
                }
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            offsetY = 0;
            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 2;
                if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                {
                    offsetY = 4;
                }
            }
        }

        public override bool Drop(int i, int j)
        {
            switch (Main.tile[i, j].frameY / 22)
            {
                case 0:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<UltrabrightRedTorch>());
                }
                break;

                case 1:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<UltrabrightGreenTorch>());
                }
                break;

                case 2:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<UltrabrightBlueTorch>());
                }
                break;

                case 3:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<ExoticRedTorch>());
                }
                break;

                case 4:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<ExoticGreenTorch>());
                }
                break;

                case 5:
                {
                    Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<ExoticBlueTorch>());
                }
                break;
            }
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int torchFrameY = Main.tile[i, j].frameY / 22;
            switch (torchFrameY)
            {
                case 3:
                case 4:
                case 5:
                {
                    float intensityMult = 1f;
                    if (torchFrameY == 3 || torchFrameY == 4 || torchFrameY == 5) // seems useless for now
                    {
                        if (Main.tile[i, j].liquid > 0)
                        {
                            intensityMult = 0.025f;
                            var screenCenter = DrawUtils.ScreenCenter;
                            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
                            var distance = (screenCenter - screenPosition).Length();
                            if (distance < TorchIntensityDistance)
                            {
                                intensityMult += (1f - distance / TorchIntensityDistance) * 1.25f;
                            }
                        }
                    }
                    ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);
                    Color color = new Color(100, 100, 100, 0) * intensityMult;
                    int frameX = Main.tile[i, j].frameX;
                    int frameY = Main.tile[i, j].frameY;
                    int width = 20;
                    int offsetY = 0;
                    int height = 20;
                    if (WorldGen.SolidTile(i, j - 1))
                    {
                        offsetY = 2;
                        if (WorldGen.SolidTile(i - 1, j + 1) || WorldGen.SolidTile(i + 1, j + 1))
                        {
                            offsetY = 4;
                        }
                    }
                    Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                    if (Main.drawToScreen)
                    {
                        zero = Vector2.Zero;
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        float x = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f * intensityMult;
                        float y = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f * intensityMult;
                        Main.spriteBatch.Draw(DrawUtils.Textures.Extras[ExtraID.Torches], new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + x, j * 16 - (int)Main.screenPosition.Y + offsetY + y) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    }
                }
                break;
            }
        }
    }
}