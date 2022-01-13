using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public sealed class WeepingVine : ModTile
    {
        private const int VineLength = 15;

        public override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 2;
            soundType = SoundID.Grass;
            soundStyle = 1;
            AddMapEntry(new Color(65, 56, 83));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (WorldGen.genRand.Next(2) == 0 && Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)].cordage)
            {
                Item.NewItem(new Vector2(i * 16f + 8f, j * 16f + 8f), 2996);
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (!Main.tile[i, j - 1].active())
            {
                WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: false);
                return false;
            }
            if (!Main.tile[i, j + 1].active())
            {
                if (Main.tile[i, j - 1].frameX == 18)
                {
                    Main.tile[i, j].frameX = 18;
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 0 ? 90 : 72);
                }
                else if (Main.tile[i, j - 1].frameX == 36)
                {
                    if (Main.tile[i, j - 1].frameY != 18)
                    {
                        Main.tile[i, j].frameY = 18;
                    }
                    Main.tile[i, j].frameX = 36;
                    Main.tile[i, j].frameY = 54;
                }
                else
                {
                    Main.tile[i, j].frameX = 0;
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 18 ? 72 : 54);
                }
            }
            else if (Main.tile[i, j + 1].type != Type)
            {
                int x = WorldGen.genRand.Next(3);
                if (x == 1)
                {
                    Main.tile[i, j].frameX = 18;
                    Main.tile[i, j].frameY = (short)(WorldGen.genRand.NextBool() ? 90 : 72);
                }
                else if (x == 2)
                {
                    Main.tile[i, j].frameX = 36;
                    Main.tile[i, j].frameY = 54;
                }
                else
                {
                    Main.tile[i, j].frameX = 0;
                    Main.tile[i, j].frameY = (short)(WorldGen.genRand.NextBool() ? 72 : 54);
                }
            }
            return false;
        }

        public static bool GrowVine(int i, int j)
        {
            int length = 1;
            for (int l = j + 1; l < j + VineLength; l++)
            {
                if (!Main.tile[i, l].active())
                {
                    if (l == j + 1)
                    {
                        Main.tile[i, l].active(active: true);
                        Main.tile[i, l].type = (ushort)ModContent.TileType<WeepingVine>();

                        int x = WorldGen.genRand.Next(3);
                        if (x == 1)
                        {
                            Main.tile[i, l].frameX = 18;
                            Main.tile[i, l].frameY = (short)(WorldGen.genRand.NextBool() ? 90 : 72);
                        }
                        else if (x == 2)
                        {
                            Main.tile[i, l].frameX = 36;
                            Main.tile[i, l].frameY = 54;
                        }
                        else
                        {
                            Main.tile[i, l].frameX = 0;
                            Main.tile[i, l].frameY = (short)(WorldGen.genRand.NextBool() ? 72 : 54);
                        }
                    }
                    else if (l == j + 2 && Main.tile[i, l - 1].type == ModContent.TileType<WeepingVine>())
                    {
                        Main.tile[i, l].active(active: true);
                        Main.tile[i, l].type = (ushort)ModContent.TileType<WeepingVine>();

                        Main.tile[i, l].frameX = Main.tile[i, l - 1].frameX;
                        Main.tile[i, l].frameY = Main.tile[i, l - 1].frameY;

                        if (Main.tile[i, l].frameX == 18)
                        {
                            Main.tile[i, l - 1].frameY = (short)(Main.tile[i, l].frameY == 90 ? 18 : 0);
                        }
                        else if (Main.tile[i, l].frameX == 36)
                        {
                            Main.tile[i, l - 1].frameY = 18;
                        }
                        else
                        {
                            Main.tile[i, l - 1].frameY = (short)(Main.tile[i, l].frameY == 90 ? 18 : 0); // same as frame 1, could possibly merge?
                        }
                    }
                    else if (Main.tile[i, l - 1].type == ModContent.TileType<WeepingVine>())
                    {
                        Main.tile[i, l].active(active: true);
                        Main.tile[i, l].type = (ushort)ModContent.TileType<WeepingVine>();

                        for (int m = 0; m < length; m++)
                        {
                            Main.tile[i, l - m].frameX = Main.tile[i, l - m - 1].frameX;
                            Main.tile[i, l - m].frameY = Main.tile[i, l - m - 1].frameY;
                        }

                        int belowX = Main.tile[i, j + 2].frameX / 18;
                        int x = belowX;
                        if (WorldGen.genRand.NextBool())
                        {
                            x = WorldGen.genRand.Next(3);
                        }

                        if (x == belowX)
                        {
                            if (belowX == 1)
                            {
                                Main.tile[i, j + 1].frameX = 18;
                                Main.tile[i, j + 2].frameX = 18;
                                Main.tile[i, j + 1].frameY = (short)(Main.tile[i, j + 2].frameY == 18 ? 0 : 18);
                            }
                            else if (belowX == 2)
                            {
                                Main.tile[i, j + 1].frameX = 36;
                                Main.tile[i, j + 2].frameX = 36;
                                Main.tile[i, j + 1].frameY = 0;
                                Main.tile[i, j + 2].frameY = 18;
                            }
                            else
                            {
                                Main.tile[i, j + 1].frameX = 0;
                                Main.tile[i, j + 2].frameX = 0;
                                Main.tile[i, j + 1].frameY = (short)(Main.tile[i, j + 2].frameY == 18 ? 0 : 18);
                            }
                        }
                        else if (x == 1)
                        {
                            Main.tile[i, j + 1].frameX = 18;
                            Main.tile[i, j + 1].frameY = (short)(WorldGen.genRand.NextBool() ? 18 : 0);
                            if (belowX == 2)
                            {
                                Main.tile[i, j + 2].frameX = 36;
                                Main.tile[i, j + 2].frameY = 0;
                            }
                            else
                            {
                                Main.tile[i, j + 2].frameX = 18;
                                Main.tile[i, j + 2].frameY = (short)(Main.tile[i, j + 1].frameY == 18 ? 36 : 54);
                            }
                        }
                        else if (x == 2)
                        {
                            Main.tile[i, j + 1].frameX = 36;
                            Main.tile[i, j + 1].frameY = 0;
                            Main.tile[i, j + 2].frameX = 36;
                            Main.tile[i, j + 2].frameY = 36;
                        }
                        else
                        {
                            Main.tile[i, j + 1].frameX = 0;
                            Main.tile[i, j + 1].frameY = (short)(WorldGen.genRand.NextBool() ? 18 : 0);
                            if (belowX == 2)
                            {
                                Main.tile[i, j + 2].frameX = 36;
                                Main.tile[i, j + 2].frameY = 0;
                            }
                            else
                            {
                                Main.tile[i, j + 2].frameX = 18;
                                Main.tile[i, j + 2].frameY = 36;
                            }
                        }
                        return true;
                    }
                    break;
                }
                length++;
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j - 1].type != Type)
            {
                TileUtils.Rendering.RenderSwayingVine(i, j, Type);
            }
            return false;
        }
    }
}