using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered
{
    public class EmpoweredManaRegeneration : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.ManaRegeneration;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.ManaRegenerationPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.manaRegenBuff = true;
            player.manaRegenBonus += 2;
        }
    }
}