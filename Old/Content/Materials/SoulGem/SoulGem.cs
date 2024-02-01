using Aequus.Common.Items;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Materials.SoulGem;

public class SoulGem : ModItem {
    public virtual int TransformID => ModContent.ItemType<SoulGemFilled>();

    public override void SetStaticDefaults() {
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<SoulGemTile>());
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1, silver: 50);
    }

    public static void TryFillSoulGems(Player player) {
        var soulGem = player.FindItemInInvOrVoidBag((item) => !item.IsAir && item.ModItem is SoulGem soulGemBase, out bool inVoidBag);
        if (soulGem != null) {
            if (Main.myPlayer == player.whoAmI) {
                soulGem.stack--;
                Item newSoulGem = null;
                if (!inVoidBag) {
                    var canStack = Array.Find(player.inventory, (item) => !item.IsAir && item.type == (soulGem.ModItem as SoulGem).TransformID && item.stack < item.maxStack);
                    if (canStack != null) {
                        newSoulGem = canStack;
                        newSoulGem.stack++;
                    }
                    else if (soulGem.stack <= 0) {
                        soulGem.stack = 1;
                        newSoulGem = soulGem;
                    }
                }
                if (newSoulGem == null && inVoidBag) {
                    var canStack = Array.Find(player.bank4.item, (item) => !item.IsAir && item.type == (soulGem.ModItem as SoulGem).TransformID && item.stack < item.maxStack);
                    if (canStack != null) {
                        newSoulGem = canStack;
                        newSoulGem.stack++;
                    }
                    else {
                        canStack = player.bank4.FindEmptySlot();
                        if (canStack != null) {
                            newSoulGem = canStack;
                        }
                        else if (soulGem.stack <= 0) {
                            soulGem.stack = 1;
                            newSoulGem = soulGem;
                        }
                    }
                }

                if (newSoulGem == null) {
                    newSoulGem = player.QuickSpawnItemDirect(player.GetSource_OpenItem(soulGem.type), (soulGem.ModItem as SoulGem).TransformID);
                    newSoulGem.newAndShiny = !ItemID.Sets.NeverAppearsAsNewInInventory[soulGem.type];
                }
                else {
                    if (newSoulGem.type != (soulGem.ModItem as SoulGem).TransformID) {
                        newSoulGem.SetDefaults((soulGem.ModItem as SoulGem).TransformID);
                    }

                    newSoulGem.Center = player.Top;
                    SoundEngine.PlaySound(SoundID.Grab, player.Top);
                    int p = PopupText.NewText(inVoidBag ? PopupTextContext.ItemPickupToVoidContainer : PopupTextContext.RegularItemPickup, newSoulGem, 1);
                    Main.popupText[p].lifeTime /= 4;
                    Main.popupText[p].lifeTime *= 3;
                }
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.5f, Pitch = 0.3f }, player.Center);
            }
        }
    }
}