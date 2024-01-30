using Aequus.Old.Content.Equipment.Accessories.HighSteaks;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.GrapplingHooks.HealingGrappleHook;

public class LeechHook : ModItem {
    public static System.Int32 ArmorPenetration { get; set; } = 5;
    public static System.Int32 HealAmount { get; set; } = 1;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration);

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HighSteaks>();
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.damage = 6;
        Item.ArmorPenetration += ArmorPenetration;
        Item.knockBack = 0f;
        Item.shoot = ModContent.ProjectileType<LeechHookProj>();
        Item.shootSpeed = 13f;
        Item.noUseGraphic = true;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Green;
        Item.noMelee = true;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveCritChance();
        tooltips.RemoveKnockback();
    }

    public override System.Boolean WeaponPrefix() {
        return true;
    }

    public static void CheckLeechHook(Player player, NPC target) {
        if (target.HasBuff<LeechHookDebuff>()) {
            player.Heal(HealAmount);
        }
    }
}
