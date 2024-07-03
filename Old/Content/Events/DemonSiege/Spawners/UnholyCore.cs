using Aequu2.Content.Events.DemonSiege;

namespace Aequu2.Old.Content.Events.DemonSiege.Spawners;

public class UnholyCore : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
        // Hack which makes this item get consumed after the event is over, instead of converting into another item
        AltarSacrifices.Register(Type, Type);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1);
    }
}