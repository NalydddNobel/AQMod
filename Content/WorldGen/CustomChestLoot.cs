using Aequus.Common.Catalogues;
using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Melee;
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
                    int chestType = ChestTypes.GetChestType(c);
                    if (chestType == ChestTypes.Gold || chestType == ChestTypes.deadMans)
                    {
                        GoldChestLoot(c);
                    }
                    else if (chestType == ChestTypes.Frozen)
                    {
                        FrozenChestLoot(c);
                    }
                }
            }
        }
        public void GoldChestLoot(Chest c)
        {
            if (WorldGen.genRand.NextBool(3))
            {
                if (CanAddGlowCore(c))
                {
                    c.Insert(ModContent.ItemType<GlowCore>(), 1);
                }
            }
        }
        public bool CanAddGlowCore(Chest c)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (c.item[i].stack > 0 && (c.item[i].type == ItemID.Torch || c.item[i].type == ItemID.Glowstick))
                {
                    return true;
                }
            }
            return false;
        }
        public void FrozenChestLoot(Chest c)
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