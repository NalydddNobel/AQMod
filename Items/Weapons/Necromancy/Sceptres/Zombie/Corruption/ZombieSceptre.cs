using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption {
    [AutoloadGlowMask]
    public class ZombieSceptre : SceptreBase {
        public override Color GlowColor => Color.Blue;
        public override int DustSpawn => ModContent.DustType<ZombieSceptreParticle>();

        public override void SetDefaults() {
            Item.DefaultToNecromancy(10);
            Item.SetWeaponValues(4, 1f, 0);
            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<ZombieSceptreProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 6;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 6)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.RainbowRod);
        }
    }
}