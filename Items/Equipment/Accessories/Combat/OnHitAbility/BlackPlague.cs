using Aequus;
using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility;

[AutoloadEquip(EquipType.HandsOn)]
public class BlackPlague : Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing.BoneRing {
    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(gold: 5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.Aequus();
        aequus.accBoneRing.SetAccessory(Item, this);
        aequus.accBoneBurningRing++;
        aequus.accBlackPhial++;
        aequus.DebuffsInfliction.OverallTimeMultiplier += 0.5f;
        aequus.accResetEnemyDebuffs = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<BoneRing.PhoenixRing>()
            .AddIngredient<BlackPhial.BlackPhial>()
            .AddIngredient<OrganicEnergy>()
            .AddTile(TileID.TinkerersWorkbench)
            .TryRegisterBefore(ItemID.PapyrusScarab);
    }
}