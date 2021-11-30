using AQMod.Assets.ItemOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HorizonSkies : ModItem
    {
        public static Color Blue => new Color(144, 144, 255, 128);
        public static Color Orange => new Color(150, 110, 66, 128);

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow")), item.type);
        }

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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 15);
            recipe.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            recipe.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 10);
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}