using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Anchor {
    public class BoneAnchor : ModItem, IDavyJonesAnchor, IBoneHawkRing {
        public int AnchorSpawnChance => 10;
        public int InflictChance => 10;
        public int DebuffDuration => 300;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
            TextHelper.ChanceFrac(AnchorSpawnChance), TextHelper.ChanceFrac(InflictChance));

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
                .AddIngredient<BoneHawkRing>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}