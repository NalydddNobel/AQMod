using Aequus.Items.Prefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Empowered
{
    public class EmpoweredMagicPower : EmpoweredBuffBase
    {
        public override int OriginalBuffType => BuffID.MagicPower;
        public override float StatIncrease => 0.5f;

        public override void SetStaticDefaults()
        {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ItemID.MagicPowerPotion, Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetDamage(DamageClass.Magic) += 0.4f;
        }
    }
}