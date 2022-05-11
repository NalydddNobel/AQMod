using Aequus.Buffs;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Potions
{
    public class NoonPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(20);
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(255, 255, 255, 0), new Color(255, 100, 10, 0), new Color(255, 255, 40, 0), new Color(255, 200, 20, 0), };

            ItemTooltips.Catalogue.Dedicated[Type] = new ItemTooltips.Catalogue.ItemDedication(new Color(200, 80, 50, 255));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = ItemDefaults.PotionsValue;
            Item.maxStack = 30;
            Item.buffTime = 28800;
            Item.buffType = ModContent.BuffType<NoonBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShinePotion)
                .AddIngredient(ItemID.NightOwlPotion)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient(ItemID.SoulofLight)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}