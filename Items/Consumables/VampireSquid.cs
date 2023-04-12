using Aequus.Buffs.Misc;
using Aequus.Common.Recipes;
using Aequus.Items.Vanity.Pets.Light;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class VampireSquid : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            this.StaticDefaultsToFood(Color.Red, Color.DarkRed);
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 3);
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.buffTime = 10800;
            Item.buffType = ModContent.BuffType<Vampirism>();
            Item.maxStack = 9999;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<AequusPlayer>().IsVampire;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<AequusPlayer>().GiveVampirism(Item.buffTime);
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<PalePufferfish>());
        }
    }
}