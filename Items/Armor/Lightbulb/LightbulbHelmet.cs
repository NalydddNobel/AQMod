using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Lightbulb
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

        public override bool DrawHead()
        {
            return false;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaRegenBonus++;
            Lighting.AddLight(new Vector2(player.position.X + player.width / 2f, player.position.Y + 2), new Vector3(0.75f, 0.65f, 0.2f));
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LightbulbBreastplate>() && legs.type == ModContent.ItemType<LightbulbGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<AQPlayer>().setLightbulb = true;
            player.setBonus = Language.GetTextValue("Mods.AQMod.ArmorSetBonus.Lightbulb");
            player.dangerSense = true;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddRecipeGroup(AQRecipes.RecipeGroups.CopperOrTin, 8);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ModContent.ItemType<Materials.Lightbulb>(), 2);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}