using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;

namespace Aequus.Buffs.Misc.Empowered {
    public class EmpoweredIronskin : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Ironskin;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.IronskinPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.statDefense += 16;
        }
    }
}