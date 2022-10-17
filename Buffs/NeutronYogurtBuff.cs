using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class NeutronYogurtBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity *= 1.8f;
        }
    }
}