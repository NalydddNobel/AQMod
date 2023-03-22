using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Misc
{
    [AutoloadEquip(EquipType.Head)]
    public class VenomDartTrapHat : DartTrapHat
    {
        public override int ProjectileShot => ModContent.ProjectileType<VenomDartTrapHatProj>();
        public override int TimeBetweenShots => (int)(base.TimeBetweenShots * 0.66f);
        public override float Speed => base.Speed * 1.5f;
        public override int Damage => 100;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.defense = 10;
            Item.damage = Damage;
            Item.DamageType = DamageClass.Summon;
            Item.ArmorPenetration = 15;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.maxMinions++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VenomDartTrapHat>()
                .AddIngredient(ItemID.SpiderFang, 2)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterBefore(ItemID.CopperBar);
        }
    }
}