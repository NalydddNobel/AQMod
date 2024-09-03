using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Common.Recipes;

namespace Aequus.Content.Items.Consumable.ShimmerPowerups;

public class TinkerersGuidebook : ModItem {
    public static readonly int RerollCount = 3;

    public override void Load() {
        On_Item.Prefix += Item_Prefix;
    }

    public override void SetStaticDefaults() {
        AequusRecipes.AddShimmerCraft(ItemID.TinkerersWorkshop, ModContent.ItemType<TinkerersGuidebook>(), condition: AequusConditions.DownedOmegaStarite);
    }

    public override void SetDefaults() {
        Item.DefaultToHoldUpItem();
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override bool AltFunctionUse(Player player) {
        return true;
    }

    public override bool? UseItem(Player player) {
        if (player.altFunctionUse == 2) {
            if (AequusWorld.UsedTinkererBook) {
                TextHelper.Broadcast("Announcement.DisabledEffect", Color.LightGray, Lang.GetItemName(Type).ToNetworkText());
            }
            AequusWorld.UsedTinkererBook = false;
            return true;
        }

        if (!AequusWorld.UsedTinkererBook) {
            TextHelper.Broadcast("Announcement.TinkerersGuidebook", TextHelper.EventMessageColor);
            AequusWorld.UsedTinkererBook = true;
            return true;
        }

        return false;
    }

    public override bool ConsumeItem(Player player) {
        return player.altFunctionUse != 2;
    }

    private static bool Item_Prefix(On_Item.orig_Prefix orig, Item item, int pre) {
        if (pre != -2 || Helper.iterations != 0) {
            return orig(item, pre);
        }

        int rerolls = AequusWorld.GetTinkererRerollCount();

        bool val = false;
        int finalPrefix = 0;
        int value = 0;
        var cloneItem = new Item();
        for (int i = 0; i < rerolls; i++) {
            Helper.iterations = i + 1;
            cloneItem.SetDefaults(item.type);
            val |= cloneItem.Prefix(pre);
            int prefixValue = cloneItem.value / 5;
            if (cloneItem.prefix == PrefixID.Ruthless) {
                prefixValue = (int)(prefixValue * 2.15f);
            }
            if ((cloneItem.pick > 0 || cloneItem.axe > 0 || cloneItem.hammer > 0) && cloneItem.prefix == PrefixID.Light) {
                prefixValue = (int)(prefixValue * 2.5f);
            }
            if (prefixValue > value || finalPrefix == 0) {
                finalPrefix = cloneItem.prefix;
                value = prefixValue;
            }
        }
        Helper.iterations = 0;
        if (val && finalPrefix > 0) {
            return item.Prefix(finalPrefix);
        }
        return orig(item, pre);
    }
}