using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class PickBreak : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed *= 2;
            player.GetModPlayer<AQPlayer>().pickBreak = true;
        }
    }
}