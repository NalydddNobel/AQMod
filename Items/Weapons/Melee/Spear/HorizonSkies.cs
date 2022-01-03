using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Spear
{
    public class HorizonSkies : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<HorizonSkies>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public static Color Blue => new Color(144, 144, 255, 128);
        public static Color Orange => new Color(150, 110, 66, 128);

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.melee = true;
            item.knockBack = 5.45f;
            item.rare = ItemRarityID.LightPurple;
            item.damage = 62;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.HorizonSkies>();
            item.shootSpeed = 7f;
            item.value = Item.sellPrice(gold: 5);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] < 1;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.HallowedBar, 15);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 10);
            r.AddIngredient(ItemID.SoulofLight, 8);
            r.AddIngredient(ItemID.SoulofNight, 8);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}