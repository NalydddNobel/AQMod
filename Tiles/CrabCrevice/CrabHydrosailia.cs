using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice
{
    public class CrabHydrosailia : ModTile
    {
        public override void Load()
        {
            On.Terraria.Liquid.DelWater += Liquid_DelWater;
        }

        private static void Liquid_DelWater(On.Terraria.Liquid.orig_DelWater orig, int l)
        {
            int x = Main.liquid[l].x;
            int y = Main.liquid[l].y;

            orig(l);

            if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<CrabHydrosailia>())
            {
                CheckAlgae(x, y);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.JungleGrass;
            HitSound = SoundID.Grass;

            AddMapEntry((Color.Teal * 0.8f).UseA(255).SaturationMultiply(0.5f));
        }

        public override void RandomUpdate(int i, int j)
        {
            CheckAlgae(i, j);
            if (WorldGen.genRand.NextBool())
            {
                if (j + 5 < Main.maxTilesY - 5)
                {
                    for (int k = 1; k < 5; k++)
                    {
                        if (Main.tile[i, j + k].HasTile)
                        {
                            return;
                        }
                    }
                    WorldGen.PlaceTile(i, j + 1, Type, mute: true, style: WorldGen.genRand.Next(6));
                    WorldGen.TileFrame(i, j, resetFrame: true);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, i, j);
                    }
                }
            }
            if (!Main.tile[i, j - 1].HasTile)
            {
                if (j + 1 < Main.maxTilesY - 5)
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        if (!Main.tile[i + k, j].HasTile)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                WorldGen.PlaceTile(i + k, j, Type, mute: true, style: WorldGen.genRand.Next(6));
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendTileSquare(-1, i + k, j - 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CheckAlgae(i, j);
            if (resetFrame)
            {
                if (j + 1 > Main.maxTilesY - 5 || j - 1 < 5)
                {
                    return true;
                }
                if (!Main.tile[i, j - 1].HasTile)
                {
                    Main.tile[i, j].TileFrameY = 0;
                }
                else if (!Main.tile[i, j + 1].HasTile || Main.tile[i, j + 1].TileType != Type)
                {
                    Main.tile[i, j].TileFrameY = 54;
                }
                else if (Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].TileType == Type)
                {
                    if (Main.tile[i, j - 2].TileType == Type)
                    {
                        Main.tile[i, j].TileFrameY = 36;
                    }
                    else
                    {
                        Main.tile[i, j].TileFrameY = 18;
                    }
                }
            }
            return true;
        }

        public static void CheckAlgae(int i, int j)
        {
            if (Main.tile[i, j].LiquidType != LiquidID.Water || Main.tile[i, j].LiquidAmount == 0 || (j - 1 > 5 && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].TileType != ModContent.TileType<CrabHydrosailia>()))
            {
                for (int k = j; k < Main.maxTilesY - 5; k++)
                {
                    if (Main.tile[i, k].HasTile && Main.tile[i, k].TileType == ModContent.TileType<CrabHydrosailia>())
                    {
                        WorldGen.KillTile(i, k);
                        continue;
                    }
                    break;
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var texture = TextureAssets.Tile[Type].Value;
            var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 26, 16);

            int topY = j;
            if (Main.tile[i, j].TileFrameY != 0)
            {
                for (int k = topY; k > 10; k--)
                {
                    if (Main.tile[i, k].HasTile && Main.tile[i, k].TileType == Type)
                    {
                        topY = k;
                        continue;
                    }
                    break;
                }
            }
            AequusEffects.EffectRand.SetRand(i * topY + i + topY + Main.tile[i, topY].TileFrameX + Main.tile[i, topY].TileFrameY);
            var drawCoords = new Vector2(i * 16f + 8f, j * 16f);
            drawCoords.Y += (1f - Main.tile[i, topY].LiquidAmount / 255f) * 16f;
            drawCoords.Y += AequusEffects.EffectRand.Rand(-6f);
            if (Main.tile[i, j].TileFrameY != 0)
            {
                drawCoords.X += AequusHelpers.Wave(AequusEffects.EffectRand.Rand(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly - j * 0.5f, -4f, 4f);
            }
            //int amt = (int)AequusEffects.EffectRand.Rand(1f, 50f);
            //float time = Main.GlobalTimeWrappedHourly * AequusEffects.EffectRand.Rand(0.75f, 1f);
            //for (int k = 0; k < amt; k++)
            //{
            //    time += AequusEffects.EffectRand.Rand(0.02f, 0.2f);
            //    var drawLoc = (new Vector2(drawCoords.X, drawCoords.Y) + (new Vector2(0f, 1f).RotatedBy(AequusEffects.EffectRand.Rand(MathHelper.TwoPi))
            //        * AequusEffects.EffectRand.Rand(2f, 3f + Math.Min(k / 2f, 8f)) + new Vector2(0f, k * 2 - 4f).RotatedBy(AequusHelpers.Wave(time, -0.1f, 0.1f)))).NumFloor();
            //    spriteBatch.Draw(texture, drawLoc - Main.screenPosition + AequusHelpers.TileDrawOffset, frame, AequusHelpers.GetColor(drawLoc),
            //        0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            //}

            spriteBatch.Draw(texture, drawCoords.NumFloor(2) - Main.screenPosition + AequusHelpers.TileDrawOffset, frame, Lighting.GetColor(i, j),
                0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}