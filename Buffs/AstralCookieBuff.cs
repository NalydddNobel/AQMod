using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class AstralCookieBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsWellFed[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wellFed = true;

            player.statDefense += 3;
            player.moveSpeed += 0.3f;

            player.GetDamage(DamageClass.Generic) += 0.075f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.075f;
            player.GetCritChance(DamageClass.Generic) += 3;
            player.GetKnockback(DamageClass.Summon) += 0.75f;

            player.GetDamage(DamageClass.Magic) += 0.025f;
            player.GetCritChance(DamageClass.Magic) += 1;
            player.statManaMax2 += 20;
        }
    }
}