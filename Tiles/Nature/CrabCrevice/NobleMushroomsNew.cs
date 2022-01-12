using AQMod.Common.Graphics;
using AQMod.Common.WorldGeneration;
using AQMod.Dusts.NobleMushrooms;
using AQMod.Items.Placeable.Nature;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public class NobleMushroomsNew : ModTile
    {
        public static int[] AnchorValidTiles => TileObjectData.GetTileData(ModContent.TileType<NobleMushroomsNew>(), 0, 0).AnchorValidTiles;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                TileID.Sand,
                TileID.Stone,
                TileID.Ebonstone,
                TileID.Crimstone,
                TileID.Pearlstone,
                TileID.BlueMoss,
                TileID.BrownMoss,
                TileID.GreenMoss,
                TileID.LavaMoss,
                TileID.PurpleMoss,
                TileID.RedMoss,
                ModContent.TileType<ArgonMossSand>(),
                ModContent.TileType<KryptonMossSand>(),
                ModContent.TileType<XenonMossSand>(),
                ModContent.TileType<SedimentSand>(),
            };
            TileObjectData.addTile(Type);
            var name = CreateMapEntryName("NobleMushrooms");
            AddMapEntry(new Color(208, 0, 126), name);
            AddMapEntry(new Color(144, 254, 2), name);
            AddMapEntry(new Color(0, 197, 208), name);
            disableSmartCursor = true;
        }

        public override bool KillSound(int i, int j)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AQSound.LegacyPlay(SoundType.Custom, AQSound.Paths.NobleMushroomHit, new Vector2(i * 16, j * 16f), 0.5f, Main.rand.NextFloat(0.9f, 1.1f));
            }
            return false;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].frameX / 108 % 3);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            switch (Main.tile[i, j].frameX / 108)
            {
                default:
                    {
                        r = 0.52f;
                        g = 0f;
                        b = 0.31f;
                    }
                    break;

                case 1:
                    {
                        r = 0.36f;
                        g = 0.7f;
                        b = 0f;
                    }
                    break;

                case 2:
                    {
                        r = 0f;
                        g = 0.49f;
                        b = 0.52f;
                    }
                    break;
            }
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            switch (Main.tile[i, j].frameX / 108)
            {
                default:
                    {
                        type = ModContent.DustType<ArgonDust>();
                    }
                    break;

                case 1:
                    {
                        type = ModContent.DustType<KryptonDust>();
                    }
                    break;

                case 2:
                    {
                        type = ModContent.DustType<XenonDust>();
                    }
                    break;
            }
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AQSound.LegacyPlay(SoundType.Custom, AQSound.Paths.NobleMushroomDestroy, new Vector2(i * 16, j * 16f), 0.5f, Main.rand.NextFloat(-0.1f, 0.1f));
            }
            switch (frameX / 108)
            {
                default:
                    {
                        Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<ArgonMushroom>());
                    }
                    break;

                case 1:
                    {
                        Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<KryptonMushroom>());
                    }
                    break;

                case 2:
                    {
                        Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<XenonMushroom>());
                    }
                    break;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].frameY < 18)
            {
                if ((Main.tile[i, j].frameX % 36) == 0)
                {
                    var drawPosition = new Vector2(i * 16f + 16f, j * 16f + 32f);
                    var drawFrame = new Rectangle(Main.tile[i, j].frameX, 0, 34, 34);
                    var drawOrigin = new Vector2(16f, 32f);
                    float rotation = 0f;
                    if (j < (int)Main.worldSurface)
                    {
                        bool applyWind = false;
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                if (Main.tile[k, l] == null)
                                {
                                    Main.tile[k, l] = new Tile();
                                }
                                if (!AQTile.WindFXHelper.WindBlocked(i + k, j + l))
                                {
                                    applyWind = true;
                                    k = 1;
                                    break;
                                }
                            }
                        }
                        if (applyWind)
                        {
                            float windPower = ((float)Math.Cos(Main.GlobalTime * MathHelper.Pi + i * 0.1f) + 1f) / 2f * 0.8f * Main.windSpeed;
                            drawPosition.X += windPower;
                            drawPosition.Y += windPower.Abs();
                            rotation = windPower * 0.1f;
                        }
                    }

                    Main.spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_Render")), drawPosition - Main.screenPosition + AQGraphics.TileZero, drawFrame,
                        Lighting.GetColor(drawPosition.TileX(), drawPosition.TileY()), rotation, drawOrigin, 1f, SpriteEffects.None, 0f);
                }
                if (!Main.gamePaused && Main.instance.IsActive)
                {
                    int chance = 60 - AQSystem.NobleMushroomsCount * 2;
                    if (chance < 0)
                        chance = 0;
                    if (Main.rand.NextBool(chance + 8))
                    {
                        switch (Main.tile[i, j].frameX / 108)
                        {
                            default:
                                {
                                    Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 2, ModContent.DustType<ArgonMist>());
                                }
                                break;

                            case 1:
                                {
                                    Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 2, ModContent.DustType<KryptonMist>());
                                }
                                break;

                            case 2:
                                {
                                    Dust.NewDust(new Vector2(i * 16f, j * 16f), 16, 2, ModContent.DustType<XenonMist>());
                                }
                                break;
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Place(int x, int y)
        {
            return Place(x, y, WorldGen.genRand.Next(9));
        }

        internal static bool Place(int x, int y, int style)
        {
            if (AQWorldGen.check2x2(x, y - 1))
            {
                WorldGen.PlaceTile(x, y - 1, ModContent.TileType<NobleMushroomsNew>(), true, true, -1, style);
                return true;
            }
            return false;
        }
    }
}