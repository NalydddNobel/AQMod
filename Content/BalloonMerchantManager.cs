using AQMod.Content.WorldEvents.GaleStreams;
using AQMod.NPCs.Town;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public sealed class BalloonMerchantManager : ModWorld
    {
        public override void PreUpdate()
        {
            if (!GaleStreams.IsActive)
            {
                return;
            }
            int merchant = BalloonMerchant.Find();
            if (merchant == -1 && Main.rand.NextBool(1000))
            {
                SpawnMerchant();
            }
        }

        public static int SpawnMerchant()
        {
            int merchant = NPC.NewNPC(Main.maxTilesX * 16, 2400, ModContent.NPCType<BalloonMerchant>());
            if (merchant != -1)
                SpawnMerchant(merchant);
            return merchant;
        }

        public static void SpawnMerchant(int merchant)
        {
            int halfSizeTile = 25;
            int sizeTile = halfSizeTile * 2;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int x = 60 + Main.rand.Next(0, Main.maxTilesX - 120);
                int y = 60 + Main.rand.Next(0, 100);
                if (checkTiles(new Rectangle(x - halfSizeTile, y - halfSizeTile, sizeTile, sizeTile)) &&
                    checkPlayers(new Rectangle((x - halfSizeTile) * 16, (y - halfSizeTile) * 16, sizeTile * 16, sizeTile * 16)))
                {
                    Main.npc[merchant].position.X = x * 16f - Main.npc[merchant].width / 2f;
                    Main.npc[merchant].position.Y = y * 16f - Main.npc[merchant].height / 2f;
                    Main.npc[merchant].aiStyle = -1;
                    break;
                }
            }
        }

        private static bool checkTiles(Rectangle rect)
        {
            for (int k = rect.X; k < rect.X + rect.Width; k++)
            {
                for (int l = rect.Y; l < rect.Y + rect.Height; l++)
                {
                    if (Main.tile[k, l] == null)
                    {
                        Main.tile[k, l] = new Tile();
                        continue;
                    } 
                    if (Main.tile[k, l].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool checkPlayers(Rectangle rect)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && Main.player[i].getRect().Contains(rect))
                {
                    return false;
                }
            }
            return true;
        }
    }
}