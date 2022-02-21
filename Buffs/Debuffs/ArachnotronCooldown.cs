using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public sealed class ArachnotronCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().setArachnotronCooldown = true;
        }
    }
}