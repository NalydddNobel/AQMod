using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class NeutronYogurtBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsWellFed[Type] = true;
            AequusBuff.IsWellFedButDoesntIncreaseLifeRegen.Add(Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity *= 1.8f;
        }
    }
}