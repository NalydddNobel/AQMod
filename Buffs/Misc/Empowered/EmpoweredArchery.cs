using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered {
    public class EmpoweredArchery : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Archery;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.ArcheryPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.archery = true;
            player.arrowDamage *= 1.4f;
        }
    }
}