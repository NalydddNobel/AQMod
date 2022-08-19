using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Passive
{
    [AutoloadEquip(EquipType.Head)]
    public class SuperDartTrapHat : DartTrapHat
    {
        public override int ProjectileShot => ModContent.ProjectileType<SuperDartTrapHatProj>();
        public override int TimeBetweenShots => base.TimeBetweenShots / 2;

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.defense = 10;
            Item.DamageType = DamageClass.Summon;
            Item.damage = 200;
            Item.ArmorPenetration = 15;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.GetDamage(DamageClass.Summon) += 0.4f;
            player.maxMinions += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SuperDartTrap)
                .AddIngredient(ItemID.ChlorophyteBar, 8)
                .AddIngredient(ItemID.LihzahrdPressurePlate)
                .AddTile(TileID.MythrilAnvil)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.CopperBar));
        }
    }
}