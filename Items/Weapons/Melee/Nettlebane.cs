using Aequus.Buffs;
using Aequus.Content.WorldGeneration;
using Aequus.Projectiles.Melee.Swords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class Nettlebane : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            HardmodeChestBoost.HardmodeJungleChestLoot.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<NettlebaneProj>(46);
            Item.SetWeaponValues(86, 4f);
            Item.width = 30;
            Item.height = 30;
            Item.scale = 1.25f;
            Item.rare = ItemDefaults.RarityPreMechs;
            Item.value = ItemDefaults.ValueEarlyHardmode;
            Item.autoReuse = true;
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RichMahogany, 100)
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.OnyxBlaster);
        }
    }
}