using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Cooldowns {
    public class FlashwayNecklaceCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}