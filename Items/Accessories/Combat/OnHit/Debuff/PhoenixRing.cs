using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Debuff {
    [AutoloadEquip(EquipType.HandsOn)]
    public class PhoenixRing : BoneRing {
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
                .AddIngredient<BoneRing>()
                .AddIngredient(ItemID.MagmaStone)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.NecroHelmet);
        }
    }
}