using AQMod.Items.Materials;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.LightbulbArmor
{
    [AutoloadEquip(EquipType.Head)]
    public class LightbulbHelmet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 1;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            player.manaRegenBonus++;
            Lighting.AddLight(new Vector2(player.position.X + player.width / 2f, player.position.Y + 2), new Vector3(0.4f, 0.35f, 0.1f));
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LightbulbBreastplate>() && legs.type == ModContent.ItemType<LightbulbGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AQText.ArmorSetBonus("Lightbulb").Value;
            player.dangerSense = true;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CopperBar, 8);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 2);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.TinBar, 8);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 2);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}