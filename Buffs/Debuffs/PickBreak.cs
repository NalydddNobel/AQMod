using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class PickBreak : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed *= 2;
            player.GetModPlayer<AequusPlayer>().pickBreak = true;
        }
    }
}