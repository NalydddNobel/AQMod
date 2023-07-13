using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Cooldowns {
    public class StormcloakCooldown : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}