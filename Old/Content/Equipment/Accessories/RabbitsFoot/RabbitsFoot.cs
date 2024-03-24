using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;

namespace Aequus.Old.Content.Equipment.Accessories.RabbitsFoot;

public class RabbitsFoot : ModItem {
    public static float LuckRerolls { get; set; } = 1f;

    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.Bunny, Type, EventTier.PreHardmode);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemCommons.Rarity.DemonSiegeTier1Loot;
        Item.value = ItemCommons.Price.DemonSiegeLoot;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accRabbitFootLuck += LuckRerolls;
    }
}