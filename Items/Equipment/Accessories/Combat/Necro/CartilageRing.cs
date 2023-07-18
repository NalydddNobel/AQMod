using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Equipment.Accessories.Combat.Necro {
    public class CartilageRing : BoneRing {
        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.zombieDebuffMultiplier++;
            aequus.ghostProjExtraUpdates += 1;
            aequus.accBoneRing.SetAccessory(Item, this);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<PandorasBox>()
                .AddIngredient<BoneRing>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.PapyrusScarab);
        }
    }
}