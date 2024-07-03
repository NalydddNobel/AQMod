using Aequu2.Core.Entities.Players;
using Terraria.Graphics.Shaders;

namespace Aequu2.Content.Items.Accessories.FlashwayShield;

public class FlashwayShieldDashData : CustomDashData {
    public override float DashSpeed => FlashwayShield.DashSpeed;

    public override bool ShowShield => true;

    public override void OnDashVelocityApplied(Player player, AequusPlayer Aequu2Player, int direction) {
        for (int i = 0; i < 10; i++) {
            var d = Terraria.Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric);
            d.velocity *= 0.2f;
            d.velocity -= player.velocity * Main.rand.NextFloat(0.5f);
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
        }
    }

    public override void OnUpdateRampDown(Player player, AequusPlayer Aequu2Player) {
        if (player.miscCounter % 4 == 0) {
            var d = Terraria.Dust.NewDustDirect(player.position, player.width, player.height, DustID.Electric);
            d.velocity *= 0.3f;
            d.velocity -= player.velocity * Main.rand.NextFloat(0.3f);
            d.noGravity = true;
            d.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
        }
    }
}