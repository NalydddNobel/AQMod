using Aequus.Common;
using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public sealed class CustomChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            bool placedMagicalBoomerang = false;
            bool placedRevenantStaff = false;
            bool placedCrystalDagger = false;

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
                    else if (chestType == ChestTypes.LockedGold)
                    {
                        DungeonChestLoot(c, ref placedRevenantStaff, ref placedMagicalBoomerang);
                    }
                    else if (chestType == ChestTypes.Frozen)
                    {
                        FrozenChestLoot(c, ref placedCrystalDagger);
                    }
                }
            }
        }

        public void GoldChestLoot(Chest c)
        {
            if (WorldGen.genRand.NextBool(3))
            {
                if (GoldChestLoot_CheckGlowCore(c))
                {
                    c.Insert(ModContent.ItemType<GlowCore>(), 1);
                }
            }
        }
        public bool GoldChestLoot_CheckGlowCore(Chest c)
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

        public void DungeonChestLoot(Chest c, ref bool placedRevenantStaff, ref bool placedMagicalBoomerang)
        {
            if (c.item[0] != null)
            {
                if (c.item[0].type == ItemID.MagicMissile)
                {
                    if (!placedRevenantStaff || Main.rand.NextBool())
                    {
                        placedRevenantStaff = true;
                        c.Insert(ModContent.ItemType<Revenant>(), 1);
                    }
                }
                else if (c.item[0].type == ItemID.Valor)
                {
                    if (!placedMagicalBoomerang || Main.rand.NextBool())
                    {
                        placedMagicalBoomerang = true;
                        c.Insert(ModContent.ItemType<Valari>(), 1);
                    }
                }
            }
        }

        public void FrozenChestLoot(Chest c, ref bool placedCrystalDagger)
        {
            if (c.item[0].type == ItemID.IceBlade)
            {
                if (!placedCrystalDagger || WorldGen.genRand.NextBool())
                {
                    placedCrystalDagger = true;
                    c.Insert(ModContent.ItemType<CrystalDagger>(), 1);
                }
            }
        }
    }
}