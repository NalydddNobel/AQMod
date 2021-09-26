using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic.Tomes
{
    public class PentalScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new DynamicInventoryGlowmask(GlowID.PentalScythe, getGlowmaskColor), item.type);
        }

        private static Color getGlowmaskColor() => Color.Lerp(new Color(188, 175, 135, 0), new Color(144, 130, 111, 0), ((float)Math.Sin(Main.GlobalTime * 1.1f) + 1f) / 2f);

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.useTime = 40;
            item.useAnimation = 40;
            item.width = 24;
            item.height = 24;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<Projectiles.FlamingScythe>();
            item.shootSpeed = 24.11f;
            item.mana = 24;
            item.autoReuse = true;
            item.UseSound = SoundID.Item8;
            item.value = AQItem.EnergyWeaponValue;
            item.knockBack = 2.43f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float rot = new Vector2(speedX, speedY).ToRotation();
            float rotOffset = MathHelper.PiOver2 / 5f;
            float speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            for (int i = 0; i < 5; i++)
            {
                Vector2 velo = new Vector2(1f, 0f).RotatedBy(rot + (i - 2) * rotOffset);
                Projectile.NewProjectile(position + velo * 10f, velo, type, damage, knockBack, player.whoAmI, 0f, speed * 0.01f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemonScythe);
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 10);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}