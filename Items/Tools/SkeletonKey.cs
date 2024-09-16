using Aequus.Common.Tiles;
using Aequus.Content.CrossMod.SplitSupport;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools {
    public class SkeletonKey : ModItem, IPostSetupContent
    {
        public static readonly HashSet<int> OtherDungeonLockBoxes = new();

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public void PostSetupContent() {
            Split.AddItem("DungeonEnvelope", OtherDungeonLockBoxes);
        }

        public override void Unload() {
            OtherDungeonLockBoxes.Clear();
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
            Item.value = Item.buyPrice(gold: 15);
            Item.rare++;
        }

        public static bool UseSkeletonKey(Item[] inv, int slot, Player player, AequusPlayer aequus) {
            if (inv[slot].type == ItemID.LockBox && aequus.HasSkeletonKey) {
                if (ItemLoader.ConsumeItem(inv[slot], player)) {
                    inv[slot].stack--;
                }
                if (inv[slot].stack < 0) {
                    inv[slot].SetDefaults();
                }
                SoundEngine.PlaySound(SoundID.Unlock);
                Main.stackSplit = 30;
                Main.mouseRightRelease = false;
                player.OpenLockBox(inv[slot].type);
                Recipe.FindRecipes();
                return true;
            }
            if (OtherDungeonLockBoxes.Contains(inv[slot].type)) {
                ItemLoader.RightClick(inv[slot], player);
                Main.ItemDropSolver.TryDropping(new() {
                    item = inv[slot].type,
                    player = player,
                    IsExpertMode = Main.expertMode,
                    IsMasterMode = Main.masterMode,
                    rng = Main.rand,
                });
                return true;
            }
            return false;
        }
    }

    public class SkeletonKeyGlobalTile : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            if (type != TileID.Containers 
                || ChestType.GetStyle(Main.tile[i, j].TileFrameX) != ChestType.LockedGold
                || !Main.LocalPlayer.Aequus().HasSkeletonKey) {
                return;
            }

            i -= Main.tile[i, j].TileFrameX % 36 / 18;
            j -= Main.tile[i, j].TileFrameY % 36 / 18;
            Chest.Unlock(i, j);
        }
    }

}