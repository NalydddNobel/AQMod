using Aequus.Common;
using Aequus.Common.PlayerLayers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetGravetender {
    [AutoloadEquip(EquipType.Body)]
    [WorkInProgress]
    public class GravetenderRobes : ModItem {
        public override void SetStaticDefaults() {
            ForceDrawShirt.BodyShowShirt.Add(Item.bodySlot);
        }

        public override void SetDefaults() {
            Item.defense = 3;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player) {
            player.Aequus().ghostSlotsMax++;
            player.Aequus().ghostLifespan += 1800;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 80)
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterBefore(ItemID.GravediggerShovel);
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 80)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterBefore(ItemID.GravediggerShovel);
#endif
        }
    }
}