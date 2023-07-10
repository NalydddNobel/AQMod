using Aequus.Projectiles.Misc.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood;

public partial class CrownOfBloodItem {
    private void Load_ExpertEffects() {
        On_Player.beeType += On_Player_beeType;
        On_Player.beeDamage += On_Player_beeDamage;
    }

    private static int On_Player_beeDamage(On_Player.orig_beeDamage orig, Player player, int dmg) {
        if (player.TryGetModPlayer<AequusPlayer>(out var aequus) && aequus.crownOfBloodBees > 0) {
            return dmg + Main.rand.Next(8 * aequus.crownOfBloodBees, 12 * aequus.crownOfBloodBees);
        }
        return orig(player, dmg);
    }

    private static int On_Player_beeType(On_Player.orig_beeType orig, Player player) {
        if (player.TryGetModPlayer<AequusPlayer>(out var aequus) && aequus.crownOfBloodBees > 0) {
            if (aequus.crownOfBloodCD <= 0) {
                aequus.crownOfBloodCD = 30;
                return ModContent.ProjectileType<HivePackMinion>();
            }
            return ProjectileID.GiantBee;
        }
        return orig(player);
    }
}