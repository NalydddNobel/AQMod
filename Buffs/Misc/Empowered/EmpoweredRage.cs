using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc.Empowered {
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