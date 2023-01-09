using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered
{
    public class EmpoweredMining : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Mining;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.MiningPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.pickSpeed -= 0.4f;
        }
    }
}