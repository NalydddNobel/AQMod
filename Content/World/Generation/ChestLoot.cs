using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.World.Generation
{
    public class ChestLoot : ModWorld
    {
        public override void PostWorldGen()
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest c = Main.chest[i];
                if (c != null && Main.tile[c.x, c.y].type == TileID.Containers)
                {
                    AddLoot(i);
                }
            }
        }

        public static void AddLoot(int i)
        {
            Chest c = Main.chest[i];
            switch (Constants.ChestStyles.GetChestStyle(c))
            {
                case Constants.ChestStyles.Wood:
                {
                    if (WorldGen.genRand.NextBool(4))
                    {
                        MainLoot(c, ModContent.ItemType<Items.Weapons.Melee.VineSword>());
                    }
                }
                break;

                case Constants.ChestStyles.Ice:
                {
                    if (WorldGen.genRand.NextBool(4))
                    {
                        MainLoot(c, ModContent.ItemType<Items.Weapons.Melee.CrystalDagger>());
                    }
                }
                break;
            }
        }

        public static void MainLoot(Chest chest, int item)
        {
            if (ModContent.GetInstance<AQConfigClient>().OverrideVanillaChestLoot)
            {
                chest.item[0].SetDefaults(item);
            }
            else
            {
                InsertLoot(chest, item, 0, CountAllActiveItemIndices(chest));
            }
        }

        public static int CountAllActiveItemIndices(Chest chest)
        {
            int i = 0;
            for (int j = 0; j < Chest.maxItems; j++)
            {
                if (chest.item[j] != null && chest.item[j].type > 0)
                {
                    i++;
                }
            }
            return i;
        }

        public static void InsertLoot(Chest shop, int itemID, int interceptPoint, int maxItems)
        {
            if (maxItems >= Chest.maxItems)
                maxItems = Chest.maxItems - 1;
            for (int j = maxItems; j > interceptPoint; j--)
            {
                shop.item[j] = shop.item[j - 1];
            }
            shop.item[interceptPoint] = new Item(); // removes the object reference in the slot after this
            shop.item[interceptPoint].SetDefaults(itemID);
        }
    }
}