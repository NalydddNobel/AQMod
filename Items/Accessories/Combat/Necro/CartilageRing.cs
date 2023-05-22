using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Accessories.Combat.Necro {
    public class CartilageRing : BoneHawkRing {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

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
                .AddIngredient<BoneHawkRing>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.PapyrusScarab);
        }
    }
}