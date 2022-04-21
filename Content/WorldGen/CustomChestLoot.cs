using Aequus.Common.Catalogues;
using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public sealed class CustomChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest c = Main.chest[i];
                if (c != null && Main.tile[c.x, c.y].TileType == TileID.Containers && c.item[0] != null && !c.item[0].IsAir)
                {
                    int chestType = ChestCatalogue.GetChestType(c);
                    if (chestType == ChestCatalogue.Gold || chestType == ChestCatalogue.deadMans)
                    {
                        GoldChestLoot(c);
                    }
                    else if (chestType == ChestCatalogue.Frozen)
                    {
                        FrozenChestLoot(c);
                    }
                }
            }
        }
        private void GoldChestLoot(Chest c)
        {
            if (WorldGen.genRand.NextBool(3))
            {
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (c.item[i].stack > 0 && (c.item[i].type == ItemID.Torch || c.item[i].type == ItemID.Glowstick))
                    {
                        goto AddGlowCore;
                    }
                }
                return;

            AddGlowCore:
                c.Insert(ModContent.ItemType<GlowCore>(), 1);
            }
        }
        private void FrozenChestLoot(Chest c)
        {
            if (c.item[0].type == ItemID.IceBlade)
            {
                if (WorldGen.genRand.NextBool())
                {
                    c.item[0].SetDefaults(ModContent.ItemType<CrystalDagger>());
                }
            }
        }
    }
}