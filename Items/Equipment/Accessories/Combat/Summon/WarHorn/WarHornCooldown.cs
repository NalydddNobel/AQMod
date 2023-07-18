using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.Summon.WarHorn;

public class WarHornCooldown : ModBuff {
    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.Aequus().cooldownWarHorn = true;
    }
}