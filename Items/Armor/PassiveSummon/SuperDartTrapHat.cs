using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.PassiveSummon
{
    [AutoloadEquip(EquipType.Head)]
    public class SuperDartTrapHat : DartTrapHat
    {
        public override int ProjectileShot => ProjectileID.PoisonDartTrap;
        public override int TimeBetweenShots => base.TimeBetweenShots / 2;

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.defense = 20;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 200;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.GetDamage(DamageClass.Summon) += 0.05f;
            player.maxMinions += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SuperDartTrap)
                .AddIngredient(ItemID.ChlorophyteBar, 8)
                .AddIngredient(ItemID.LihzahrdPressurePlate)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}