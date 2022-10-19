using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredRage : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Rage;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.RagePotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetCritChance(DamageClass.Generic) += 20;
        }
    }
}