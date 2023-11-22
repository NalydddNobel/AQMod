using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.Baguette;

public class Baguette : ModItem, IDedicatedItem {
    public string DedicateeName => "niker";

    public string DisplayedDedicateeName => null;

    public Color TextColor => new Color(127, 92, 32);

    public override void SetStaticDefaults() {
        this.StaticDefaultsToFood(new Color(194, 136, 36, 255), new Color(147, 103, 27, 255), new Color(100, 49, 2, 255));
    }

    public override void SetDefaults() {
        Item.DefaultToFood(20, 20, ModContent.BuffType<BaguetteBuff>(), 216000);
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 10);
        Item.maxStack = 29;
    }
}