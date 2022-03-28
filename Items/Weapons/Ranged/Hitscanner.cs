using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public sealed class Hitscanner : ModItem
    {
        public override bool OnlyShootOnSwing => true;

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Bullet;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/doomshotgun");
            item.value = Item.sellPrice(gold: 7, silver: 50);
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 1f;
            item.useTime = 10;
            item.useAnimation = 60;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 10; i++)
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)), type, damage, knockBack, player.whoAmI);
                Main.projectile[p].extraUpdates++;
                Main.projectile[p].extraUpdates *= 35;
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Boomstick);
            r.AddIngredient(ItemID.MythrilBar, 12);
            r.AddIngredient(ItemID.SoulofNight, 8);
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}