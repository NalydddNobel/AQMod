using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.CrabCrevice
{
    public sealed class WeepingVine : ModTile
    {
        public const int MaxLength = 15;

        public override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 2;
            soundType = SoundID.Grass;
            soundStyle = 1;
            AddMapEntry(new Color(20, 100, 10));
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
            if (!Main.tile[i, j + 1].active() || Main.tile[i, j + 1].type != Type) // bottom 
            {
                if (Main.tile[i, j - 1].type != Type)
                {
                    switch (WorldGen.genRand.Next(3))
                    {
                        default:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = (short)(WorldGen.genRand.NextBool() ? 54 : 72);
                            break;

                        case 1:
                            {
                                Main.tile[i, j].frameX = 18;
                                Main.tile[i, j].frameY = (short)(WorldGen.genRand.NextBool() ? 72 : 90);
                            }
                            break;

                        case 2:
                            {
                                Main.tile[i, j].frameX = 36;
                                Main.tile[i, j].frameY = 54;
                            }
                            break;
                    }
                }
                else
                {
                    if (Main.tile[i, j].frameX == 18)
                    {
                        Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 18 ? 90 : 72);
                    }
                    else if (Main.tile[i, j].frameX == 36)
                    {
                        Main.tile[i, j].frameY = 54;
                    }
                    else
                    {
                        Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 18 ? 72 : 54);
                    }
                }
            }
            else if (Main.tile[i, j - 1].type != Type) // top
            {
                Main.tile[i, j].frameX = Main.tile[i, j + 1].frameX;
                if (Main.tile[i, j].frameX == 18)
                {
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j + 1].frameY == 18 ? 0 : 18);
                }
                else if (Main.tile[i, j].frameX == 36)
                {
                    Main.tile[i, j].frameY = 0;
                }
                else
                {
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j + 1].frameY == 18 ? 0 : 18);
                }
            }
            else
            {
                Main.tile[i, j].frameX = Main.tile[i, j - 1].frameX;
                if (Main.tile[i, j].frameX == 18)
                {
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 18 || Main.tile[i, j - 1].frameY == 90 ? 0 : 18);
                }
                else if (Main.tile[i, j].frameX == 36)
                {
                    Main.tile[i, j].frameY = 18;
                }
                else
                {
                    Main.tile[i, j].frameY = (short)(Main.tile[i, j - 1].frameY == 18 || Main.tile[i, j - 1].frameY == 72 ? 0 : 18);
                }
            }
            return false;
        }

        public static bool GrowVine(int i, int j)
        {
            for (int l = j + 1; l < j + MaxLength; l++)
            {
                if (!Main.tile[i, l].active() && (l == j + 1 || Main.tile[i, l - 1].type == ModContent.TileType<WeepingVine>()))
                {
                    Main.tile[i, l].active(active: true);
                    Main.tile[i, l].type = (ushort)ModContent.TileType<WeepingVine>();
                    if (l != j + 1)
                    {
                        Main.tile[i, l].frameX = Main.tile[i, l - 1].frameX;
                    }
                    WorldGen.SquareTileFrame(i, l, resetFrame: true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, l, 1, TileChangeType.None);
                    }
                    break;
                }
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