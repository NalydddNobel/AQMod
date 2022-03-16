using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class MagikaAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.defense = 2;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().antiGravityItems = true;
            player.manaCost -= 0.08f;
            player.manaFlower = true;
            player.statManaMax2 += 20;
            player.magicCuffs = true;
            player.manaMagnet = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Amulet>());
            r.AddIngredient(ItemID.ManaFlower);
            r.AddIngredient(ItemID.CelestialCuffs);
            r.AddIngredient(ModContent.ItemType<EquivalenceMachine>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}