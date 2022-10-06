using Aequus.Projectiles.Misc.GrapplingHooks;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.GrapplingHooks
{
    public class Meathook : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 35;
            Item.knockBack = 1f;
            Item.shoot = ModContent.ProjectileType<MeathookProj>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.LightRed;
            Item.noMelee = true;
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveCritChance();
        }
    }
}