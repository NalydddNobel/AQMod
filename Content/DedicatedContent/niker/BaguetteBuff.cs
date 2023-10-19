using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.niker;

public class BaguetteBuff : ModBuff {
    public override void SetStaticDefaults() {
        BuffID.Sets.IsFedState[Type] = true;
        BuffID.Sets.IsWellFed[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.wellFed = true;
        player.lifeRegen += 4;
        player.statDefense += 10;
        player.GetDamage(DamageClass.Generic) += 0.1f;
        player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.1f;

        player.GetCritChance(DamageClass.Generic) += 10;

        player.GetKnockback(DamageClass.Summon) += 1f;
        player.moveSpeed += 1f;
    }
}