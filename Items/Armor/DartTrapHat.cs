using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DartTrapHat : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.defense = 2;
            item.summon = true;
            item.damage = 20;
            item.knockBack = 2f;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.dartHead = true;
            aQPlayer.dartHeadDelay = 180;
            aQPlayer.dartHeadType = ProjectileID.PoisonDart;
            player.minionDamage += 0.05f;
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawAltHair = true;
            drawHair = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.DartTrap);
            r.AddIngredient(ItemID.CopperBar, 8);
            r.AddRecipeGroup("PresurePlate");
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.DartTrap);
            r.AddIngredient(ItemID.TinBar, 8);
            r.AddRecipeGroup("PresurePlate");
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}