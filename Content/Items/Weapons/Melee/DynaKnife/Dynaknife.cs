using Aequus.Common.Items.Components;
using Aequus.Core.Autoloading;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.DynaKnife;

[AutoloadGlowMask]
public class Dynaknife : ModItem, ICooldownItem {
    public int CooldownTime => 120;

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.SetWeaponValues(20, 0.1f, 46);
        Item.DamageType = DamageClass.Melee;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 10);
        Item.shootSpeed = 2f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<DynaknifeProj>();
        Item.scale = 1.1f;
    }

    public override bool AltFunctionUse(Player player) {
        return !this.HasCooldown(player);
    }

    public override bool? UseItem(Player player) {
        if (player.altFunctionUse == 2) {
            int dir = player.direction;
            if (Main.myPlayer == player.whoAmI) {
                dir = Math.Sign(Main.MouseWorld.X - player.Center.X);
            }
            player.velocity.X = 12f * dir;
            this.SetCooldown(player);
        }
        return null;
    }
}
