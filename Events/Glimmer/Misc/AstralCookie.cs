using Aequus.Buffs;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.Glimmer.Misc
{
    public class AstralCookie : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            this.StaticDefaultsToFood(new Color(150, 100, 100, 255), new Color(150, 100, 100, 255), new Color(150, 100, 100, 255), new Color(55, 35, 35, 255), new Color(120, 10, 150, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<AstralCookieBuff>(), 36000);
            Item.rare = ItemDefaults.RarityOmegaStarite - 2;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}

namespace Aequus.Buffs
{
    public class AstralCookieBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsFedState[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wellFed = true;

            player.statDefense += 3;
            player.moveSpeed += 0.3f;

            player.GetDamage(DamageClass.Generic) += 0.075f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.075f;
            player.GetCritChance(DamageClass.Generic) += 3;
            player.GetKnockback(DamageClass.Summon) += 0.75f;

            player.GetDamage(DamageClass.Magic) += 0.025f;
            player.GetCritChance(DamageClass.Magic) += 1;
            player.statManaMax2 += 20;
        }
    }
}