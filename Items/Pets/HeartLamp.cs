using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Pets
{
    public class HeartLamp : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item2;
            item.value = Item.sellPrice(gold: 5);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<Projectiles.Pets.HeartMoth>();
            item.buffType = ModContent.BuffType<Buffs.Pets.HeartMoth>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 255);

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(item.buffType, 3600, true);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.LifeCrystal, 3);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 2);
            r.AddIngredient(ModContent.ItemType<LightMatter>(), 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}