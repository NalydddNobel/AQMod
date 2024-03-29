﻿using Aequus.Items.Tools.Cameras.MapCamera.Clip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras.MapCamera;

public class PixelCamera : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.GamepadExtraRange[Type] = 400;
        AequusItem.HasCooldown.Add(Type);
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 5);
        Item.useAmmo = PixelCameraClipAmmo.AmmoID;
        Item.shoot = ModContent.ProjectileType<PixelCameraHeldProj>();
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
}