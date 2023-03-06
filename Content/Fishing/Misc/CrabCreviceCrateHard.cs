using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Tools.GrapplingHooks;
using Aequus.Items.Weapons.Magic;
using Aequus.Items.Weapons.Ranged.Misc;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Misc
{
    public class CrabCreviceCrateHard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 10;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var l = new ItemLoot(ItemID.OceanCrateHard, Main.ItemDropsDB).Get(includeGlobalDrops: false);
            foreach (var loot in l)
            {
                if (loot is AlwaysAtleastOneSuccessDropRule oneFromOptions)
                {
                    itemLoot.Add(ItemDropRule.OneFromOptions(1,
                        ModContent.ItemType<StarPhish>(), ModContent.ItemType<DavyJonesAnchor>(), ModContent.ItemType<ArmFloaties>()));
                    continue;
                }
                itemLoot.Add(loot);
            }
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.OceanCrateHard);
            Item.createTile = ModContent.TileType<FishingCrates>();
            Item.placeStyle = FishingCrates.CrabCreviceCrateHard;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<CrabCreviceCrate>());
        }
    }
}