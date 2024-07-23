using Aequus.Common.Entities;
using Aequus.Common.Items;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Accessories.FlashwayShield;

[AutoloadEquip(EquipType.Shield)]
public class FlashwayShield : ModItem, ICustomDashProvider {
    public static readonly float DashSpeed = 14.5f;
    float ICustomDashProvider.DashSpeed => DashSpeed;

    bool ICustomDashProvider.ShowShield => true;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = ItemDefaults.NPCSkyMerchant;
        Item.defense = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.dashType != 0 || player.dashDelay != 0) {
            return;
        }

        player.dashType = -1;
        player.GetModPlayer<DashPlayer>().Dash = this;
    }

    void ICustomDashProvider.OnDashVelocityApplied(Player player, int direction) {
        for (int i = 0; i < 10; i++) {
            var d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric);
            d.velocity *= 0.2f;
            d.velocity -= player.velocity * Main.rand.NextFloat(0.5f);
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
        }
    }

    void ICustomDashProvider.OnUpdateRampDown(Player player) {
        if (player.miscCounter % 4 == 0) {
            var d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric);
            d.velocity *= 0.3f;
            d.velocity -= player.velocity * Main.rand.NextFloat(0.3f);
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
        }
    }
}