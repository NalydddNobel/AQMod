using Aequus.Content.CrossMod;
using Aequus.Content.ItemPrefixes.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.Pollen
{
    public class MistralPollen : ModItem
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
            var prefix = PrefixLoader.GetPrefix(ModContent.PrefixType<EmpoweredPrefix>());
            foreach (var pair in EmpoweredPrefix.ItemToEmpoweredBuff)
            {
                if (prefix.CanRoll(ContentSamples.ItemsByType[pair.Key]))
                {
                    var r = Recipe.Create(pair.Key, 1)
                        .AddIngredient(pair.Key)
                        .AddIngredient(Type)
                        .TryRegisterAfter(pair.Key);
                    r.createItem.Prefix(prefix.Type);
                    MagicStorage.AddBlacklistedItemData(pair.Key, prefix.Type);
                }
            }
        }
    }
}