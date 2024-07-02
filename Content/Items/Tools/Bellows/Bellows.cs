using Aequus.Core;
using System.Collections.Generic;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Content.Items.Tools.Bellows;

[FilterOverride(FilterOverride.Tools)]
public class Bellows : ModItem {
    public static float MountPushForcePenalty { get; set; } = 0.33f;

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.knockBack = 0.3f;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.reuseDelay = 5;
        Item.UseSound = SoundID.DoubleJump;
        Item.autoReuse = true;
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;
        Item.shoot = ModContent.ProjectileType<BellowsProj>();
        Item.shootSpeed = 1f;
        Item.noUseGraphic = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        try {
            tooltips.Insert(tooltips.GetIndex("Knockback"), new TooltipLine(Mod, "Knockback", XLanguage.GetKnockbackText(Item.knockBack)));
            tooltips.Insert(tooltips.GetIndex("Speed"), new TooltipLine(Mod, "Speed", XLanguage.GetUseAnimationText(Item.useAnimation)));
        }
        catch {

        }
    }
}