using Aequus.Content.CrossMod;
using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Pollen
{
    public class MorayPollen : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PixieDust);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<SplashPrefix>());
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (prefix.CanRoll(ContentSamples.ItemsByType[i]))
                {
                    var r = Recipe.Create(i, 1)
                        .AddIngredient(i)
                        .AddIngredient(Type)
                        .TryRegisterAfter(i);
                    r.createItem.Prefix(prefix.Type);
                    MagicStorage.AddBlacklistedItemData(i, prefix.Type);
                }
            }
        }
    }
}