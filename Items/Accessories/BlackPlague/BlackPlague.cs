using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.BlackPlague {
    [AutoloadEquip(EquipType.HandsOn)]
    public class BlackPlague : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accBoneRing++;
            aequus.accBoneBurningRing++;
            aequus.accBlackPhial++;
            aequus.DebuffsInfliction.OverallTimeMultiplier += 0.5f;
            aequus.accResetEnemyDebuffs = true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<PhoenixRing>()
                .AddIngredient<BlackPhial>()
                .AddIngredient<OrganicEnergy>()
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.PapyrusScarab);
        }
    }
}