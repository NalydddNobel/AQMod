using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Tiles
{
    public class TEGlimmeringStatue : ModTileEntity
    {
        public bool discovered;

        public TEGlimmeringStatue()
        {
            discovered = true;
        }

        public override bool ValidTile(int i, int j)
        {
            var t = Framing.GetTileSafely(i, j);
            return t.active() && t.type == ModContent.TileType<GlimmeringStatue>() && t.frameX == 0 && t.frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - 1, j - 2, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i - 1, j - 2);
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["discovered"] = discovered,
            };
        }

        public override void Load(TagCompound tag)
        {
            discovered = tag.GetBool("discovered");
        }
    }
}