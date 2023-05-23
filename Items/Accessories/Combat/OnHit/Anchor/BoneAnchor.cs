using Aequus.Buffs.Debuffs;
using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Anchor {
    public class BoneAnchor : ModItem, IDavyJonesAnchor, IBoneRing {
        public int AnchorSpawnChance => 10;
        public int DebuffDuration => 30;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            TextHelper.Create.ChanceFracPercent(AnchorSpawnChance), TextHelper.Create.MultiplierPercentDifference(BoneRingWeakness.MovementSpeedMultiplier));

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accDavyJonesAnchor.SetAccessory(Item, this);
            player.Aequus().accBoneRing.SetAccessory(Item, this);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<DavyJonesAnchor>()
                .AddIngredient<BoneRing>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}