using Aequus.Common.Recipes;
using Aequus.Items.Tools;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Scepters
{
    [AutoloadGlowMask]
    public class ZombieScepter : ScepterBase
    {
        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shoot = ModContent.ProjectileType<ZombieBolt>();
            Item.shootSpeed = 9f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 4)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterAfter(ItemID.RainbowRod)
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}