using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material;

public class Fluorescence : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SoulOfFlight;
        ItemID.Sets.AnimatesAsSoul[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<FrozenTear>();
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemCommons.Rarity.SpaceStormLoot - 1;
        Item.value = Item.sellPrice(silver: 15);
        Item.GetGlobalItem<AequusItem>().itemGravityCheck = 255;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }
}