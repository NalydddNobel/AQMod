using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public sealed class CustomChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
            var rockmanChests = new List<int>();
            bool placedMagicalBoomerang = false;
            bool placedRevenantStaff = false;
            bool placedCrystalDagger = false;

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest c = Main.chest[i];
                if (c != null && c.item[0] != null && !c.item[0].IsAir)
                {
                    if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                    {
                        int chestType = ChestTypes.GetChestType(c);
                        if (chestType == ChestTypes.Gold || chestType == ChestTypes.deadMans)
                        {
                            rockmanChests.Add(i);
                            GoldChestLoot(c);
                        }
                        else if (chestType == ChestTypes.LockedGold)
                        {
                            DungeonChestLoot(c, ref placedRevenantStaff, ref placedMagicalBoomerang);
                        }
                        else if (chestType == ChestTypes.Frozen)
                        {
                            rockmanChests.Add(i);
                            FrozenChestLoot(c, ref placedCrystalDagger);
                        }
                        else if (chestType == ChestTypes.Skyware)
                        {
                            FrozenChestLoot(c, ref placedCrystalDagger);
                        }
                    }
                    else if (Main.tile[c.x, c.y].TileType == TileID.Containers2)
                    {
                        int chestType = ChestTypes.GetChestType(c);
                        if (chestType == ChestTypes.Sandstone)
                        {
                            rockmanChests.Add(i);
                        }
                    }
                }
            }
            if (rockmanChests.Count > 0)
            {
                Main.chest[rockmanChests[WorldGen.genRand.Next(rockmanChests.Count)]].Insert(ModContent.ItemType<RockMan>(), WorldGen.genRand.Next(Chest.maxItems - 1));
            }
        }

        public void FrozenChestLoot(Chest c)
        {
            if (c.item[0].type == ItemID.CreativeWings)
            {
                c.Insert(ModContent.ItemType<Slingshot>(), 1);
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