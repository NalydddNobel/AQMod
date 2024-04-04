using Aequus.Common.Items.Components;
using System;
using System.Collections.Generic;

namespace Aequus.Items.Tools.Cameras.CarpenterCamera;

public class Shutterstocker : ModItem, ICooldownItem {
    public int CooldownTime => 600;

    public override void SetStaticDefaults() {
        ItemSets.GamepadWholeScreenUseRange[Type] = true;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1);
        Item.shoot = ModContent.ProjectileType<ShutterstockerHeldProj>();
        Item.shootSpeed = 1f;
        Item.useTime = 28;
        Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.channel = true;
        Item.noUseGraphic = true;
    }

    public override bool CanUseItem(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return false;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (Item.buy || !Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpenter) || carpenter.SelectedBounty <= -1) {
            return;
        }
    }

    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
        return line.Mod != "Aequus" || !line.Name.StartsWith("Bounty");
    }
}