using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class ManaDrain : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.magicDamageMult -= 0.8f;
        }
    }
}