using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Summon.Sentry
{
    public class LotusStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(CommonUtils.GetPath(this) + "_Glow"), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Lime;
            item.summon = true;
            item.sentry = true;
            item.damage = 195;
            item.useTime = 45;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shoot = ModContent.ProjectileType<Projectiles.Summon.Sentry.Lotus>();
            item.mana = 8;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item45;
            item.noMelee = true;
            item.value = AQItem.EnergyWeaponValue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Projectiles.Summon.Sentry.Lotus.FindSpawn(Main.MouseWorld);
            player.UpdateMaxTurrets();
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.FlowerofFire);
            r.AddIngredient(ModContent.ItemType<OrganicEnergy>(), 10);
            r.AddIngredient(ItemID.ChlorophyteBar, 12);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}