using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility;

public class BoneAnchor : ModItem, Anchor.IDavyJonesAnchor, BoneRing.IBoneRing {
    public int AnchorSpawnChance => 10;
    public int DebuffDuration => 30;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
        TextHelper.Create.ChanceFracPercent(AnchorSpawnChance),
        TextHelper.Create.PercentDifference(BoneRing.BoneRingDebuff.MovementSpeedMultiplier),
        DebuffDuration / 60f);

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetEntry(this, new EquipBoostEntry(this.GetLocalization("BoostTooltip").WithFormatArgs(
            TextHelper.Create.ChanceFracPercent(AnchorSpawnChance),
            TextHelper.Create.PercentDifference(BoneRing.BoneRingDebuff.MovementSpeedMultiplier),
            DebuffDuration * 2 / 60f)
        ));
    }

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
            .AddIngredient<Anchor.DavyJonesAnchor>()
            .AddIngredient<BoneRing.BoneRing>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}