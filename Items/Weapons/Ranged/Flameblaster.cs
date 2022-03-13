using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public sealed class Flameblaster : ModItem, ICooldown
    {
        private void DefaultUse()
        {
            item.useTime = 3;
            item.useAnimation = 30;
        }
        public override void SetDefaults()
        {
            item.damage = 32;
            item.ranged = true;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<FlameblasterFire>();
            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Gel;
            item.UseSound = SoundID.Item34;
            item.value = Item.sellPrice(gold: 7, silver: 50);
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 1f;
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
                item.useTime = 60;
                item.useAnimation = 60;
            }
            else
            {
                DefaultUse();
            }
            return true;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return player.altFunctionUse != 2 && Main.rand.NextBool();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4f, 4f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                DefaultUse();
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                aQPlayer.ItemCooldownCheck(Cooldown(player, aQPlayer), item: item);
                AQProjectile.NewWind<FlameblasterWind>(player, position + Vector2.Normalize(new Vector2(speedX, speedY)) * item.width * 2f, new Vector2(speedX, speedY) / 3f, item.knockBack * 3f, 90, 120, 8, hide: true);
                return false;
            }
            if (player.ZoneHoly)
            {
                damage += 5;
            }
            var velocity = new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f));
            speedX = velocity.X;
            speedY = velocity.Y;
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Flamethrower);
            r.AddIngredient(ModContent.ItemType<TheFan>());
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddIngredient(ItemID.PixieDust, 50);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }

        public ushort Cooldown(Player player, AQPlayer aQPlayer)
        {
            return (ushort)(player.altFunctionUse == 2 ? 300 : 0);
        }
    }
}