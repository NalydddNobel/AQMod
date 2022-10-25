using Aequus.Buffs;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class SplashPrefix : ModPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item)
        {
            item.useStyle = ItemUseStyleID.Swing;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<SplashPotionProj>();
            item.shootSpeed = 10f;
            item.noUseGraphic = true;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1f;
        }

        public override bool CanRoll(Item item)
        {
            return item.buffType > 0 && item.buffTime > 600 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && item.shoot == ProjectileID.None && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffsBlacklist.Contains(item.buffType);
        }
    }
}
