using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredRegeneration : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Regeneration;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.RegenerationPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.lifeRegen += 8;
        }
    }
}