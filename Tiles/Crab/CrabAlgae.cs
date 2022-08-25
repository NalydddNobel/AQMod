using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Crab
{
    public class CrabAlgae : ModTile
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

            if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<CrabAlgae>())
            {
                CheckAlgae(x, y);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;

            DustType = DustID.JungleGrass;
            HitSound = SoundID.Grass;

            AddMapEntry((Color.Teal * 0.8f).UseA(255).SaturationMultiply(0.5f));
        }

        public override void RandomUpdate(int i, int j)
        {
            CheckAlgae(i, j);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CheckAlgae(i, j);
            return true;
        }

        public static void CheckAlgae(int i, int j)
        {
            if (Main.tile[i, j].LiquidType != LiquidID.Water || Main.tile[i, j].LiquidAmount == 0)
            {
                WorldGen.KillTile(i, j);
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

            AequusEffects.EffectRand.SetRand(i * j + i + j + Main.tile[i, j].TileFrameX + Main.tile[i, j].TileFrameY);
            var drawCoords = new Vector2(i * 16f + 8f, j * 16f + (1f - Main.tile[i, j].LiquidAmount / 255f) * 16f).NumFloor(2);
            drawCoords.Y += AequusEffects.EffectRand.Rand(2f, 6f);
            int amt = (int)AequusEffects.EffectRand.Rand(1f, 50f);
            float time = Main.GlobalTimeWrappedHourly * AequusEffects.EffectRand.Rand(0.75f, 1f);
            for (int k = 0; k < amt; k++)
            {
                time += AequusEffects.EffectRand.Rand(0.02f, 0.2f);
                var drawLoc = (new Vector2(drawCoords.X, drawCoords.Y) + (new Vector2(0f, 1f).RotatedBy(AequusEffects.EffectRand.Rand(MathHelper.TwoPi))
                    * AequusEffects.EffectRand.Rand(2f, 3f + Math.Min(k / 2f, 8f)) + new Vector2(0f, k * 2 - 4f).RotatedBy(AequusHelpers.Wave(time, -0.1f, 0.1f)))).NumFloor();
                spriteBatch.Draw(texture, drawLoc - Main.screenPosition + AequusHelpers.TileDrawOffset, frame, AequusHelpers.GetColor(drawLoc),
                    0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawCoords - Main.screenPosition + AequusHelpers.TileDrawOffset, frame, Lighting.GetColor(i, j),
                0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}