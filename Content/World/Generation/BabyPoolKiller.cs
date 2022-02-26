using System;
using Terraria;
using Terraria.Localization;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public sealed class BabyPoolKiller
    {
        public static void PassFix1TileHighWater(GenerationProgress progress)
        {
            if (!AQConfigServer.Instance.fixBabyPools)
            {
                return;
            }
            if (progress != null)
                progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.Fix1TileHighWater");

            var logger = AQMod.GetInstance().Logger;
            for (int i = 250; i < Main.maxTilesX - 250; i++) // should ignore beaches and the far side of the world
            {
                for (int j = 50; j < (int)Main.worldSurface; j++)
                {
                    try
                    {
                        ApplyFix(i, j);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("An error occured when generating at: {x:" + i + ", y:" + j + "}");
                        logger.Error(ex);
                    }
                }
            }
        }

        public static void ApplyFix(int x, int y)
        {
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
                return;
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
                return;
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
                return;
            }
            if (Main.tile[x, y - 1].liquid == 0 && (!Main.tile[x, y - 1].active() || Main.tileCut[Main.tile[x, y - 1].type])
                && Main.tile[x, y].liquid > 0 && (!Main.tile[x, y].active() || Main.tileCut[Main.tile[x, y].type]) && Main.tile[x, y + 1].active()
                && Main.tileSolid[Main.tile[x, y + 1].type] && !Main.tileSolidTop[Main.tile[x, y + 1].type] && AQTile.Sets.CanFixWaterOnType[Main.tile[x, y + 1].type])
            {
                Main.tile[x, y].active(active: true);
                Main.tile[x, y].type = Main.tile[x, y + 1].type;
                Main.tile[x, y].slope(slope: Main.tile[x, y + 1].slope());
                Main.tile[x, y].halfBrick(halfBrick: Main.tile[x, y + 1].halfBrick());
                Main.tile[x, y].liquid = 0;

                if (Main.tile[x, y + 2] == null)
                {
                    Main.tile[x, y + 2] = new Tile();
                }
                else if (Main.tile[x, y + 2].active())
                {
                    Main.tile[x, y + 1].type = Main.tile[x, y + 2].type;
                    Main.tile[x, y + 1].slope(slope: 0);
                    Main.tile[x, y + 1].halfBrick(halfBrick: false);
                    Main.tile[x, y + 1].liquid = 0;
                }
                else
                {
                    Main.tile[x, y].active(active: false);
                }

                if (Main.tile[x + 1, y] == null) // helps prevent some ugly world generation marks
                {
                    Main.tile[x + 1, y] = new Tile();
                }
                else if (Main.tile[x + 1, y].active())
                {
                    Main.tile[x + 1, y].slope(slope: 0);
                    Main.tile[x + 1, y].halfBrick(halfBrick: false);
                }

                if (Main.tile[x - 1, y] == null)
                {
                    Main.tile[x - 1, y] = new Tile();
                }
                else if (Main.tile[x - 1, y].active())
                {
                    Main.tile[x - 1, y].slope(slope: 0);
                    Main.tile[x - 1, y].halfBrick(halfBrick: false);
                }

                WorldGen.SquareTileFrame(x, y);
            }
        }
    }
}