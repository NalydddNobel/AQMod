using Aequus;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Ranged.Darts.StarPhish;

public class StarPhish : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 8;
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 18;
        Item.useTime = 31;
        Item.useAnimation = 31;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2f;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Dart;
        Item.shoot = ProjectileID.Seed;
        Item.shootSpeed = 25f;
        Item.UseSound = SoundID.Item65;
        Item.noMelee = true;
        Item.rare = ItemDefaults.Rarity.PollutedOceanLoot;
        Item.value = ItemDefaults.Price.PollutedOceanLoot;
    }

    public override void UseItemFrame(Player player) {
        player.bodyFrame.Y = player.bodyFrame.Height * 2;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(4f, -4f);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        int amt = Main.rand.Next(35, 50);
        var direction = Vector2.Normalize(velocity);
        var offset = new Vector2(-6f + -2f * player.direction, -10f);
        int dustId = Dust.dustWater();
        for (int i = 0; i < amt; i++) {
            var d = Dust.NewDustDirect(player.MountedCenter + direction * 36f + offset, 10, 10, dustId);
            d.velocity = direction.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(2f, 5f);
            d.noGravity = true;
        }
        return true;
    }
}