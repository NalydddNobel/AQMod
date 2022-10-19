using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredWrath : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.Wrath;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.WrathPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetDamage(DamageClass.Generic) += 0.2f;
        }
    }
}