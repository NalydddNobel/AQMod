using Aequus.Items.Accessories;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class BagPools : GlobalItem
    {
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (context == "lockBox")
            {
                player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.LockBox), AequusSystem.DungeonChestItem(Main.rand.Next(AequusSystem.DungeonChestItemTypesMax)));
            }
            else if (context == "obsidianLockBox")
            {
                if (Main.rand.NextBool(3))
                    player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.ObsidianLockbox), ModContent.ItemType<AshCandle>());
            }
            else if (context == "crate")
            {
                if (arg == ItemID.IronCrate)
                {
                    if (Main.rand.NextBool(6))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<GlowCore>());
                    }
                }
                else if (arg == ItemID.FloatingIslandFishingCrate || arg == ItemID.FloatingIslandFishingCrateHard)
                {
                    if (Main.rand.NextBool(3))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<Slingshot>());
                    }
                }
                else if (arg == ItemID.FrozenCrate || arg == ItemID.FrozenCrateHard)
                {
                    if (Main.rand.NextBool(3))
                    {
                        player.QuickSpawnItem(player.GetSource_OpenItem(arg), ModContent.ItemType<Slingshot>());
                    }
                }
            }
        }
    }
}