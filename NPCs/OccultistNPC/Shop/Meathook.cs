using Aequus.Items;
using Aequus.Projectiles.Misc.GrapplingHooks;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.OccultistNPC.Shop
{
    public class Meathook : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DualHook);
            Item.damage = 30;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<MeathookProj>();
            Item.value = Item.buyPrice(gold: 10);
            Item.shootSpeed /= 2f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveCritChance();
            tooltips.RemoveKnockback();
        }

        public override bool WeaponPrefix()
        {
            return true;
        }
    }
}