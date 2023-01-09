using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered
{
    public class EmpoweredThorns : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Thorns;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.ThornsPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (player.thorns < 2f)
            {
                player.thorns = 2f;
            }
        }
    }
}