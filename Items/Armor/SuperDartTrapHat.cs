using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SuperDartTrapHat : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.defense = 20;
            item.summon = true;
            item.damage = 200;
            item.knockBack = 5f;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.dartHead = true;
            aQPlayer.dartHeadDelay = 180;
            aQPlayer.dartHeadType = ProjectileID.PoisonDartTrap;
            player.maxMinions += 2;
            player.minionDamage += 0.1f;
        }

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair)
        {
            drawAltHair = true;
            drawHair = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SuperDartTrap);
            r.AddIngredient(ItemID.ChlorophyteBar, 8);
            r.AddIngredient(ItemID.LihzahrdPressurePlate);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}