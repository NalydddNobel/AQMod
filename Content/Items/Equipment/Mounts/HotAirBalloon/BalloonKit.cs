using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Mounts.HotAirBalloon; 

public class BalloonKit : ModItem {
    public override void SetDefaults() {
        Item.DefaultToMount(ModContent.MountType<HotAirBalloonMount>());
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.UseSound = SoundID.Item34;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 10);
    }
}