using Aequus.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class BoundedPrefix : ModPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item)
        {
            item.Aequus().prefixPotionsBounded = true;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.persistentBuff[item.buffType] && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffsBlacklist.Contains(item.buffType);
        }
    }
}
