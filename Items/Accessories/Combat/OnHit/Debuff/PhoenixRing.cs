using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Debuff {
    [AutoloadEquip(EquipType.HandsOn)]
    public class PhoenixRing : BoneHawkRing {
        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accBoneRing.SetAccessory(Item, this);
            aequus.accBoneBurningRing++;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<BoneHawkRing>()
                .AddIngredient(ItemID.MagmaStone)
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.NecroHelmet);
        }
    }
}