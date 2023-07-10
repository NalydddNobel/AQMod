using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.RockMan;

public class RockMan : ModItem {
    public override void SetDefaults() {
        Item.width = 30;
        Item.height = 30;
        Item.DamageType = DamageClass.Melee;
        Item.damage = 28;
        Item.knockBack = 5f;
        Item.useTime = 24;
        Item.useAnimation = 24;
        Item.useStyle = ItemUseStyleID.Rapier;
        Item.UseSound = SoundID.Item1;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(gold: 1);
        Item.shootSpeed = 8f;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<RockManProj>();
        Item.autoReuse = true;
        Item.SetItemVariant(ItemVariants.StrongerVariant, Condition.RemixWorld);

        if (Item.Variant == ItemVariants.StrongerVariant) {
            Item.damage = 80;
            Item.rare = ItemRarityID.Green;
            Item.scale = 1.5f;
        }
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedBy((float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.15f);
        position += Vector2.Normalize(velocity) * 10f;
    }
}