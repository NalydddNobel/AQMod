using AQMod.Assets.LegacyItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class RedSpriteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlay(this.GetPath("_Glow"), new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.magic = true;
            item.damage = 46;
            item.knockBack = 0f;
            item.mana = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 19;
            item.useTime = 19;
            item.UseSound = SoundID.Item66;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.value = AQItem.Prices.GaleStreamsValue;
            item.shootSpeed = 30f;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.RedSpriteStaff>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[p].timeLeft = (int)((Main.MouseWorld - position).Length() / new Vector2(speedX, speedY).Length());
            Main.projectile[p].ai[1] = player.ownedProjectileCounts[type] * 1000f;
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.NimbusRod);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.AtmosphericEnergy>(), 10);
            r.AddIngredient(ModContent.ItemType<Materials.Fluorescence>(), 12);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}