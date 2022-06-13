using Aequus.Items.Weapons.Summon.Candles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public class AequusTile : GlobalTile
    {
        public const int ShadowOrbDrops_Aequus = 5;

        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.ShadowOrbs && Main.tile[i, j].TileFrameX % 36 == 0 && Main.tile[i, j].TileFrameY % 36 == 0)
            {
                if (Main.tile[i, j].TileFrameX < 36)
                {
                    CorruptionOrbDrops(i, j);
                }
                else
                {
                    CrimsonOrbDrops(i, j);
                }
                AequusWorld.shadowOrbsBrokenTotal++;
            }
            return true;
        }
        public void CrimsonOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            Main.NewText(c);
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CrimsonCandle>());
                    break;
            }
        }
        public void CorruptionOrbDrops(int i, int j)
        {
            int c = OrbDrop();
            Main.NewText(c);
            switch (c)
            {
                case 1:
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16f, j * 16f), 32, 32, ModContent.ItemType<CorruptionCandle>());
                    break;
            }
        }
        public int OrbDrop()
        {
            return AequusWorld.shadowOrbsBrokenTotal < ShadowOrbDrops_Aequus ? AequusWorld.shadowOrbsBrokenTotal : WorldGen.genRand.Next(ShadowOrbDrops_Aequus);
        }
    }
}