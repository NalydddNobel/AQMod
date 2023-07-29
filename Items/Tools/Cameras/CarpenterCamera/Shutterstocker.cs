using Aequus;
using Aequus.Common.Carpentry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Tools.Cameras.CarpenterCamera;

public class Shutterstocker : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        AequusItem.HasCooldown.Add(Type);
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
        return !player.Aequus().HasCooldown;
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) {
        return false;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (Item.buy || !Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpenter) || carpenter.SelectedBounty <= -1)
            return;
    }

    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
        return line.Mod != "Aequus" || !line.Name.StartsWith("Bounty");
    }
}