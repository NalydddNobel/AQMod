using AQMod.Items.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Legs)]
    public class ArachnotronRevvers : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 10;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 6);
            item.scale = 2f;
        }

        public override void UpdateEquip(Player player)
        {
            player.accRunSpeed = 7.5f;
            player.maxMinions += 1;
            player.fireWalk = true;
            player.moveSpeed += 0.14f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderGreaves);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.HallowedBar, 12);
            r.AddIngredient(ItemID.SoulofFright, 3);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}