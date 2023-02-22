using Aequus.Items.Misc.Energies;
using Aequus.Items.Tools;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    [GlowMask]
    public class Flameblaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AequusItem.HasWeaponCooldown.Add(Type);
        }

        private void DefaultUse()
        {
            Item.useTime = 3;
            Item.useAnimation = 30;
        }
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<FlameblasterProj>();
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Gel;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.ArmorPenetration = 15;
            DefaultUse();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
                return !player.Aequus().HasCooldown;
            }
            else
            {
                DefaultUse();
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.Aequus().SetCooldown(120, itemReference: Item);
            }
            return true;
        }

        public override bool NeedsAmmo(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return player.altFunctionUse != 2 && player.ItemAnimationJustStarted && Main.rand.NextBool(3);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 4f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position += Vector2.Normalize(velocity) * 30f;
                velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f));
            }
            else
            {
                damage = 0;
                knockback *= 50f;
                type = ModContent.ProjectileType<FlameblasterWind>();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"))
                {
                    t.Text = AequusHelpers.FormatWith(t.Text, new { Color = Colors.AlphaDarken(Color.Red).Hex3() });
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Pumpinator>()
                .AddIngredient(ItemID.Flamethrower)
                .AddIngredient<DemonicEnergy>(3)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.Flamethrower);
        }
    }
}