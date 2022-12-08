using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class TinkerersGuidebook : ModItem
    {
        public override void Load()
        {
            On.Terraria.Item.Prefix += Item_Prefix;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                AequusWorld.tinkererRerolls = 0;
                return true;
            }
            if (AequusWorld.tinkererRerolls < 3)
            {
                AequusWorld.tinkererRerolls += 3;
                return true;
            }

            return false;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinkerersWorkshop)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register();
        }

        private static bool Item_Prefix(On.Terraria.Item.orig_Prefix orig, Item item, int pre)
        {
            if (pre == -2 && AequusWorld.tinkererRerolls > 0 && AequusHelpers.iterations == 0)
            {
                bool val = false;
                int finalPrefix = 0;
                int value = 0;
                var cloneItem = new Item();
                for (int i = 0; i < AequusWorld.tinkererRerolls; i++)
                {
                    AequusHelpers.iterations = i + 1;
                    cloneItem.SetDefaults(item.type);
                    val |= cloneItem.Prefix(pre);
                    int prefixValue = cloneItem.value / 5;
                    if (cloneItem.prefix == PrefixID.Ruthless)
                    {
                        prefixValue = (int)(prefixValue * 2.15f);
                    }
                    if ((cloneItem.pick > 0 || cloneItem.axe > 0 || cloneItem.hammer > 0) && cloneItem.prefix == PrefixID.Light)
                    {
                        prefixValue = (int)(prefixValue * 2.5f);
                    }
                    if (prefixValue > value || finalPrefix == 0)
                    {
                        finalPrefix = cloneItem.prefix;
                        value = prefixValue;
                    }
                }
                AequusHelpers.iterations = 0;
                if (val && finalPrefix > 0)
                {
                    return item.Prefix(finalPrefix);
                }
            }
            return orig(item, pre);
        }
    }
}