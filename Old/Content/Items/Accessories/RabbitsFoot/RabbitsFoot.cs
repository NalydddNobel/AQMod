using Aequu2.Core;
using Aequu2.Content.Events.DemonSiege;

namespace Aequu2.Old.Content.Items.Accessories.RabbitsFoot;

public class RabbitsFoot : ModItem {
    public static float LuckRerolls { get; set; } = 1f;

    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.Bunny, Type, EventTier.PreHardmode);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = Commons.Rare.EventDemonSiege;
        Item.value = Commons.Cost.EventDemonSiege;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().accRabbitFootLuck += LuckRerolls;
    }
}