using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class BaguetteBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsWellFed[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wellFed = true;
            player.lifeRegen += 4;
            player.statDefense += 10;
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.meleeSpeed += 0.1f;

            player.GetCritChance(DamageClass.Generic) += 10;

            player.minionKB += 1f;
            player.moveSpeed += 1f;
        }
    }
}