using Aequus.Common;
using Aequus.Content.Events.DemonSiege;

namespace Aequus.Old.Content.Items.Accessories.RabbitsFoot;

public class RabbitsFoot : ModItem {
    public static float LuckRerolls { get; set; } = 1f;

    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.Bunny, Type, EventTier.PreHardmode);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = Commons.Rare.DemonSiegeLoot;
        Item.value = Commons.Cost.DemonSiegeLoot;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accRabbitFootLuck += LuckRerolls;
    }
}