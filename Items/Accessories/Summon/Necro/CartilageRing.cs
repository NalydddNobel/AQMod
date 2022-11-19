using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public class CartilageRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.zombieDebuffMultiplier++;
            aequus.ghostProjExtraUpdates += 1;
            aequus.accBoneRing++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PandorasBox>()
                .AddIngredient<BoneRing>()
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.PapyrusScarab);
        }
    }
}